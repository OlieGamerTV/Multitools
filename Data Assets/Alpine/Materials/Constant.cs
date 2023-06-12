using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class Constant
    {
        public float x, y, z, w;
        public List<float> values;

        public Constant(ByteArray param1 = null) : base()
        {
            if (param1 == null)
            {
                values = new List<float>(4);
            }
            else
            {
                x = param1.ReadFloat();
                y = param1.ReadFloat();
                z = param1.ReadFloat();
                w = param1.ReadFloat();
                values = new List<float>{x,y,z,w};
            }
        }

        public void Set(float param1,  float param2, float param3, float param4)
        {
            values.Insert(0, param1);
            values.Insert(1, param2);
            values.Insert(2, param3);
            values.Insert(3, param4);
            x = values[0];
            y = values[1];
            z = values[2];
            w = values[3];
        }

        public Constant Clone()
        {
            Constant loc1 = new Constant();
            loc1.Set(this.x, this.y, this.z, this.w);
            return loc1;
        }
    }
}
