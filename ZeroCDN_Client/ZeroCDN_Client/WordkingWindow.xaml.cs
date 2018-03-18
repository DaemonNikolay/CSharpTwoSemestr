using System;
using System.Collections.Generic;
using System.Linq;
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
        public WordkingWindow()
        {
            InitializeComponent();

            Loaded += TableDirectoriesServer_Loaded;
        }

        private void TableDirectoriesServer_Loaded(object sender, RoutedEventArgs e)
        {
            List<DirectoryFromServer> listDrectoryServer = new List<DirectoryFromServer>
            {
                new DirectoryFromServer { nameDirectory = "Имя первое", dateCreate="Год 2018", directLink="http://zeroCDN.com" },
                new DirectoryFromServer { nameDirectory = "Имя второе", dateCreate="Год 2011", directLink="http://nikulux.ru" },
            };

            MessageBox.Show("" + listDrectoryServer);

            TableDirectoriesServer.ItemsSource = listDrectoryServer;
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
