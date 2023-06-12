using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Renderables.Models.Skeleton;
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
using System.IO;
using Utilities.Flash;
using Alpine.Renderables.Models.Animation.Data;
using Multi_Tool.Result_Windows;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for Model_Builder.xaml
    /// </summary>
    public partial class Model_Builder : Window
    {
        private const string modelName = "Select the Alpine Model Package file you want to build from.\n";
        private const string modelFilter = "Alpine Model Package (*.mmf)|*.mmf|All Files (*.*)|*.*";
        private const string animName = "Select the Alpine Animation Package file you want to build from.\n";
        private const string animFilter = "Alpine Animation Package (*.mma)|*.mma|All Files (*.*)|*.*";
        string fileName, modelPath, texturesPath;
        ModelData model;
        AnimationData animation;
        List<Mesh> modelMesh;
        MergedMesh mergedMesh;
        ByteArray modelFile, texturesFile;
        public Model_Builder()
        {
            InitializeComponent();
            model = new ModelData(Output_List);
            animation = new AnimationData(Output_List);
        }

        private void StartAnim_Button_Click(object sender, RoutedEventArgs e)
        {
            Output_List.Items.Clear();
            GetFileDirs(animFilter, animName);
            animation.Load(fileName, modelFile, texturesFile);
        }

        private void StartModel_Button_Click(object sender, RoutedEventArgs e)
        {
            Output_List.Items.Clear();
            GetFileDirs(modelFilter, modelName);
            Mouse.OverrideCursor = Cursors.AppStarting;
            model.Load(fileName, modelFile, texturesFile);
            Mouse.OverrideCursor = null;
            modelMesh = new List<Mesh>();
            for(int i = 0; i < model.meshes.Count; i++)
            {
                modelMesh.Insert(i, new Mesh(model.meshes[i]));
            }
            mergedMesh = new MergedMesh(0);
            foreach(Mesh mesh in modelMesh)
            {
                mergedMesh.Merge(mesh);
            }
            Alpine_Model_List model_List = new Alpine_Model_List(model);
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        public void GetFileDirs(string fileFilter, string fileBarName)
        {
            string message = "Does this file have a .mmt file included?", caption = "Model Data";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckPathExists = true;
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            fileDialog.Title = "Open Model File";
            fileDialog.Filter = fileFilter;
            fileDialog.ValidateNames = true;
            fileDialog.CheckFileExists = true;
            fileDialog.FileName = fileBarName;
            // Code for looking for a file.
            if (fileDialog.ShowDialog() == true)
            {
                string path = Path.GetFullPath(fileDialog.FileName);
                Debug.WriteLine("Selected path: " + path);
                string file = Path.GetFileNameWithoutExtension(path);
                Debug.WriteLine("Selected file: " + file);
                fileName = file;
                modelPath = path;
                Debug.WriteLine("Selected model file to rip set to: " + path);
                modelFile = new ByteArray(File.ReadAllBytes(path));
            }
            else
            {
                Debug.WriteLine("No file selected or file does not exist!");
            }
            result = MessageBox.Show(message, caption, button);
            if(result == MessageBoxResult.Yes)
            {
                fileDialog.Title = "Open Textures File";
                fileDialog.Filter = "Alpine Textures Package (*.mmt)|*.mmt|All Files (*.*)|*.*";
                fileDialog.FileName = "Select the Alpine Textures Package file you want to use.";
                if (fileDialog.ShowDialog() == true)
                {
                    string path = Path.GetFullPath(fileDialog.FileName);
                    Debug.WriteLine("Selected path: " + path);
                    texturesPath = path;
                    Debug.WriteLine("Selected textures file to rip set to: " + path);
                    texturesFile = new ByteArray(File.ReadAllBytes(path));
                }
                else
                {
                    Debug.WriteLine("No file selected or file does not exist!");
                }
            }
            else if(result == MessageBoxResult.No)
            {
                texturesFile = null;
            }
        }
    }
}
