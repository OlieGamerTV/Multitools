using Multi_Tool.Tools.Unity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Multi_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Disney_Canvas.Visibility = Visibility.Collapsed;
            Unity_Canvas.Visibility = Visibility.Collapsed;
        }

        private void Disney_Algo_Button_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Visibility = Visibility.Collapsed;
            Disney_Canvas.Visibility = Visibility.Visible;
            this.Title = "Multitools - Disney Compressions Menu";
        }

        private void Go_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Visibility = Visibility.Visible;
            if (Disney_Canvas.Visibility == Visibility.Visible)
            {
                Disney_Canvas.Visibility = Visibility.Collapsed;
            }
            if (Unity_Canvas.Visibility == Visibility.Visible)
            {
                Unity_Canvas.Visibility = Visibility.Collapsed;
            }
            this.Title = "Multitools - Main Menu";
        }

        private void Embed_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new File_Decompiler();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void DPack_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new DPack_Compression();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void BKV_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new BKV_Reader();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void Common_Algo_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new Common_File_Comp();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void PVR_Convert_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new PVR_Converter();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void Model_Button_Click(object sender, RoutedEventArgs e)
        {
            Model_Builder newWindow = new Model_Builder();
            newWindow.Owner = this;
            newWindow.Show();
        }

        private void Unity_Button_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Visibility = Visibility.Collapsed;
            Unity_Canvas.Visibility = Visibility.Visible;
            this.Title = "Multitools - Unity Tools Menu";
        }

        private void Atlas_Converter_Button_Click(object sender, RoutedEventArgs e)
        {
            NGUI_Atlas_Separator nGUIWindow = new NGUI_Atlas_Separator();
            nGUIWindow.Owner = this;
            nGUIWindow.Show();
        }

        private void Genesis3D_Button_Click(object sender, RoutedEventArgs e)
        {
            Genesis3D gen3DWindow = new Genesis3D();
            gen3DWindow.Owner = this;
            gen3DWindow.Show();
        }
    }
}
