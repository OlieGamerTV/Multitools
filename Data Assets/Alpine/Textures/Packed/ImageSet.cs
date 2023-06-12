using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Alpine.Textures.Packed
{
    public class ImageSet
    {
        private uint id;
        private string name;
        private Point? maxSize;
        private List<int> frameTable;
        private List<Image>? frames;

        public ImageSet(params dynamic[] rest) : base()
        {
            if (rest.Length == 0)
            {
                return;
            }
            if (rest[0] is BKVTable)
            {

            }
            else
            {
                InitFromValues(rest[0], rest.Length >= 2 ? rest[1] : null, rest.Length >= 3 ? rest[2] : null);
            }
        }

        private void InitFromValues(uint param1, Point? param2, List<Image>? param3 = null)
        {
            id = param1;
            maxSize = param2;
            if (maxSize == null)
            {
                maxSize = new Point(0, 0);
            }
            frames = param3;
            if(frames == null)
            {
                frames = new List<Image>();
            }
        }

        private void FromBKV(BKVTable param1)
        {
            id = param1.GetValue("id").AsUInt();
            name = param1.GetValue("name").AsString();
            BKVTable loc2 = param1.GetValue("frames").AsTable();
            int loc3 = (int)loc2.GetNumValues();
            frames = new List<Image>(loc3);
            int loc4 = 0;
            while (loc4 < loc3)
            {
                frames[loc4] = new Image(loc2.GetValue(loc4).AsTable());
                loc4++;
            }
        }

        public uint Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public Point? MaxSize
        {
            get { return maxSize; }
        }

        public int NumFrames
        {
            get
            {
                if (this.frameTable != null)
                {
                    return frameTable.Count;
                }
                return frames.Count;
            }
        }
    }
}
