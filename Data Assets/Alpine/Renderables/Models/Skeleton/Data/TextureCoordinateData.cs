using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class TextureCoordinateData
    {
        public string name;
        public int id, size;
        public ByteArray data;
        public List<string> channels;

        public TextureCoordinateData(string param1, ByteArray param2 = null, int param3 = 2) : base()
        {
            this.id = -1;
            this.name = param1;
            this.data = param2;
            this.size = param3;
            this.channels = new List<string>(2);
        }
    }
}
