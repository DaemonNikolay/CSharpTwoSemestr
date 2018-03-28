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
    /// Логика взаимодействия для MovingToDirectory.xaml
    /// </summary>
    public partial class MovingToDirectory : Window
    {
        private String selectedItem;
        private List<DirectoryFromServer> items;

        public string SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                selectedItem = value;
            }
        }

        internal MovingToDirectory(List<DirectoryFromServer> items)
        {
            InitializeComponent();

            this.items = items;

            Loaded += MovingToDirectory_Loaded;
        }

        private void MovingToDirectory_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxListDirectories.Items.Clear();

            foreach (var element in this.items)
            {
                comboBoxListDirectories.Items.Add(element.NameDirectory);
            }
        }

        private void OkNameDirectory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = comboBoxListDirectories.SelectedValue;

            //DialogResult = true;

            this.SelectedItem = "" + selectedItem;
            
            MessageBox.Show("" + this.SelectedItem);  //выводит выделенный элемент
        }

        private void CancelNameDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
