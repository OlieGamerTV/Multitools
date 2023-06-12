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
using Multi_Tool.Tools;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for File_Decompiler.xaml
    /// </summary>
    public partial class File_Decompiler : Window
    {
        EmbeddedFileRip fileRipper;
        string currentFileIndex = "PNG";

        public File_Decompiler()
        {
            InitializeComponent();
            currentFileIndex = fileFormatCombo.Text.ToUpper();
            fileRipper = new EmbeddedFileRip(Output_List);
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        private async void StartRip_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            await fileRipper.RipFile();
        }

        private void fileFormatCombo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (fileFormatCombo.Text.ToUpper() == currentFileIndex)
            {

            }
            else
            {
                string[] temp = fileFormatCombo.Text.ToUpper().Split(' ');
                fileRipper.UpdateFileSeek(temp[0]);
                currentFileIndex = fileFormatCombo.Text.ToUpper();
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
    }
}
