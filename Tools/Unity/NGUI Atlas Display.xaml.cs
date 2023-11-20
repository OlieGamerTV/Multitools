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

namespace Multi_Tool.Tools.Unity
{
    /// <summary>
    /// Interaction logic for NGUI_Atlas_Display.xaml
    /// </summary>
    public partial class NGUI_Atlas_Display : Window
    {
        Canvas _canvas;
        public NGUI_Atlas_Display()
        {
            InitializeComponent();
            _canvas = ImageDisplay;
        }
    }
}
