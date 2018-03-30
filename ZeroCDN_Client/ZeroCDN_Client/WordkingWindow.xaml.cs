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

    ///Directory
    public partial class WordkingWindow : Window
    {
        private ApiZeroCDN api;
        private List<DirectoryFromServer> markedItemsDirectory = new List<DirectoryFromServer>();
        private List<FilesFromDirectory> markedItemsFiles = new List<FilesFromDirectory>();

        private DirectoryFromServer selectedDirectory;

        private DirectoryFromServer SelectedDirectory
        {
            get
            {
                return selectedDirectory;
            }

            set
            {
                selectedDirectory = value;
            }
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

            var listFiles = api.GetFilesInDirectory(currentDirectory.Id);


            TableFilesFromDirectory.ItemsSource = listFiles;
        }

        public WordkingWindow(ApiZeroCDN api)
        {
            this.api = api;

            InitializeComponent();
            Loaded += TableDirectoriesServer_Loaded;
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
            FilesFromDirectory currentDirectory = (FilesFromDirectory)selectItem;

            if (currentDirectory == null)
            {
                MessageBox.Show("Выберите файл!");
                return;
            }

            NameForDirectory window = new NameForDirectory();
            if (window.ShowDialog() == true)
            {
                var resultRename = api.RenameFile(window.NameDirectory.Text, Convert.ToInt32(currentDirectory.Id));

                UpdateListFiles();
            }
        }
    }
}
