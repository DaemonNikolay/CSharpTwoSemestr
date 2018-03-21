using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client
{
    class ApiZeroCDN
    {
        private static String urlFile = "http://mng.zerocdn.com/api/v2/users/files.json";
        private static String urlkDirectory = "http://mng.zerocdn.com/api/v2/users/folders.json";

        public static String AuthLoginPassword(String username, String password)
        {
            WebClient client = new WebClient();

            client.Credentials = new System.Net.NetworkCredential(username, password);

            String credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

            var result = client.DownloadString(urlFile);

            return "-1";
        }

        public static String AuthLoginKey()
        {
            return "-1";
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

        public static String CreateDirectory(String username, String key) //сделать правильно, сейчас черновик
        {
            using (var client = new WebClientEx())
            {
                var data = new NameValueCollection
                {
                    { "Content-Type", "application/json" },
                    { "name", "newMyFolder" },
                };
               
                var response2 = client.UploadValues("http://mng.zerocdn.com/api/v2/users/folders.json?username=" + username + "&api_key=" + key, data);
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


    }
}
