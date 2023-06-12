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

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for File_List.xaml
    /// </summary>
    public partial class File_List : Window
    {
        public File_List()
        {
            InitializeComponent();
        }

        public void FillList(string fileName, params string[] items)
        {
            File_Tree.Items.Clear();
            TreeViewItem amount = new TreeViewItem();
            TreeViewItem root = new TreeViewItem();
            amount.Header = "Total files extracted: " + items.Length;
            File_Tree.Items.Add(amount);
            root.Header = fileName;
            foreach(string item in items)
            {
                MenuItem file = new MenuItem();
                file.Header = item;
                file.CommandParameter = item;
                root.Items.Add(file);
            }
            File_Tree.Items.Add(root);
        }

        private void OpenExplorer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
