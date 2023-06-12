using System;
using System.Collections.Generic;
using System.IO;
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
using Multi_Tool.Tools;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for DPack_Compression.xaml
    /// </summary>
    public partial class DPack_Compression : Window
    {
        DPack extractor;

        public DPack_Compression()
        {
            InitializeComponent();
            extractor = new DPack(Output_List);
        }

        private void SelectFile_Button_Click(object sender, RoutedEventArgs e)
        {
            extractor.GetDir(true);
        }

        private void ExportDir_Button_Click(object sender, RoutedEventArgs e)
        {
            extractor.GetDir(false);
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        private void StartDPak_Button_Click(object sender, RoutedEventArgs e)
        {
            extractor.StartTool(Output_List, true);
            File_List fileWindow = new File_List();
            fileWindow.Owner = this;
            string[] files = Directory.GetFiles(extractor.GetCurrentExportDir);
            fileWindow.FillList(extractor.GetCurrentFileName, files);
            fileWindow.Show();
        }

        private void StartDPakCompress_Button_Click(object sender, RoutedEventArgs e)
        {
            extractor.StartTool(Output_List, false);
        }
    }
}
