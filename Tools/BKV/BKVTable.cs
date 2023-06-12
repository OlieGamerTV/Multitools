using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Tools.BKV
{
    public class BKVTable : Object
    {
        private BKVValue invalid = new BKVValue(BKVValue.TYPE_INVALID, new byte[0], new WeakReference(null));
        private Dictionary<string, BKVValue> keyMap;
        private List<string> keys;
        private List<BKVValue> values;
        private uint numValues;

        public BKVTable()
        {
            this.keyMap = new Dictionary<string, BKVValue>();
            this.keys = new List<string>();
            this.values = new List<BKVValue>();
        }

        public void AddPair(string param1, BKVValue param2)
        {
            Debug.WriteLine("numValues " + numValues);
            Debug.WriteLine(param1);
            this.keyMap[param1] = param2;
            this.keys.Add(param1);
            this.values.Add(param2);
            numValues++;
        }

        public uint GetNumValues()
        {
            return this.numValues;
        }

        public bool HasValue(string param1)
        {
            return this.keys.IndexOf(param1) != -1;
        }


        public BKVValue GetValue(dynamic param1)
        {
            if (param1 is string)
            {
                if (keyMap.ContainsKey((string)param1))
                {
                    return keyMap[(string)param1];
                }
            }
            else if (param1 is int)
            {
                if (param1 as int? < numValues)
                {
                    return values[(int)param1];
                }
            }
            return invalid;
        }

        public string GetKey(uint param1)
        {
            return param1 < numValues ? keys[(int)param1] : null;
        }

        private string DoToString(uint param1)
        {
            BKVValue loc6 = new BKVValue();
            object loc2 = "";
            uint loc3 = 0;
            while (loc3 < param1)
            {
                loc2 += "\t";
                loc3++;
            }
            dynamic loc4 = "";
            uint loc5 = GetNumValues();
            loc3 = 0;
            while (loc3 < loc5)
            {
                if ((loc6 = GetValue(loc3)).Type() != BKVValue.TYPE_TABLE)
                {
                    loc4 = (loc4 = (loc4 = (loc4(loc4 += loc2) + GetKey(loc3)) + " : ") + loc6.ToString()) + "\n";
                }
                loc3++;
            }
            loc3 = 0;
            while (loc3 < loc5)
            {
                if ((loc6 = GetValue(loc3)).Type() == BKVValue.TYPE_TABLE)
                {
                    loc4 = (loc4 = (loc4 = (loc4 += loc2) + GetKey(loc3)) + " :\n") + loc6.AsTable().DoToString(param1 + 1);
                }
                loc3++;
            }
            return loc4;
        }

        override public string ToString()
        {
            return DoToString(0);
        }
    }
}
