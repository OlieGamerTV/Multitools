using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Multi_Tool.Tools.BKV
{
    public struct BKVValue
    {
        public const int TYPE_INVALID = -1, TYPE_BOOL_FALSE = 0, TYPE_BOOL_TRUE = 1;
        public const int TYPE_FLOAT = 2, TYPE_BYTE = 3, TYPE_SHORT = 4, TYPE_INT = 5;
        public const int TYPE_STRING = 6, TYPE_TABLE = 7, TYPE_ARRAY = 8, TYPE_BINARY = 9;
        private BKVReader reader;
        private int type;
        private float n;

        public BKVValue(int param1, byte[] param2, WeakReference param3)
        {
            if (param3.Target == null)
            {
                this.reader = null;
            }
            else
            {
                this.reader = param3.Target as BKVReader;
            }
            this.type = param1;
            switch (type)
            {
                case TYPE_BOOL_FALSE:
                    n = 0;
                    break;
                case TYPE_BOOL_TRUE:
                    n = 1;
                    break;
                case TYPE_FLOAT:
                    n = BitConverter.ToSingle(param2);
                    break;
                case TYPE_BYTE:
                    n = param2[0];
                    break;
                case TYPE_SHORT:
                    n = BitConverter.ToInt16(param2);
                    break;
                case TYPE_INT:
                    n = BitConverter.ToInt32(param2);
                    break;
                case TYPE_STRING:
                case TYPE_TABLE:
                case TYPE_ARRAY:
                    n = BitConverter.ToUInt16(param2);
                    break;
                default:
                    type = TYPE_INVALID;
                    n = -1;
                    break;
            }
            Debug.WriteLine("BKVValue: Value created, Type - " + type + ", Value - " + n);
        }

        public BKVValue(int param1, ByteArray param2, WeakReference param3)
        {
            this.reader = param3.Target as BKVReader;
            this.type = param1;
            switch (type)
            {
                case TYPE_BOOL_FALSE:
                    n = 0;
                    break;
                case TYPE_BOOL_TRUE:
                    n = 1;
                    break;
                case TYPE_FLOAT:
                    n = param2.ReadFloat();
                    break;
                case TYPE_BYTE:
                    n = param2.ReadByte();
                    break;
                case TYPE_SHORT:
                    n = param2.ReadShort();
                    break;
                case TYPE_INT:
                    n = param2.ReadInt();
                    break;
                case TYPE_STRING:
                case TYPE_TABLE:
                case TYPE_ARRAY:
                    n = param2.ReadUnsignedShort();
                    break;
                default:
                    type = TYPE_INVALID;
                    n = -1;
                    break;
            }
            Debug.WriteLine("BKVValue: Value created, Type - " + type + ", Value - " + n);
        }

        public bool IsValid()
        {
            Debug.WriteLine("BKVValue: IsValid called. Type - " + type);
            return type != TYPE_INVALID;
        }

        public int Type()
        {
            Debug.WriteLine("BKVValue: Type called. Type - " + type);
            return type;
        }

        public float AsFloat()
        {
            Debug.WriteLine("BKVValue: AsFloat called. Type - " + type + ", Value - " + n);
            return type == TYPE_FLOAT || type == TYPE_BYTE || type == TYPE_SHORT || type == TYPE_INT ? n : float.NaN;
        }

        public int AsInt()
        {
            Debug.WriteLine("BKVValue: AsInt called. Type - " + type + ", Value - " + n);
            return type == TYPE_FLOAT || type == TYPE_BYTE || type == TYPE_SHORT || type == TYPE_INT ? Convert.ToInt32(n) : 0;
        }

        public uint AsUInt()
        {
            Debug.WriteLine("BKVValue: AsUInt called. Type - " + type + ", Value - " + n);
            return type == TYPE_FLOAT || type == TYPE_BYTE || type == TYPE_SHORT || type == TYPE_INT ? Convert.ToUInt32(n) : 0;
        }

        public bool AsBool()
        {
            Debug.WriteLine("BKVValue: AsBool called. Type - " + type);
            return type == TYPE_BOOL_TRUE;
        }

        public string AsString()
        {
            Debug.WriteLine("BKVValue: AsString called. Type - " + type + ", Value - " + n);
            return type == TYPE_STRING ? reader.GetString((int)n) : null;
        }

        public BKVTable AsTable()
        {
            Debug.WriteLine("BKVValue: AsTable called. Type - " + type + ", Value - " + n);
            return type == TYPE_TABLE ? reader.GetTable((int)n) : null;
        }

        public List<BKVValue> AsArray()
        {
            Debug.WriteLine("BKVValue: AsArray called. Type - " + type + ", Value - " + n);
            return type == TYPE_ARRAY ? reader.GetArray((int)n) : null;
        }

        public string ValueToString()
        {
            List<BKVValue> loc1 = new List<BKVValue>(1);
            uint loc2 = 0, loc4 = 0;
            dynamic loc3;
            switch (type)
            {
                case TYPE_BOOL_FALSE:
                    return "False";
                case TYPE_BOOL_TRUE:
                    return "True";
                case TYPE_BYTE:
                case TYPE_SHORT:
                case TYPE_INT:
                    return AsInt().ToString();
                case TYPE_FLOAT:
                    return AsFloat().ToString();
                case TYPE_STRING:
                    return AsString();
                case TYPE_ARRAY:
                    loc1 = AsArray();
                    loc2 = (uint)loc1.Count;
                    loc3 = "<" + loc2 + ">[";
                    loc4 = 0;
                    while (loc4 < loc2)
                    {
                        if (loc4 != 0)
                        {
                            loc3 += ", ";
                        }
                        loc3 += loc1[(int)loc4];
                        loc4++;
                    }
                    return loc3 + "]";
                case TYPE_TABLE:
                    return "[Table]";
                case TYPE_BINARY:
                    return "[Binary]";
                case TYPE_INVALID:
                    return "[Invalid]";
                default:
                    return "[Unknown]";
            }
        }
    }
}
