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
using Multi_Tool.Tools.BKV;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for BKV_Reader.xaml
    /// </summary>
    public partial class BKV_Reader : Window
    {
        BKVReader bkvReader;
        public BKV_Reader()
        {
            InitializeComponent();
            bkvReader = new BKVReader(OutputList);
        }

        private void TestRip_Button_Click(object sender, RoutedEventArgs e)
        {
            bkvReader.StartRun(false);
        }

        private void SelectFile_Button_Click(object sender, RoutedEventArgs e)
        {
            bkvReader.GetFile();
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        private void StartBKV_Button_Click(object sender, RoutedEventArgs e)
        {
            bkvReader.StartRun(true);
        }

        private void ExportDir_Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
