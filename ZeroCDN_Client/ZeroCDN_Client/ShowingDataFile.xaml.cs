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
using System.Windows.Shapes;

namespace ZeroCDN_Client
{
    /// <summary>
    /// Логика взаимодействия для ShowingDataFile.xaml
    /// </summary>
    public partial class ShowingDataFile : Window
    {
        private String url;

        public ShowingDataFile(String url)
        {
            Url = "http://" + url;

            String[] temp = Url.Split('.');

            if (temp[temp.Length - 1] == "png")
            {
                Loaded += ImageData;
            }
            else
            {
                Loaded += TextData;
            }

            InitializeComponent();
        }

        private void ImageData(object sender, RoutedEventArgs e)
        {
            textBoxDataShow.Visibility = Visibility.Hidden;

            imageDataShow.Source = new BitmapImage(new Uri(Url));
        }

        private void TextData(object sender, RoutedEventArgs e)
        {
            imageDataShow.Visibility = Visibility.Hidden;

            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;

            textBoxDataShow.Text = client.DownloadString(Url);
        }

        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }
    }
}
