using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Alpine.Textures.Packed
{
    public class Image
    {
        private uint? id, replacementId;
        private string name, filename;
        private Image? texture;
        private Point? size, offset, scale, extOffset;

        public Image(params dynamic[] rest) : base()
        {
            if (rest.Length == 0)
            {
                return;
            }
            if (rest[0] is BKVTable)
            {
                FromBKV(rest[0] as BKVTable);
            }
            else
            {
                InitWithValues(rest[0] as uint?, rest[1] as Point?, rest[2] as Point?, rest[3] as Point?);
            }
        }

        private void InitWithValues(uint? param1, Point? param2, Point? param3, Point? param4)
        {
            this.id = param1;
            replacementId = uint.MaxValue;
            size = new Point();
            offset = param2;
            scale = param3;
            extOffset = param4;
        }

        private void FromBKV(BKVTable param1)
        {
            replacementId = uint.MaxValue;
            this.id = (uint)param1.GetValue("id").AsInt();
            if (param1.HasValue("name"))
            {
                name = param1.GetValue("name").AsString();
            }
            if (param1.HasValue("filename"))
            {
                filename = param1.GetValue("filename").AsString();
            }
            this.size = new Point(param1.GetValue("width").AsInt(), param1.GetValue("height").AsInt());
            this.offset = new Point(param1.GetValue("offsetX").AsInt(), param1.GetValue("offsetY").AsInt());
        }

        public uint Id
        {
            get { return (uint)id; }
        }

        public uint ReplacementId
        {
            get { return (uint)replacementId; }
            set { replacementId = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Filename
        {
            get { return filename; }
        }

        public Image Texture
        {
            get { return texture; }
            set
            {
                this.texture = value;
                size = new Point((int)texture.Size.X, (int)texture.Size.Y);
            }
        }

        public Point Size
        {
            get { return (Point)size; }
        }

        public Point Offset
        {
            get { return (Point)offset; }
        }

        public Point Scale
        {
            get { return (Point)scale; }
        }

        public Point ExtOffset
        {
            get { return (Point)extOffset; }
        }
    }
}
