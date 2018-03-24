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
    /// Логика взаимодействия для NameForDirectory.xaml
    /// </summary>
    public partial class NameForDirectory : Window
    {
        private String name;
        public NameForDirectory()
        {
            InitializeComponent();
        }

        private void OkNameDirectory_Click(object sender, RoutedEventArgs e)
        {
            name = NameDirectory.Text;
            DialogResult = true;
        }

        public new String Name()
        {
            return name;
        }

        private void CancelNameDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
