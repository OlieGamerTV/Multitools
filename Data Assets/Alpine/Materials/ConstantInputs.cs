using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class ConstantInputs
    {
        public List<string> names;
        public int numInputs;

        public ConstantInputs(ByteArray param1 = null) : base()
        {
            uint loc2 = 0;
            List<string> loc3 = null;
            int loc4 = 0;
            if (param1 == null)
            {
                names = new List<string>();
                numInputs = 0;
            }
            else
            {
                loc2 = param1.ReadUnsignedByte();
                loc3 = names = new List<string>((int)loc2);
                while (loc4 < loc2)
                {
                    loc3[loc4] = param1.ReadUTF();
                    loc4++;
                }
                numInputs = (int)loc2;
            }
        }

        public void Add(string param1)
        {
            names.Add(param1);
            numInputs++;
        }

        public void Remove(string param1)
        {
            int loc2 = names.IndexOf(param1);
            if (loc2 == -1)
            {
                return;
            }
            names.RemoveAt(loc2);
            numInputs--;
        }

        public bool IsEqual(ConstantInputs other)
        {
            if (numInputs != other.numInputs)
            {
                return false;
            }
            foreach (string loc2 in names)
            {
                if(other.names.IndexOf(loc2) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        public ConstantInputs Clone()
        {
            ConstantInputs loc1 = new ConstantInputs();
            loc1.names = names;
            loc1.numInputs = numInputs;
            return loc1;
        }
    }
}
