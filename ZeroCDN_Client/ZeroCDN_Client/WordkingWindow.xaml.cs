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
        ApiZeroCDN api;

        public WordkingWindow(ApiZeroCDN api)
        {
            this.api = api;

            InitializeComponent();
            Loaded += TableDirectoriesServer_Loaded;
        }

        private void TableDirectoriesServer_Loaded(object sender, RoutedEventArgs e)
        {
            TableDirectoriesServer.AutoGenerateColumns = false;
            TableDirectoriesServer.Columns.Add(new DataGridTextColumn
            {
                Header = "Название",
                Binding = new Binding("NameDirectory"),
            });
            TableDirectoriesServer.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата создания",
                Binding = new Binding("DateCreate"),
            });

            var listDirectories = api.GetDirectories();

            TableDirectoriesServer.ItemsSource = listDirectories;
        }

        private void TableDirectoriesServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CreateDirecoryToServer_Click(object sender, RoutedEventArgs e)
        {
            NameForDirectory windowName = new NameForDirectory();

            if (windowName.ShowDialog() == true)
            {
                var name = windowName.NameDirectory.Text;

                while (true)
                {
                    name = windowName.NameDirectory.Text;
                    var newDirectory = api.CreateDirectory(name);

                    if (newDirectory == "-1")
                    {
                        MessageBox.Show("Такое имя уже существует!");

                        continue;
                    }

                    break;
                }

                TableDirectoriesServer.ItemsSource = null;
                TableDirectoriesServer.ItemsSource = api.GetDirectories(); 
            }
        }
    }
}
