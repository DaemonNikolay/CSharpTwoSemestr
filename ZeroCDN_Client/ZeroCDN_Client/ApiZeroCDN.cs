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
        private static String id;

        private static String url = "http://mng.zerocdn.com/api/v2/users/";

        private static String postfixUsers = "users.json";
        private static String postfixDirectories = "folders.json";

        private static String urlDirectoryIdWithPassword = url + postfixDirectories + id + ".json" + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlDirectoryIdWithKey = url + postfixDirectories + id + ".json";

        private static String urlFileIdWithKey = url + "files/" + id + ".json" + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlFileIdWithPassword = url + "files/" + id + ".json";

        private static String urlFileWithKey = url + postfixUsers + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlDirectoryWithKey = url + postfixDirectories + "?username=" + userName + "&api_key=" + pasOrKey;

        private static String urlFileWithPassword = url + postfixDirectories;
        private static String urlDirectoryWithPassword = url + postfixUsers;

        private static typeAuthorization typeAuth;
        private enum typeAuthorization
        {
            LoginAndAPiKey,
            LoginAndPassword
        }

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

        public static String AddFiles()
        {
            return "-1";
        }

        public static String GettingDataFile()
        {
            return "-1";
        }

        public static String DeleteFiles()
        {
            if (typeAuth.Equals(null) || id == null)
            {
                return null;
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileIdWithKey : urlFileIdWithPassword;
            WebClient client = new WebClient();

            try
            {
                var deleteFile = client.UploadValues(url, "DELETE", new NameValueCollection());

                return Encoding.ASCII.GetString(deleteFile);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
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
                    return "Directory found";
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

        public static String DeleteDirectory()
        {
            if (typeAuth.Equals(null) || id == null)
            {
                return null;
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryIdWithKey : urlDirectoryIdWithPassword;
            WebClient client = new WebClient();

            var deleteDirectory = client.UploadString(url, "DELETE", "");

            return deleteDirectory;
        }

        public static String MovingFileToDirectory()
        {
            return "-1";
        }

        public static String RenameDirectory(String newNameDirectory)
        {
            if (typeAuth.Equals(null) || id == null)
            {
                return null;
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryIdWithKey : urlDirectoryIdWithPassword;
            WebClient client = new WebClient();

            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", newNameDirectory },
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
        }

        public static String LoadFileToDirectory()
        {
            return "-1";
        }

        /// <summary>
        /// Дополнительные методы для взаимодействий
        /// </summary>

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
                        DateCreate = element.DateCreate
                    });
                }
            }
        }  // Обновление списка директорий

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
                    directoriesFromServer.Add(new DirectoryFromServer { NameDirectory = (String)obj["name"], DateCreate = (String)obj["created"] });
                }

                return directoriesFromServer;
            }
            catch (Exception)
            {
                return null;
            }
        }  // Записывание имеющихся директорий на сервере
    }
}