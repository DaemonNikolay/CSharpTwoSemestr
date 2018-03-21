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
        private static String userName;
        private static String pasOrKey;
        private static String urlFile = "http://mng.zerocdn.com/api/v2/users/files.json";
        private static String urlkDirectory = "http://mng.zerocdn.com/api/v2/users/folders.json";
        private static String postfixUsername = "?username=";
        private static String postfixApiKey = "&api_key=";
        private static bool isAithorised = false;
        private static typeAuthorization typeAuth;
        private static List<String> existsDirectories = new List<string>();

        public static String AuthLoginPassword(String username, String password)
        {
            userName = username;
            pasOrKey = password;
            typeAuth = typeAuthorization.LoginAndPassword;

            WebClient client = new WebClient();

            client.Credentials = new NetworkCredential(userName, pasOrKey);

            String credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + pasOrKey));
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            try
            {
                StreamReader reader = new StreamReader(client.DownloadString(urlFile));
                isAithorised = true;

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
            typeAuth = typeAuthorization.LoginAndAPiKey;

            WebClient client = new WebClient();

            try
            {
                var respone = client.DownloadString(urlFile + postfixUsername + userName + postfixApiKey + pasOrKey);
                StreamReader reader = new StreamReader(respone);

                isAithorised = true;

                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }

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

        public static String CreateDirectory(String nameNewDirectory)
        {
            if (nameNewDirectory.Length == 0 || isAithorised == false)
            {
                return null;
            }


            WebClient client = new WebClient();

            var data = new NameValueCollection
            {
                { "Content-Type", "application/json" },
                { "name", nameNewDirectory },
            };

            try
            {
                var response = client.UploadValues(urlFile + "?username=" + username + "&api_key=" + key, data);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }

            return "-1";
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

        enum typeAuthorization
        {
            LoginAndAPiKey,
            LoginAndPassword
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

        private static void WriteExistingDirectories()
        {
            try
            {
                var client = new WebClient();
                var text = client.DownloadString("http://example.com/page.html");
            }
            catch ()
            {

            }



            HttpWebRequest query = (HttpWebRequest)WebRequest.Create("http://mng.zerocdn.com/api/v2/users/folders.json?username=" + login + "&api_key=" + password);
            query.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)query.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                    var jObject = JObject.Parse(reader.ReadToEnd());

                    List<DirectoryFromServer> listDirectoryServer = new List<DirectoryFromServer>();
                    foreach (var obj in jObject["objects"])
                    {
                        listDirectoryServer.Add(new DirectoryFromServer { NameDirectory = (String)obj["name"], DateCreate = (String)obj["created"], DirectLink = "http://tyr-tyr.com" });
                    }
                }

                response.Close();


                existsDirectories.Add();
            }
    }
    }
