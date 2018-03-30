using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZeroCDN_Client
{
    public class ApiZeroCDN
    {
        /// <summary>
        /// Поля
        /// </summary>

        private String userName;
        private String pasOrKey;

        private String idToServer;

        private const String baseUrl = "http://mng.zerocdn.com/api/v2/users/";

        private const String postfixFiles = "files.json";
        private const String postfixDirectories = "folders.json";

        private String urlDirectoryIdWithPassword = "http://zerocdn.com/api/v2/users/" + "folders/";
        private String urlDirectoryIdWithKey = "http://zerocdn.com/api/v2/users/" + "folders/";

        private String urlFileIdWithKey = baseUrl + "files/";
        private String urlFileIdWithPassword = baseUrl + "files/";

        private String urlFileWithKey = baseUrl + postfixFiles;
        private String urlDirectoryWithKey = baseUrl + postfixDirectories;

        private String urlFileWithPassword = baseUrl + postfixFiles;
        private String urlDirectoryWithPassword = baseUrl + postfixDirectories;

        private enum typeAuthorization
        {
            LoginAndAPiKey,
            LoginAndPassword
        }
        private typeAuthorization typeAuth;

        private List<DirectoryFromServer> existsDirectories = new List<DirectoryFromServer>();
        private List<FilesFromDirectory> existsFiles = new List<FilesFromDirectory>();

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        public string PasOrKey
        {
            get
            {
                return pasOrKey;
            }

            set
            {
                pasOrKey = value;
            }
        }

        public string IdToServer
        {
            get
            {
                return idToServer;
            }

            set
            {
                idToServer = value;
            }
        }

        /// <summary>
        /// Авторизация
        /// </summary>

        public String AuthLoginPassword(String username, String password)
        {
            UserName = username;
            PasOrKey = password;

            WebClient client = new WebClient();

            client.Credentials = new NetworkCredential(UserName, PasOrKey);

            String credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(UserName + ":" + PasOrKey));
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            try
            {
                var response = client.DownloadString(urlFileWithPassword);

                typeAuth = typeAuthorization.LoginAndPassword;

                return response;
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

        public String AuthLoginKey(String username, String apiKey)
        {
            this.UserName = username;
            this.PasOrKey = apiKey;

            WebClient client = new WebClient();

            try
            {
                var response = client.UploadValues(urlFileWithKey + "?username=" + UserName + "&api_key=" + PasOrKey,
                                                   new NameValueCollection());

                typeAuth = typeAuthorization.LoginAndAPiKey;

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }

        /// <summary>
        /// Взаимодействие с файлами
        /// </summary>

        internal List<FilesFromDirectory> GetFilesInDirectory(String idDirectory)
        {
            List<FilesFromDirectory> filesInDirectory = new List<FilesFromDirectory>();

            foreach (var element in GetListFiles())
            {
                if (element.DirectoryId == idDirectory)
                {
                    filesInDirectory.Add(element);
                }
            }

            MessageBox.Show("" + filesInDirectory);

            return filesInDirectory;
        }

        public String LoadFileToDirectory(int idDirectory, String pathToFile)
        {
            if (pathToFile.Length == 0)
            {
                return null;
            }

            IdToServer = idDirectory.ToString();
            //if (IsExistDirectoryId())
            //{
            //    return "-1";
            //}

            WebClient client = new WebClient();

            this.IdToServer = idDirectory.ToString();

            NameValueCollection data = new NameValueCollection
            {
                { "Content-Type", "application/json" },
                { "file", "@" + pathToFile },
                { "folder", IdToServer.ToString() }
            };

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileWithKey +
                                                                        "?username=" + UserName +
                                                                        "&api_key=" + PasOrKey : urlFileWithPassword;

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

        public String LoadFileToDirectoryOnLink()
        {
            return "-1";
        }  // ТРЕБУЕТСЯ РЕАЛИЗАЦИЯ

        public String RenameFile(String newNameFile, int idCurrentFile)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            this.IdToServer = idCurrentFile.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlFileIdWithKey + this.IdToServer + ".json" +
                                                        "?username=" + UserName +
                                                        "&api_key=" + PasOrKey : urlFileIdWithPassword +
                                                                                 this.IdToServer + ".json"; ;

            return Rename(url, newNameFile);
        }

        public String DeleteFile(int id)
        {
            IdToServer = id.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlFileIdWithKey + this.IdToServer + ".json" +
                                                        "?username=" + this.UserName +
                                                        "&api_key=" + this.PasOrKey : urlFileIdWithPassword +
                                                                                      this.IdToServer + ".json";

            return Delete(url);
        }

        /// <summary>
        /// Взаимодействие с директориями
        /// </summary>
        /// 

        internal List<DirectoryFromServer> GetDirectories()
        {
            return GetListDirectories();
        }

        public String CreateDirectory(String nameNewDirectory)
        {
            foreach (var element in GetListDirectories())
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

        public String DeleteDirectory(int id)
        {
            this.IdToServer = id.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlDirectoryIdWithKey +
                                     IdToServer + ".json" +
                                     "?username=" + UserName +
                                     "&api_key=" + PasOrKey : urlDirectoryIdWithPassword +
                                                                   IdToServer + ".json";

            return Delete(url);
        }

        public String MovingDirectory(String idDirectoryBegin, String idDirectoryEnd)
        {
            WebClient client = new WebClient();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlDirectoryIdWithKey +
                                     idDirectoryEnd + ".json" +
                                     "?username=" + this.UserName +
                                     "&api_key=" + this.PasOrKey : urlDirectoryIdWithPassword +
                                                                   this.IdToServer + ".json";


            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "folder", idDirectoryBegin },
                };

            try
            {
                foreach (var element in data.AllKeys)
                {
                    MessageBox.Show(element + " : " + data.Get(element));
                }

                MessageBox.Show("URL: " + url);

                var moving = client.UploadValues(url, "PATCH", data);

                MessageBox.Show("mOVING: " + moving);

                return Encoding.ASCII.GetString(moving);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }  // ТРЕБУЕТСЯ РЕАЛИЗАЦИЯ

        public String RenameDirectory(String newNameDirectory, int idCurrentDirectory)
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            this.IdToServer = idCurrentDirectory.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlDirectoryIdWithKey + IdToServer + ".json" +
                                                             "?username=" + UserName +
                                                             "&api_key=" + PasOrKey : urlDirectoryIdWithPassword +
                                                                                      IdToServer + ".json";
            MessageBox.Show("url: " + url);

            return Rename(url, newNameDirectory);
        }

        /// <summary>
        /// Дополнительные методы для взаимодействий
        /// </summary>
        /// 

        private String Delete(String url)
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

        private String Rename(String url, String newName)
        {
            WebClient client = new WebClient();

            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", $"{newName}" },
                };

            try
            {
                var response = client.UploadValues(url, "PATCH", data);

                return Encoding.ASCII.GetString(response);
            }
            catch (WebException ex)
            {
                return GetHttpStatusCode(ex);
            }
        }  // Общий код переименований

        private bool IsExistDirectoryName(String newNameDirectory)
        {
            foreach (var element in GetListDirectories())
            {
                if (element.NameDirectory == newNameDirectory)
                {
                    return true;
                }
            }

            return false;
        }  // Есть ли директория с таким именем

        private List<DirectoryFromServer> GetListDirectories()
        {
            UpdateListDirectories();

            return existsDirectories;
        }  // Получение списка директорий 

        private void UpdateListDirectories()
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

        private List<DirectoryFromServer> WriteExistingDirectories()
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryWithKey +
                                                                        "?username=" + this.UserName +
                                                                        "&api_key=" + this.PasOrKey : urlDirectoryWithPassword;

            //MessageBox.Show("WriteExistingDirectories\n" + (typeAuth == typeAuthorization.LoginAndPassword));

            //try
            //{
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
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }  // Записывание имеющихся директорий на сервере

        private List<FilesFromDirectory> GetListFiles()
        {
            UpdateListFiles();

            return existsFiles;
        }  // Получение списка файлов в директории

        private void UpdateListFiles()
        {
            var newListFiles = WriteExistingFiles();

            if (newListFiles != null)
            {
                existsFiles.Clear();

                foreach (var element in newListFiles)
                {
                    existsFiles.Add(new FilesFromDirectory
                    {
                        Id = element.Id,
                        Name = element.Name,
                        SizeInMB = Math.Round(Double.Parse(element.SizeInMB) / 1024, 2).ToString(),
                        DateCreate = element.DateCreate,
                        DirectoryId = element.DirectoryId,
                        Type = element.Type,
                        PublicLink = $"{UserName}.zerocdn.com/",
                        DirectLink = $"zerocdn.com/{element.Id}/{element.Name}"
                    });
                }
            }
        }  // Обновление списка файлов в директории

        private List<FilesFromDirectory> WriteExistingFiles()
        {
            if (typeAuth.Equals(null))
            {
                return null;
            }

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlFileWithKey +
                                                                        "?username=" + UserName +
                                                                        "&api_key=" + PasOrKey : urlFileWithPassword;

            try
            {
                var client = new WebClient();
                var response = client.DownloadString(url);
                var jObject = JObject.Parse(response);

                List<FilesFromDirectory> filesFromDirectory = new List<FilesFromDirectory>();
                foreach (var obj in jObject["objects"])
                {
                    filesFromDirectory.Add(new FilesFromDirectory
                    {
                        Id = (String)obj["id"],
                        Name = (String)obj["name"],
                        SizeInMB = (String)obj["size"],
                        Type = (String)obj["content_type"],
                        DateCreate = (String)obj["created"],
                        DirectoryId = (String)obj["folder"]
                    });
                }

                return filesFromDirectory;
            }
            catch (Exception)
            {
                return null;
            }
        }  // Запись нового списка файлов в директории

        private String AnswerIsCreatingDirectory(NameValueCollection data, WebClient client)
        {
            String url = typeAuth == typeAuthorization.LoginAndAPiKey ? urlDirectoryWithKey +
                                                                        "?username=" + UserName +
                                                                        "&api_key=" + PasOrKey : urlDirectoryWithPassword;

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

        private String GetHttpStatusCode(WebException ex)
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