﻿using Microsoft.Win32;
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
                //var response = client.UploadValues(urlFileWithKey + "?username=" + UserName + "&api_key=" + PasOrKey,
                //                                   new NameValueCollection());

                //typeAuth = typeAuthorization.LoginAndAPiKey;


                //return Encoding.ASCII.GetString(response);


                var response = client.DownloadString(urlFileWithKey + "?username=" + UserName + "&api_key=" + PasOrKey);

                typeAuth = typeAuthorization.LoginAndAPiKey;

                return response;
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

            return filesInDirectory;
        }

        public async Task<String> RenameFile(String newNameFile, int idCurrentFile)
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

            return await Rename(url, newNameFile);
        }

        public async Task<String> DeleteFile(int id)
        {
            IdToServer = id.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlFileIdWithKey + this.IdToServer + ".json" +
                                                        "?username=" + this.UserName +
                                                        "&api_key=" + this.PasOrKey : urlFileIdWithPassword +
                                                                                      this.IdToServer + ".json";

            return await Delete(url);
        }

        public async Task<String> UploadFile(String directoryId, String pathToFile)
        {
            WebClient client = new WebClient();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlFileWithKey +
                                     "?username=" + this.UserName +
                                     "&api_key=" + this.PasOrKey : urlFileWithPassword;

            return await Task.Run(() =>
            {
                client.Headers[HttpRequestHeader.ContentType] = "";

                var bodyData = new NameValueCollection {
                    { "folder=", $"{directoryId}" }
                };
                client.QueryString = bodyData;

                foreach (var element in client.QueryString.AllKeys)
                {
                    MessageBox.Show(element.ToString() + " " + client.QueryString.Get(element.ToString()));
                }

                var upload = client.UploadFile(url, pathToFile);

                return Encoding.ASCII.GetString(upload);
            });
        }

        /// <summary>
        /// Взаимодействие с директориями
        /// </summary>
        /// 

        internal async Task<List<DirectoryFromServer>> GetDirectories()
        {
            return await GetListDirectories();
        }

        public async Task<String> CreateDirectory(String nameNewDirectory)
        {
            foreach (var element in await GetListDirectories())
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

        public async Task<String> DeleteDirectory(int id)
        {
            this.IdToServer = id.ToString();

            String url = typeAuth == typeAuthorization.LoginAndAPiKey ?
                                     urlDirectoryIdWithKey +
                                     IdToServer + ".json" +
                                     "?username=" + UserName +
                                     "&api_key=" + PasOrKey : urlDirectoryIdWithPassword +
                                                                   IdToServer + ".json";

            return await Delete(url);
        }

        public async Task<String> MovingDirectory(String idDirectoryBegin, String idDirectoryEnd)
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

            await Task.Run(() =>
            {
                try
                {
                    var moving = client.UploadValues(url, "PATCH", data);

                    return Encoding.ASCII.GetString(moving);
                }
                catch (WebException ex)
                {
                    return GetHttpStatusCode(ex);
                }
            });

            return "-1";
        }

        public async Task<String> RenameDirectory(String newNameDirectory, int idCurrentDirectory)
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

            return await Rename(url, newNameDirectory);
        }

        /// <summary>
        /// Дополнительные методы для взаимодействий
        /// </summary>
        /// 

        private async Task<String> Delete(String url)
        {
            WebClient client = new WebClient();

            await Task.Run(() =>
            {
                try
                {
                    var delete = client.UploadValues(url, "DELETE", new NameValueCollection());

                    return Encoding.ASCII.GetString(delete);
                }
                catch (WebException ex)
                {
                    return GetHttpStatusCode(ex);
                }
            });

            return "-1";
        }  // Общий код удалений

        private async Task<String> Rename(String url, String newName)
        {
            WebClient client = new WebClient();

            var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", $"{newName}" },
                };

            await Task.Run(() =>
            {
                try
                {
                    var response = client.UploadValues(url, "PATCH", data);

                    return Encoding.ASCII.GetString(response);
                }
                catch (WebException ex)
                {
                    return GetHttpStatusCode(ex);
                }
            });

            return "-1";
        }  // Общий код переименований

        private async Task<bool> IsExistDirectoryName(String newNameDirectory)
        {
            foreach (var element in await GetListDirectories())
            {
                if (element.NameDirectory == newNameDirectory)
                {
                    return true;
                }
            }

            return false;
        }  // Есть ли директория с таким именем

        private async Task<List<DirectoryFromServer>> GetListDirectories()
        {
            await UpdateListDirectories();

            return existsDirectories;
        }  // Получение списка директорий 

        private async Task UpdateListDirectories()
        {
            await Task.Run(() =>
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
            });
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
                        SizeInMB = element.SizeInMB,
                        DateCreate = element.DateCreate,
                        DirectoryId = element.DirectoryId,
                        Type = element.Type,
                        PublicLink = element.PublicLink,
                        DirectLink = element.DirectLink,
                    });
                }
            }
        }  // Обновление списка файлов в директории

        private String GenerationPublicLink(JToken obj, WebClient client)
        {
            String directoryUrl = (String)obj["folder"];

            if (directoryUrl == null)
            {
                return $"http://{UserName}.cdn.zerocdn.com/{(String)obj["name"]}";
            }

            String nameDirectory = "";

            while (directoryUrl != null)
            {
                String url = "";
                if (typeAuth == typeAuthorization.LoginAndAPiKey)
                {
                    url = $"http://mng.zerocdn.com/api/v2/users/folders/{directoryUrl}.json?username={UserName}&api_key={PasOrKey}";
                }
                else
                {
                    url = $"http://mng.zerocdn.com/api/v2/users/folders/{directoryUrl}.json";
                }

                var responseForDirectoryUrl = client.DownloadString(url);

                var jObject2 = JObject.Parse(responseForDirectoryUrl);

                directoryUrl = (String)jObject2["folder"];
                nameDirectory = '/' + (String)jObject2["name"] + nameDirectory;
            }

            return $"http://{UserName}.cdn.zerocdn.com{nameDirectory}/{(String)obj["name"]}";
        }

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
                    String id = (String)obj["id"];
                    String name = (String)obj["name"];
                    String directoryId = (String)obj["folder"];

                    filesFromDirectory.Add(new FilesFromDirectory
                    {
                        Id = id,
                        Name = name,
                        SizeInMB = Math.Round(Double.Parse((String)obj["size"]) / 1024, 2).ToString(),
                        Type = (String)obj["content_type"],
                        DateCreate = (String)obj["created"],
                        DirectoryId = directoryId,
                        PublicLink = GenerationPublicLink(obj, client),
                        DirectLink = $"zerocdn.com/{id}/{name}"
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