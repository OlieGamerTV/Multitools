using Alpine.Textures.Packed;
using Multi_Tool.Tools.BKV;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using DrawImage = System.Drawing.Image;

namespace Alpine.Textures
{
    public class PackedTexture
    {
        public static Point NO_SCALE = new Point(1, 1);
        public static Point NO_OFFSET = new Point(0, 0);
        protected static uint hashFiller = 1;
        protected string name;
        protected int id, hash, width, height;
        protected DrawImage? texture;
        protected string extension;
        protected ByteArray? rawData;
        protected bool compressed;
        private Dictionary<int, ImageSet> imageSets;
        private Dictionary<int, Image> images;
        private int numReplacements;
        public bool downsample16BPP;

        public PackedTexture(string param1, params dynamic[] rest) : base()
        {
            name = param1;
            if (rest.Length == 1)
            {
                if (rest[0] is BKVTable)
                {
                    FromBKV(rest[0] as BKVTable);
                }
            }
        }

        private void FromBKV(BKVTable param1)
        {
            int loc2 = 0, loc3 = 0;
            BKVTable? loc4 = null, loc6 = null;
            Image? loc5 = null;
            ImageSet? loc7 = null;
            width = param1.GetValue("width").AsInt();
            height = param1.GetValue("height").AsInt();
            hash = param1.GetValue("hash").AsInt();
            if (param1.HasValue("images"))
            {
                loc3 = (int)(loc4 = param1.GetValue("images").AsTable()).GetNumValues();
                images = new Dictionary<int, Image>();
                loc2 = 0;
                while(loc2 < loc3)
                {
                    loc5 = new Image(loc4.GetValue(loc2).AsTable());
                    images.Add((int)loc5.Id, loc5);
                    loc2++;
                }
            }
        }

        public void Destroy()
        {
            images.Clear();
            imageSets.Clear();
            texture = null;
            rawData = null;
        }

        public void Repack(DrawImage param1, Dictionary<int, Image> param2, Dictionary<int, ImageSet> param3)
        {
            texture = param1;
            images = param2;
            imageSets = param3;
        }

        //Variable Get / Set Encapsulations

        public string Name
        {
            get {  return name; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        public uint Hash
        {
            get { return (uint)hash; }
        }

        public bool IsModified
        {
            get { return numReplacements > 0; }
        }

        public int NumReplacements
        {
            get { return numReplacements; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Width
        {
            get
            {
                if (texture == null)
                {
                    return width;
                }
                else
                {
                    return !this.compressed ? (int)(texture.Width) : width;
                }
            }
        }

        public int Height
        {
            get
            {
                if (texture == null)
                {
                    return height;
                }
                else
                {
                    return !this.compressed ? (int)(texture.Height) : height;
                }
            }
        }

        public DrawImage? Texture
        {
            get { return !this.compressed ? texture : null; }
            set
            {
                texture = value;
                rawData = null;
                compressed = false;
            }
        }

        public ByteArray? RawData
        {
            get { return !this.compressed ? null : rawData; }
            set
            {
                rawData = value;
                texture = null;
                compressed = true;
            }
        }

        public Dictionary<int, Image> Images
        {
            get { return images; }
        }

        public List<string> ImageNames
        {
            get
            {
                List<string> loc1 = new List<string>();
                if (images != null)
                {
                    foreach (dynamic loc2 in images)
                    {
                        loc1.Add(images[loc2].Name);
                    }
                }
                if (imageSets != null)
                {
                    foreach (dynamic loc2 in imageSets)
                    {
                        loc1.Add((imageSets[loc2].Name));
                    }
                }
                return loc1;
            }
        }
    }
}
