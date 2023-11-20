using Alpine.Renderables.Models.Skeleton.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Utilities.Flash;
using System.IO;
using Genesis3D.Geometry;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for Genesis3D.xaml
    /// </summary>
    public partial class Genesis3D : Window
    {
        string filePath, exportPath;

        public Genesis3D()
        {
            InitializeComponent();
        }

        private void Select_File_Click(object sender, RoutedEventArgs e)
        {
            GetFileDirs();
        }

        private void Load_File_Click(object sender, RoutedEventArgs e)
        {
            ActReader.ReadFile(filePath);
        }

        public void GetFileDirs()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckPathExists = true;
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            fileDialog.Title = "Open Model File";
            fileDialog.Filter = "Genesis3D Actor File|*.act";
            fileDialog.ValidateNames = true;
            fileDialog.CheckFileExists = true;
            fileDialog.FileName = "Choose a file and select \"Open\".";
            // Code for looking for a file.
            if (fileDialog.ShowDialog() == true)
            {
                string path = Path.GetFullPath(fileDialog.FileName);
                Debug.WriteLine("Selected path: " + path);
                filePath = path;
            }
            else
            {
                filePath = null;
                Debug.WriteLine("No file selected or file does not exist!");
            }
        }
    }
}
