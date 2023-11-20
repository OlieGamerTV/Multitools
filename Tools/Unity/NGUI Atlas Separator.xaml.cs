using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using SysPath = System.IO.Path;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using DrawGraphics = System.Drawing.Graphics;
using DrawBitmap = System.Drawing.Bitmap;
using DrawImage = System.Drawing.Image;

namespace Multi_Tool.Tools.Unity
{
    /// <summary>
    /// Interaction logic for NGUI_Atlas_Separator.xaml
    /// </summary>
    public partial class NGUI_Atlas_Separator : Window
    {
        List<AtlasSprite> atlasSprites = new List<AtlasSprite>();
        Canvas displayCanvas;
        DrawImage atlasImage;
        NGUI_Atlas_Display display;
        public NGUI_Atlas_Separator()
        {
            InitializeComponent();
            display = new NGUI_Atlas_Display();
            display.Show();
            AddDisplayCanvas(ref display.ImageDisplay);
        }

        void AddDisplayCanvas(ref Canvas canvas)
        {
            displayCanvas = canvas;
        }

        private void AtlasTextureLoader_Button_Click(object sender, RoutedEventArgs e)
        {
            SetAtlasTexture();
        }

        private void AtlasLoader_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Open Atlas Texture File";
            dialog.Filter = "Unity Prefab Files (*.prefab)|*.prefab";
            dialog.ValidateNames = true;
            dialog.CheckFileExists = true;
            if(dialog.ShowDialog() == true)
            {
                string file = File.ReadAllText(dialog.FileName);
                List<string> atlas = file.Split("- name:").ToList();
                atlas.RemoveAt(0);
                for (int i = 0; i < atlas.Count; i++)
                {
                    string originalString = atlas[i];
                    atlas[i] = originalString.Insert(0, "name:");
                }
                atlas[atlas.Count - 1] = atlas[atlas.Count - 1].Remove(atlas[atlas.Count - 1].IndexOf("mPixelSize"));
                atlasSprites = ReadAtlasString(atlas);
                Atlas_Sprites.ItemsSource = atlasSprites;
                Atlas_Sprites.Items.Refresh();
                foreach(AtlasSprite sprite in atlasSprites)
                {
                    Rectangle rect = new Rectangle();
                    rect.Stroke = Brushes.White;
                    rect.Width = sprite.Width;
                    rect.Height = sprite.Height;
                    sprite.SetSpriteRect(ref rect);
                    displayCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, sprite.X);
                    Canvas.SetTop(rect, sprite.Y);
                }
            }
        }

        private void SetAtlasTexture()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Open Atlas Texture File";
            dialog.Filter = "PNG Files (*.png)|*.png";
            dialog.ValidateNames = true;
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                switch (SysPath.GetExtension(dialog.FileName))
                {
                    case ".png":
                        displayCanvas.Children.Clear();
                        atlasImage = DrawImage.FromFile(dialog.FileName);
                        BitmapImage bitmapImage = new BitmapImage(new Uri(dialog.FileName));
                        Image image = new Image();
                        image.Source = bitmapImage;
                        image.Stretch = Stretch.Uniform;
                        displayCanvas.Width = bitmapImage.Width;
                        displayCanvas.Height = bitmapImage.Height;
                        displayCanvas.Children.Add(image);
                        break;
                }
            }
        }

        private List<AtlasSprite> ReadAtlasString(List<string> elements)
        {
            StringReader reader;
            List<AtlasSprite> sprites = new List<AtlasSprite>();
            foreach (string sprite in elements)
            {
                reader = new StringReader(sprite);
                Debug.WriteLine(sprite);

                // General Data
                reader.ReadBlock("name: ".ToCharArray());
                string name = reader.ReadLine();
                reader.ReadBlock("      x: ".ToCharArray());
                float xPos = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      y: ".ToCharArray());
                float yPos = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      width: ".ToCharArray());
                float width = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      height: ".ToCharArray());
                float height = Convert.ToSingle(reader.ReadLine());

                // Borders
                reader.ReadBlock("      borderLeft: ".ToCharArray());
                float borderL = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      borderRight: ".ToCharArray());
                float borderR = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      borderTop: ".ToCharArray());
                float borderT = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      borderBottom: ".ToCharArray());
                float borderB = Convert.ToSingle(reader.ReadLine());

                // Padding
                reader.ReadBlock("      paddingLeft: ".ToCharArray());
                float padL = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      paddingRight: ".ToCharArray());
                float padR = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      paddingTop: ".ToCharArray());
                float padT = Convert.ToSingle(reader.ReadLine());
                reader.ReadBlock("      paddingBottom: ".ToCharArray());
                float padB = Convert.ToSingle(reader.ReadLine());

                // Creating Sprite Data
                AtlasSprite spriteData = new AtlasSprite(name, xPos, yPos, width, height, borderL, borderR, borderT, borderB, padL, padR, padT, padB);
                Debug.WriteLine(spriteData.ToString());
                sprites.Add(spriteData);
            }
            return sprites;
        }

        private void ExportFiles_Button_Click(object sender, RoutedEventArgs e)
        {
            string exitPath = GetSaveLocation();
            if (!string.IsNullOrEmpty(exitPath))
            {
                foreach (AtlasSprite atlas in atlasSprites)
                {
                    Rectangle rectangle = atlas.GetSpriteRect();
                    using (DrawBitmap bm = new DrawBitmap((int)atlas.Width, (int)atlas.Height))
                    {
                        using (DrawGraphics graphics = DrawGraphics.FromImage(bm))
                        {
                            graphics.DrawImage(atlasImage, new System.Drawing.Rectangle(0, 0, (int)rectangle.Width, (int)rectangle.Height), new System.Drawing.Rectangle((int)Canvas.GetLeft(rectangle), (int)Canvas.GetTop(rectangle), (int)rectangle.Width, (int)rectangle.Height), System.Drawing.GraphicsUnit.Pixel);
                            bm.Save(exitPath + atlas.Name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            Debug.WriteLine("Sprite " + atlas.Name + " exported to " + exitPath);
                        }
                    }
                }
            }
        }

        private string GetSaveLocation()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Sprite Files";
            dialog.FileName = "Enter a folder to export the files to.";
            if (dialog.ShowDialog() == true)
            {
                string savePath = SysPath.GetFullPath(dialog.FileName).Remove(dialog.FileName.LastIndexOf('\\') + 1);
                return savePath;
            }
            return null;
        }
    }

    public class AtlasSprite
    {
        string name = "";
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        float borderLeft = 0;
        float borderRight = 0;
        float borderTop = 0;
        float borderBottom = 0;
        float paddingLeft = 0;
        float paddingRight = 0;
        float paddingTop = 0;
        float paddingBottom = 0;

        public string Name
        {
            get { return name; }
            set 
            {
                if(name != value)
                {
                    name = value;
                    NotifyPropertyChanged("NAME");
                }
            }
        }
        public float X
        {
            get { return x; }
            set 
            { 
                if(x != value)
                {
                    x = value;
                    NotifyPropertyChanged("XPOS");
                }
            }
        }
        public float Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    NotifyPropertyChanged("YPOS");
                }
            }
        }
        public float Width
        {
            get { return  width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    NotifyPropertyChanged("WIDTH");
                }
            }
        }
        public float Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    NotifyPropertyChanged("HEIGHT");
                }
            }
        }
        public float BorderLeft { get { return borderLeft; } set { borderLeft = value; } }
        public float BorderRight { get { return borderRight; } set { borderRight = value; } }
        public float BorderTop { get { return borderTop; } set { borderTop = value; } }
        public float BorderBottom { get { return borderBottom; } set { borderBottom = value; } }
        public float PaddingLeft { get { return paddingLeft; } set { paddingLeft = value; } }
        public float PaddingRight { get { return paddingRight; } set { paddingRight = value; } }
        public float PaddingTop { get { return paddingTop; } set { paddingTop = value; } }
        public float PaddingBottom { get { return paddingBottom; } set { paddingBottom = value; } }

        Rectangle Sprite { get; set; }

        public AtlasSprite(string name = "", float xPos = 0, float yPos = 0, float width = 0, float height = 0, float borderL = 0, float borderR = 0, float borderT = 0, float borderB = 0, float paddingL = 0, float paddingR = 0, float paddingT = 0, float paddingB = 0)
        {
            Name = name;
            X = xPos;
            Y = yPos;
            Width = width;
            Height = height;
            BorderLeft = borderL;
            BorderRight = borderR;
            BorderTop = borderT;
            BorderBottom = borderB;
            PaddingLeft = paddingL;
            PaddingRight = paddingR;
            PaddingTop = paddingT;
            PaddingBottom = paddingB;
        }

        public void SetSpriteRect(ref Rectangle spriteRect)
        {
            Sprite = spriteRect;
        }

        public Rectangle GetSpriteRect()
        {
            return Sprite;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            Debug.WriteLine("Firing Property Changed, String Parameter: " + propertyName);
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: ( X [{1}], Y [{2}], Width [{3}], Height [{4}], Border: [ L [{5}], R [{6}], T [{7}], B [{8}] ], Padding: [ L [{9}], R [{10}], T [{11}], B [{12}] ])", 
                name, x, y, width, height, borderLeft, borderRight, borderTop, borderBottom, paddingLeft, paddingRight, paddingTop, paddingBottom);
        }
    }
}
