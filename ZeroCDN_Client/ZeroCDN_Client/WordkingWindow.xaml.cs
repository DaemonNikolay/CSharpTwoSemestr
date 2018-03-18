using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZeroCDN_Client
{
    /// <summary>
    /// Логика взаимодействия для WordkingWindow.xaml
    /// </summary>
    public partial class WordkingWindow : Window
    {
        private String login;
        private String password;

        public WordkingWindow(String login, String password)
        {
            this.login = login;
            this.password = password;

            InitializeComponent();
            Loaded += TableDirectoriesServer_Loaded;
        }

        private void TableDirectoriesServer_Loaded(object sender, RoutedEventArgs e)
        {
            HttpWebRequest query = (HttpWebRequest)WebRequest.Create("http://mng.zerocdn.com/api/v2/users/folders.json?username=" + login + "&api_key=" + password);
            query.AllowAutoRedirect = false;
            //try
            //{
            HttpWebResponse response = (HttpWebResponse)query.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                var jObject = JObject.Parse(reader.ReadToEnd());
                //MessageBox.Show("" + jObject);

                List<DirectoryFromServer> listDirectoryServer = new List<DirectoryFromServer>();
                //listDirectoryServer.Add(new DirectoryFromServer { NameDirectory = "Первая", DateCreate = "2018", DirectLink = "https://fdsfds.com" });

                foreach (var obj in jObject["objects"])
                {
                    listDirectoryServer.Add(new DirectoryFromServer { NameDirectory = (String)obj["name"], DateCreate = (String)obj["created"], DirectLink = "http://tyr-tyr.com" });
                }
                //var names = jObject["objects"];

                //MessageBox.Show("" + names);
                TableDirectoriesServer.ItemsSource = listDirectoryServer;
            }

            response.Close();
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"The following Exception was raised : {ex.Message}");
            //}
        }

        private void TableDirectoriesServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //List<DirectoryFromServer> listDrectoryServer = new List<DirectoryFromServer>
            //{
            //    new DirectoryFromServer { nameDirectory = "Имя первое", dateCreate="Год 2018", directLink="http://zeroCDN.com" },
            //    new DirectoryFromServer { nameDirectory = "Имя первое", dateCreate="Год 2018", directLink="http://zeroCDN.com" },
            //};

            //MessageBox.Show("" + listDrectoryServer);

            //TableDirectoriesServer.ItemsSource = listDrectoryServer;
        }


    }
}
