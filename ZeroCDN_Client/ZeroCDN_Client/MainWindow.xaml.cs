using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
using System.Windows.Threading;

namespace ZeroCDN_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiZeroCDN api = new ApiZeroCDN();
        private DispatcherTimer timer = null;

        public MainWindow()
        {
            InitializeComponent();

            TimerIsInternetConnection();
        }

        private async void SendDataAuth_Click(object sender, RoutedEventArgs e)
        {
            String login = InputAuthLogin.Text;
            String password = InputAuthPassword.Text;

            var auth = api.AuthLoginKey(login, password);
            //var auth = api.AuthLoginPassword(login, password);

            if (await ConnectionAvailable() == true)
            {

                if (auth == "429")
                {
                    MessageBox.Show("Жди пять минут, слишком много некорректных запросов. \nКод 429.");
                }
                else if (auth == HttpStatusCode.Forbidden.ToString())
                {
                    MessageBox.Show("Учётные данные пользователя не верны!");
                }
                else if (auth.Contains("{\"meta\":{\"previous\":null,\"next\":null,\"limit\":100,\"offset\":0}"))
                {
                    this.Visibility = Visibility.Collapsed;

                    WordkingWindow wind = new WordkingWindow(api);
                    wind.Closed += (sender2, e2) =>
                    {
                        this.Close();
                    };

                    wind.ShowDialog();
                }
                else
                {
                    MessageBox.Show($"Error, code {auth}");
                }
            }
            else
            {
                MessageBox.Show("Проверьте подключение к сети интернет");
            }
        }


        /// <summary>
        /// Is internet connection
        /// </summary>

        private void TimerIsInternetConnection()
        {
            timer = new DispatcherTimer();

            timer.Tick += new EventHandler(CorrectImageSource);
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);

            timer.Start();
        }

        private async void CorrectImageSource(object sender, EventArgs e)
        {
            if (await ConnectionAvailable() == true)
            {
                IsInternetConnection.Source = new BitmapImage(new Uri("Image/InternetConnection.png", UriKind.Relative));
            }
            else
            {
                IsInternetConnection.Source = new BitmapImage(new Uri("Image/NoInternetConnection.png", UriKind.Relative));
            }
        }

        private async Task<Boolean> ConnectionAvailable()
        {
            Ping ping = new Ping();
            PingReply pingReply = null;

            return await Task.Run(() =>
            {
                try
                {
                    pingReply = ping.Send("8.8.8.8");

                    return pingReply.Status == IPStatus.Success ? true : false;
                }
                catch (PingException)
                {
                    return false;
                }
            });
        }
    }
}
