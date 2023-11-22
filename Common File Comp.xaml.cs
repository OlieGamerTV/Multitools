using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using Multi_Tool.Enumerators;
using Multi_Tool.Tools;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for Common_File_Comp.xaml
    /// </summary>
    public partial class Common_File_Comp : Window
    {
        CompressionTool fileRipper = null;
        string currentFileIndex = "AUTOMATIC";
        int currentFileCompression = 6;

        public Common_File_Comp()
        {
            InitializeComponent();
            if (fileRipper == null)
            {
                fileRipper = new CompressionTool(Output_List);
            }
            currentFileIndex = AlgorithmEnums.AUTOMATIC;
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        private void StartRip_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            fileRipper.StartZlibFile(false);
        }

        private void fileFormatCombo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AlgorithmEnums.fileIndexes[compressionCombo.SelectedIndex] == currentFileIndex)
            {
            }
            else
            {
                currentFileIndex = AlgorithmEnums.fileIndexes[compressionCombo.SelectedIndex];
                fileRipper.SetAlgorithm(currentFileIndex);
            }
        }

        private void SelectFile_Button_Click(object sender, RoutedEventArgs e)
        {
            fileRipper.GetDirs(true);
        }

        private void ExportDir_Button_Click(object sender, RoutedEventArgs e)
        {
            fileRipper.GetDirs(false);
        }

        private void FileDecomp_Button_Click(object sender, RoutedEventArgs e)
        {
            fileRipper.StartZlibFile(true);
        }

        private void compressionLevel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (currentFileCompression == compressionLevel.SelectedIndex)
            {
            }
            else
            {
                currentFileCompression = compressionLevel.SelectedIndex;
                fileRipper.SetCompLevel(currentFileCompression);
            }
        }

        private void XOR_Key_A_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fileRipper != null)
            {
                fileRipper.xorKeyA = XOR_Key_A.Text;
            }
        }

        private void XOR_Key_B_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fileRipper != null)
            {
                fileRipper.xorKeyB = XOR_Key_B.Text;
            }
        }
    }
}
