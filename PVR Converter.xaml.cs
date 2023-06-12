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
    /// Interaction logic for PVR_Converter.xaml
    /// </summary>
    public partial class PVR_Converter : Window
    {
        PVRConvert pvrConvert;
        int previousIndex = 0;

        public PVR_Converter()
        {
            InitializeComponent();
            pvrConvert = new PVRConvert(OutputList, PVR_CurPixelFormat, PVR_CurrentVer, PVR_DesiredPixFormat_Combo);
            SetupPVRPixFormats();
        }

        private void ConvertPVR_Button_Click(object sender, RoutedEventArgs e)
        {
            pvrConvert.StartConvert();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new MainWindow();
            newWindow.Owner = this;
            newWindow.Show();
            newWindow.Owner = null;
            Close();
        }

        private void SelectFile_Button_Click(object sender, RoutedEventArgs e)
        {
            pvrConvert.GetDirs(true);
        }

        private void ExportDir_Button_Click(object sender, RoutedEventArgs e)
        {
            pvrConvert.GetDirs(false);
        }

        private void DesiredPVR_Type_DropDownClosed(object sender, EventArgs e)
        {
            if (PVR_DesiredType_Combo.SelectedIndex != previousIndex)
            {
                SetupPVRPixFormats();
            }
        }

        private void SetupPVRPixFormats()
        {
            string[] pixelTypes = pvrConvert.SetCurrentPixFormat(PVR_DesiredType_Combo.SelectedIndex);
            PVR_DesiredPixFormat_Combo.Items.Clear();
            for (int i = 0; i < pixelTypes.Length; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = pixelTypes[i];
                PVR_DesiredPixFormat_Combo.Items.Add(item);
            }
            PVR_DesiredPixFormat_Combo.SelectedIndex = 0;
            previousIndex = PVR_DesiredType_Combo.SelectedIndex;
        }

        private void PVR_UseDesiredPixFormat_Checked(object sender, RoutedEventArgs e)
        {
            pvrConvert.useOtherPixel = true;
        }

        private void PVR_UseDesiredPixFormat_Unchecked(object sender, RoutedEventArgs e)
        {
            pvrConvert.useOtherPixel = false;
        }
    }
}
