using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client
{
    class ApiZeroCDN
    {
        /// <summary>
        /// Поля
        /// </summary>

        private static String userName;
        private static String pasOrKey;

        private static String idToServer;

        private static String baseUrl = "http://mng.zerocdn.com/api/v2/users/";

        private static String postfixUsers = "users.json";
        private static String postfixDirectories = "folders.json";

        private static String urlDirectoryIdWithPassword = baseUrl + postfixDirectories + idToServer + ".json" + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlDirectoryIdWithKey = baseUrl + postfixDirectories + idToServer + ".json";

        private static String urlFileIdWithKey = baseUrl + "files/" + idToServer + ".json" + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlFileIdWithPassword = baseUrl + "files/" + idToServer + ".json";

        private static String urlFileWithKey = baseUrl + postfixUsers + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlDirectoryWithKey = baseUrl + postfixDirectories + "?username=" + userName + "&api_key=" + pasOrKey;

        private static String urlFileWithPassword = baseUrl + postfixDirectories;
        private static String urlDirectoryWithPassword = baseUrl + postfixUsers;

        private enum typeAuthorization
        {
            LoginAndAPiKey,
            LoginAndPassword
        }
        private static typeAuthorization typeAuth;

        private static List<DirectoryFromServer> existsDirectories = new List<DirectoryFromServer>();

        /// <summary>
        /// Авторизация
        /// </summary>

        public static String AuthLoginPassword(String username, String password)
        {
            userName = username;
            pasOrKey = password;

            WebClient client = new WebClient();

            client.Credentials = new NetworkCredential(userName, pasOrKey);

            String credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + pasOrKey));
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            try
            {
                StreamReader reader = new StreamReader(client.DownloadString(urlFileWithPassword));
                typeAuth = typeAuthorization.LoginAndPassword;

                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        return response.StatusCode.ToString();
                    }
                }

                return ex.Status.ToString();
            }
        }

        public static String AuthLoginKey(String username, String apiKey)
        {
            userName = username;
            pasOrKey = apiKey;

            WebClient client = new WebClient();

            try
            {
                var response = client.DownloadString(urlFileWithKey);
                StreamReader reader = new StreamReader(response);

                typeAuth = typeAuthorization.LoginAndAPiKey;

                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }

        /// <summary>
        /// Взаимодействие с файлами
        /// </summary>

        public static String LoadFileToDirectory(int idDirectory, String pathToFile)
        {
            if (pathToFile.Length == 0)
            {
                return null;
            }

            idToServer = idDirectory.ToString();
            if (IsExistDirectoryId())
            {
                return "-1";
            }

            WebClient client = new WebClient();

            idToServer = idDirectory.ToString();
            NameValueCollection data = new NameValueCollection
            {
                { "Content-Type", "application/json" },
                { "file", "@" + pathToFile },
                { "folder", idToServer.ToString() }
            };

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileWithKey : urlFileWithPassword;

            try
            {
                var response = client.UploadValues(url, data);

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }

        public static String RenameFile(String newNameFile)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            if (IsExistDirectoryName(newNameFile))
            {
                return "-1";
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileIdWithKey : urlFileIdWithPassword;

            return Rename(url, newNameFile);
        }

        public static String DeleteFiles(int id)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            idToServer = id.ToString();

            if (IsExistFileId())
            {
                return "-1";
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileIdWithKey : urlFileIdWithPassword;
           
            return Delete(url, id);
        }

        public static String ListFiles()
        {
            return "-1";
        }

        /// <summary>
        /// Взаимодействие с директориями
        /// </summary>

        public static String CreateDirectory(String nameNewDirectory)
        {
            if (nameNewDirectory.Length == 0 || typeAuth.ToString().Length == 0)
            {
                return null;
            }

            foreach (var element in ListDirectories())
            {
                if (element.NameDirectory == nameNewDirectory)
                {
                    return "-1";
                }
            }

            WebClient client = new WebClient();
            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", nameNewDirectory },
                };

            return AnswerIsCreatingDirectory(data, client);
        }

        public static String DeleteDirectory(int id)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            idToServer = id.ToString();

            if (IsExistDirectoryId())
            {
                return "-1";
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryIdWithKey : urlDirectoryIdWithPassword;

            return Delete(url, id);
        }

        public static String MovingFileToDirectory()
        {
            return "-1";
        }

        public static String RenameDirectory(String newNameDirectory)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            if (IsExistDirectoryName(newNameDirectory))
            {
                return "-1";
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryIdWithKey : urlDirectoryIdWithPassword;

            return Rename(url, newNameDirectory);
        }

        /// <summary>
        /// Дополнительные методы для взаимодействий
        /// </summary>
        /// 

        private static String Delete(String url, int id)
        {
            WebClient client = new WebClient();

            try
            {
                var delete = client.UploadValues(url, "DELETE", new NameValueCollection());

                return Encoding.ASCII.GetString(delete);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }  // Общий код удалений

        private static String Rename(String url, String newName)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            WebClient client = new WebClient();

            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", newName },
                };

            try
            {
                var response = client.UploadValues(url, data);

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }  // Общий код переименований

        private static bool IsExistDirectoryId()
        {
            foreach (var element in ListDirectories())
            {
                if (element.Id == idToServer)
                {
                    return true;
                }
            }

            return false;
        }  // Есть ли директория с таким id

        private static bool IsExistDirectoryName(String newNameDirectory)
        {
            foreach (var element in ListDirectories())
            {
                if (element.NameDirectory == newNameDirectory)
                {
                    return true;
                }
            }

            return false;
        }  // Есть ли директория с таким именем

        private static bool IsExistFileId()  // Требуется реализация
        {
            return true;
        }

        private static bool IsExistFileName(String name)  // Требуется реализация
        {
            return true;
        }

        private static List<DirectoryFromServer> ListDirectories()
        {
            UpdateListDirectories();

            return existsDirectories;
        }  // Получение списка директорий 

        private static void UpdateListDirectories()
        {
            var newDirectories = WriteExistingDirectories();

            if (newDirectories != null)
            {
                existsDirectories.Clear();

                foreach (var element in newDirectories)
                {
                    existsDirectories.Add(new DirectoryFromServer
                    {
                        NameDirectory = element.NameDirectory,
                        DateCreate = element.DateCreate,
                        Id = element.Id
                    });
                }
            }
        }  // Обновление списка директорий

        private static List<DirectoryFromServer> WriteExistingDirectories()
        {
            String url = "";

            if (typeAuth.Equals(null))
            {
                return null;
            }

            url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryWithKey : urlDirectoryWithPassword;

            try
            {
                var client = new WebClient();
                var response = client.DownloadString(url);
                var jObject = JObject.Parse(response);

                List<DirectoryFromServer> directoriesFromServer = new List<DirectoryFromServer>();
                foreach (var obj in jObject["objects"])
                {
                    directoriesFromServer.Add(new DirectoryFromServer
                    {
                        NameDirectory = (String)obj["name"],
                        DateCreate = (String)obj["created"],
                        Id = (String)obj["id"]
                    });
                }

                return directoriesFromServer;
            }
            catch (Exception)
            {
                return null;
            }
        }  // Записывание имеющихся директорий на сервере

        private static String AnswerIsCreatingDirectory(NameValueCollection data, WebClient client)
        {
            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryWithKey : urlDirectoryWithPassword;

            try
            {
                var response = client.UploadValues(url, data);

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }  // Ответ сервера на создание директории

        private static String GetHttpStatusCode(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    return response.StatusCode.ToString();
                }
            }

            return ex.Status.ToString();
        }  // Возврат HTTP статуса при обработке Exception в запросе
    }
}