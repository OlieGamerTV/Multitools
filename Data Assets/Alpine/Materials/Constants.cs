using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class Constants
    {
        public List<Constant> constants;
        public int numConstants;

        public Constants(ByteArray param1 = null) : base()
        {
            uint loc2 = 0;
            List<Constant> loc3 = null;
            int loc4 = 0;
            if(param1 == null)
            {
                constants = new List<Constant>();
                numConstants = 0;
            }
            else
            {
                loc2 = param1.ReadUnsignedByte();
                loc3 = constants = new List<Constant>((int)loc2);
                while (loc4 < loc2)
                {
                    loc3[loc4] = new Constant(param1);
                    loc4++;
                }
                numConstants = (int)loc2;
            }
        }

        public Constants Clone()
        {
            Constants loc1 = new Constants();
            loc1.numConstants = numConstants;
            loc1.constants = constants;
            return loc1;
        }
    }
}
