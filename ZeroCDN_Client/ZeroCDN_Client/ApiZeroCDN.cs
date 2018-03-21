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

        private static String urlFile = "http://mng.zerocdn.com/api/v2/users/files.json";
        private static String urlDirectory = "http://mng.zerocdn.com/api/v2/users/folders.json";
        private static String urlFileWithKey = urlFile + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlDirectoryWithKey = urlDirectory + "?username=" + userName + "&api_key=" + pasOrKey;
        private static String urlFileWithPassword = urlFile;
        private static String urlDirectoryWithPassword = urlDirectory;

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
            return "-1";
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

            WebClient client = new WebClient();

            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", nameNewDirectory },
                };

            if (typeAuth == typeAuthorization.LoginAndAPiKey)
            {
                return AnswerIsCreatingDirectory(data, client);
            }

            if (typeAuth == typeAuthorization.LoginAndPassword)
            {
                return AnswerIsCreatingDirectory(data, client);
            }

            return "";
        }

        public static String DeleteDirectory()
        {
            return "-1";
        }

        public static String MovingFileToDirectory()
        {
            return "-1";
        }

        public static String LoadFileToDirectory()
        {
            return "-1";
        }

        /// <summary>
        /// Дополнительные методы для взаимодействий
        /// </summary>

        public static void RenewedListDirectories()
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
        }

        private static String AnswerIsCreatingDirectory(NameValueCollection data, WebClient client)
        {
            try
            {
                var response = client.UploadValues(urlDirectoryWithPassword, data);

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }

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
        }

        private static List<DirectoryFromServer> WriteExistingDirectories()
        {
            String url = "";

            if (typeAuth.Equals(null))
            {
                return null;
            }

            url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryWithKey : urlFileWithPassword;

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
        }
    }
}