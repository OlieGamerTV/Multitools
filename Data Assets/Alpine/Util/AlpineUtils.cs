using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multi_Tool.Enumerators;
using Alpine.Geom;

namespace Alpine.Util
{
    internal class AlpineUtils
    {
        private static Rectangle TEMP_RECT = new Rectangle();
        private static Utilities.Flash.Point TEMP_POINT = new Utilities.Flash.Point();

        public AlpineUtils() : base() { }

        public static uint NextPow2(uint param1)
        {
            param1--;
            param1 |= param1 >>> 1;
            param1 |= param1 >>> 2;
            param1 |= param1 >>> 4;
            param1 |= param1 >>> 8;
            param1 |= param1 >>> 16;
            param1++;
            return param1;
        }

        private static string Vertex(List<float> param1, int[] param2, int param3)
        {
            int loc6 = 0;
            dynamic loc4 = "\t" + param3 + " ";
            foreach (int loc5 in param2)
            {
                loc4 += "<" + param1[param3];
                loc6 = 1;
                while (loc6 < loc5)
                {
                    loc4 += ", " + param1[param3 + loc6];
                    loc6++;
                }
                loc4 += "> ";
                param3 += loc5;
            }
            return loc4;
        }

        public static string ToString(List<float> param1, List<uint> param2, int[] param3, int param4, int param5)
        {
            int loc6 = (int)(param2[param5] * param4);
            int loc7 = (int)(param2[param5 + 1] * param4);
            int loc8 = (int)(param2[param5 + 2] * param4);
            return "Tri " + param5 + ":\n" + Vertex(param1, param3, loc6) + "\n" + Vertex(param1, param3, loc7) + "\n" + Vertex(param1, param3, loc8);
        }

        public static string VerticesToString(List<float> arg1, string arg2 = ",", int arg3 = -1)
        {
            if(arg1 == null)
            {
                return "";
            }
            int loc4 = (arg1.Count / 3), loc5 = 0, loc7 = 0;
            List<string> loc6 = new List<string>(loc4);
            if ((arg3 >= 0) && (arg3 <= 20))
            {
                while (loc7 < loc4)
                {
                    loc6.Insert(loc7, "(" + arg1[loc5++].ToString() + ", " + arg1[loc5++].ToString() + ", " + arg1[loc5++].ToString() + ")");
                    loc7++;
                }
            }
            else
            {
                while (loc7 < loc4)
                {
                    loc6.Insert(loc7, "(" + arg1[loc5++] + ", " + arg1[loc5++] + ", " + arg1[loc5++] + ")");
                    loc7++;
                }
            }
            return string.Join(arg2, loc6);
        }

        public static string Matrix3DToString(AlpineMatrix3D arg1)
        {
            return string.Join(", ", arg1.ToList());
        }

        public static ByteArray CreateByteArray(dynamic? param1 = null)
        {
            ByteArray loc3 = null;
            List<uint> loc4 = null;
            List<int> loc6 = null;
            List<float> loc8 = null;
            byte[] loc10 = null;
            List<short> loc11 = null;
            ByteArray loc2 = new ByteArray();
            loc2.endian = Endian.LITTLE_ENDIAN;
            if(param1 != null)
            {
                if (param1 is int)
                {
                    loc2.Length = param1;
                }
                else if (param1 is uint)
                {
                    loc2.Length = (int)param1;
                }
                else if (param1 is ByteArray)
                {
                    loc3 = param1 as ByteArray;
                    loc3.Position = 0;
                    loc2.Length = loc3.Length;
                    loc2.WriteBytes(loc3);
                }
                else if(param1 is byte[])
                {
                    loc10 = param1;
                    loc2.Length = loc10.Length;
                    loc2.WriteBytes(loc10);
                }
                else if (param1 is List<uint>)
                {
                    loc4 = param1 as List<uint>;
                    foreach (uint loc5 in loc4)
                    {
                        loc2.WriteShort(loc5);
                    }
                }
                else if (param1 is List<int>)
                {
                    loc6 = param1 as List<int>;
                    foreach (int loc7 in loc6)
                    {
                        loc2.WriteShort(loc7);
                    }
                }
                else if (param1 is List<float>)
                {
                    loc8 = param1 as List<float>;
                    foreach (float loc9 in loc8)
                    {
                        loc2.WriteFloat(loc9);
                    }
                }
            }
            loc2.Position = 0;
            return loc2;
        }
    }
}
