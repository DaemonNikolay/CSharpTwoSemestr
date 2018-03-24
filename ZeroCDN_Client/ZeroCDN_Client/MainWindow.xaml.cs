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
        ApiZeroCDN api = new ApiZeroCDN();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendDataAuth_Click(object sender, RoutedEventArgs e)
        {
            String login = InputAuthLogin.Text;
            String password = InputAuthPassword.Text;

            var auth = api.AuthLoginKey(login, password);

            if (auth != "200")
            {
                this.Visibility = Visibility.Collapsed;

                WordkingWindow wind = new WordkingWindow(api);
                wind.Closed += (sender2, e2) =>
                {
                    this.Close();
                };

                wind.ShowDialog();
            }
        }
    }
}
