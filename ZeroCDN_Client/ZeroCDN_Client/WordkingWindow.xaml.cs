using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ZeroCDN_Client
{
    /// <summary>
    /// Логика взаимодействия для WordkingWindow.xaml
    /// </summary>

    ///Directory
    public partial class WordkingWindow : Window
    {
        private ApiZeroCDN api;
        private List<DirectoryFromServer> markedItemsDirectory = new List<DirectoryFromServer>();
        private List<FilesFromDirectory> markedItemsFiles = new List<FilesFromDirectory>();

        private DispatcherTimer timer = null;


        public WordkingWindow(ApiZeroCDN api)
        {
            this.api = api;

            InitializeComponent();
            Loaded += TableDirectoriesServer_Loaded;

            TimerIsInternetConnection();
        }

        private void UpdateListFiles()
        {
            var selectItem = TableDirectoriesServer.SelectedItem;
            DirectoryFromServer currentDirectory = (DirectoryFromServer)selectItem;

            TableFilesFromDirectory.IsEnabled = true;
            TableFilesFromDirectory.AutoGenerateColumns = false;

            TableFilesFromDirectory.Columns.Clear();

            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Название",
                Binding = new Binding("Name"),
            });
            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Размер (КБ)",
                Binding = new Binding("SizeInMB"),
            });
            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Дата загрузки",
                Binding = new Binding("DateCreate"),
            });
            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Тип",
                Binding = new Binding("Type"),
            });
            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Публичная ссылка",
                Binding = new Binding("PublicLink"),
            });
            TableFilesFromDirectory.Columns.Add(new DataGridTextColumn
            {
                Header = "Прямая ссылка",
                Binding = new Binding("DirectLink"),
            });

            var listFiles = api.GetFilesInDirectory(currentDirectory.Id);


            TableFilesFromDirectory.ItemsSource = listFiles;
        }

        private void TableDirectoriesServer_Loaded(object sender, RoutedEventArgs e)
        {
            TableDirectoriesServer.IsEnabled = true;
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
                name = windowName.NameDirectory.Text;

                var newDirectory = api.CreateDirectory(name);

                if (newDirectory != "-1")
                {
                    TableDirectoriesServer.ItemsSource = null;
                    TableDirectoriesServer.ItemsSource = api.GetDirectories();

                    return;
                }

                MessageBox.Show("Выбранное имя уже существует!");
            }
        }

        private void DeleteDirecoryToServer_Click(object sender, RoutedEventArgs e)
        {
            foreach (var element in TableDirectoriesServer.SelectedItems)
            {
                markedItemsDirectory.Add((DirectoryFromServer)element);
            }
            foreach (var element in markedItemsDirectory)
            {
                var resultDelete = api.DeleteDirectory(Int32.Parse(element.Id));
            }

            markedItemsDirectory.Clear();

            TableDirectoriesServer.ItemsSource = null;
            TableDirectoriesServer.ItemsSource = api.GetDirectories();
        }

        private void RenameDirectoryToServer_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = TableDirectoriesServer.SelectedItem;
            DirectoryFromServer currentDirectory = (DirectoryFromServer)selectItem;

            if (currentDirectory == null)
            {
                MessageBox.Show("Выберите директорию!");
                return;
            }

            NameForDirectory window = new NameForDirectory();
            if (window.ShowDialog() == true)
            {
                var resultRename = api.RenameDirectory(window.NameDirectory.Text, Convert.ToInt32(currentDirectory.Id));

                TableDirectoriesServer.ItemsSource = null;
                TableDirectoriesServer.ItemsSource = api.GetDirectories();
            }
        }

        private void TableDirectoriesServer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateListFiles();
        }

        private void TableDirectoriesServer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(TableDirectoriesServer, TableDirectoriesServer.Items, DragDropEffects.Move);
        }

        private void TableDirectoriesServer_Drop(object sender, DragEventArgs e)
        {

        }

        private void MovingDirectoryToServer_Click(object sender, RoutedEventArgs e)
        {
            var currentDirectories = api.GetDirectories();

            MovingToDirectory window = new MovingToDirectory(currentDirectories);

            var selectItem = TableDirectoriesServer.SelectedItem;
            DirectoryFromServer selectDirectory = (DirectoryFromServer)selectItem;


            if (window.ShowDialog() == true)
            {
                foreach (var element in currentDirectories)
                {
                    if (element.NameDirectory == window.SelectedDirectoryFromDropDown)
                    {
                        var moving = api.MovingDirectory(element.Id, selectDirectory.Id);

                        break;
                    }
                }



                TableDirectoriesServer.ItemsSource = null;
                TableDirectoriesServer.ItemsSource = api.GetDirectories();
            }

        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            foreach (var element in TableFilesFromDirectory.SelectedItems)
            {
                markedItemsFiles.Add(((FilesFromDirectory)element));
            }
            foreach (var element in markedItemsFiles)
            {
                var resultDelete = api.DeleteFile(Int32.Parse(element.Id));
            }

            markedItemsFiles.Clear();
            UpdateListFiles();
        }

        private void RenameFile_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = TableFilesFromDirectory.SelectedItem;
            FilesFromDirectory currentFile = (FilesFromDirectory)selectItem;

            if (currentFile == null)
            {
                MessageBox.Show("Выберите файл!");
                return;
            }

            NameForDirectory window = new NameForDirectory();
            if (window.ShowDialog() == true)
            {
                var resultRename = api.RenameFile(window.NameDirectory.Text, Convert.ToInt32(currentFile.Id));

                UpdateListFiles();
            }
        }

        private void ShowingFile_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = TableFilesFromDirectory.SelectedItem;
            FilesFromDirectory currentFile = (FilesFromDirectory)selectItem;

            ShowingDataFile window = new ShowingDataFile(currentFile.DirectLink);

            window.Show();
        }

        private void TableFilesFromDirectory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowingFile_Click(sender, e);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            String themeMessageBoxText = "Уверены, что желаете покинуть нас?";
            String titleMessageBox = "...жили долго и счастливо";

            MessageBoxButton buttonMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage iconMessageBox = MessageBoxImage.Warning;

            MessageBoxResult resultMessageBox = MessageBox.Show(themeMessageBoxText, titleMessageBox, buttonMessageBox, iconMessageBox);

            if (resultMessageBox == MessageBoxResult.Yes)
            {
                this.Visibility = Visibility.Collapsed;

                MainWindow wind = new MainWindow();
                wind.Closed += (sender2, e2) =>
                {
                    this.Close();
                };

                wind.ShowDialog();
            }
        }


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
                InternetConnection.Source = new BitmapImage(new Uri("Image/InternetConnection.png", UriKind.Relative));
            }
            else
            {
                InternetConnection.Source = new BitmapImage(new Uri("Image/NoInternetConnection.png", UriKind.Relative));
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

        private async void UploadToServer_Click(object sender, RoutedEventArgs e)
        {
            DirectoryFromServer selectDirectory = (DirectoryFromServer)TableDirectoriesServer.SelectedItem;
            if (selectDirectory == null)
            {
                MessageBox.Show("Выберите одну директорию");

                return;
            }

            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                String pathToFile = dialog.FileName;

                var upload = await api.UploadFile(selectDirectory.Id, pathToFile);

                MessageBox.Show(upload);
            }
        }




        ///


    }
}
