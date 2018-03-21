using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroCDN_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendDataAuth_Click(object sender, RoutedEventArgs e)
        {

            //{
            //    this.Visibility = System.Windows.Visibility.Collapsed;

            //    WordkingWindow wind = new WordkingWindow();
            //    wind.Closed += (sender2, e2) =>
            //    {
            //        this.Close();
            //    };

            //    wind.ShowDialog();
            //}

            String login = InputAuthLogin.Text;
            String password = InputAuthPassword.Text;

            HttpWebRequest query = (HttpWebRequest)WebRequest.Create("http://mng.zerocdn.com/api/v2/users/files.json?username=" + login + "&api_key=" + password);
            query.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)query.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.Visibility = Visibility.Collapsed;

                    WordkingWindow wind = new WordkingWindow(login, password);
                    wind.Closed += (sender2, e2) =>
                    {
                        this.Close();
                    };

                    wind.ShowDialog();
                }

                response.Close();
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The following Exception was raised : {ex.Message}");
            }
        }
    }
}
