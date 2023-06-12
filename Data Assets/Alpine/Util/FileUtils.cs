using Alpine.Geom;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Alpine.Util
{
    internal class FileUtils
    {
        public enum Encoding : int
        {
            ENCODING_NONE = 0,
            ENCODING_BYTE = 1,
            ENCODING_BYTE_SIGNED = 2,
            ENCODING_SHORT = 3,
            ENCODING_SHORT_SIGNED = 4,
            UNENCODED_BYTE = 5,
            UNENCODED_BYTE_SIGNED = 6,
            UNENCODED_SHORT = 7,
            UNENCODED_SHORT_SIGNED = 8
        }

        public Encoding encoding;

        public FileUtils() : base() { }

        public static float ReadNumber(ByteArray param1, int param2)
        {
            switch (param2)
            {
                case (int)Encoding.ENCODING_NONE: return param1.ReadFloat();
                case (int)Encoding.ENCODING_BYTE: return param1.ReadUnsignedByte() / 255;
                case (int)Encoding.ENCODING_BYTE_SIGNED: return param1.ReadByte() / 127;
                case (int)Encoding.ENCODING_SHORT: return param1.ReadUnsignedShort() / 65535;
                case (int)Encoding.ENCODING_SHORT_SIGNED: return param1.ReadShort() / 32767;
                case (int)Encoding.UNENCODED_BYTE: return param1.ReadUnsignedByte();
                case (int)Encoding.UNENCODED_BYTE_SIGNED: return param1.ReadByte();
                case (int)Encoding.UNENCODED_SHORT: return param1.ReadUnsignedShort();
                case (int) Encoding.UNENCODED_SHORT_SIGNED: return param1.ReadShort();
                default: return float.NaN;
            }
        }

        public static float ReadNumber(byte[] param1, int param2)
        {
            MemoryStream paramStream = new MemoryStream(param1);
            BinaryReader stream = new BinaryReader(paramStream, System.Text.Encoding.Default);
            switch (param2)
            {
                case (int)Encoding.ENCODING_NONE: return stream.ReadSingle();
                case (int)Encoding.ENCODING_BYTE: return stream.ReadByte() / 255;
                case (int)Encoding.ENCODING_BYTE_SIGNED: return stream.ReadSByte() / 127;
                case (int)Encoding.ENCODING_SHORT: return stream.ReadUInt16() / 65535;
                case (int)Encoding.ENCODING_SHORT_SIGNED: return stream.ReadInt16() / 32767;
                case (int)Encoding.UNENCODED_BYTE: return stream.ReadByte();
                case (int)Encoding.UNENCODED_BYTE_SIGNED: return stream.ReadSByte();
                case (int)Encoding.UNENCODED_SHORT: return stream.ReadUInt16();
                case (int)Encoding.UNENCODED_SHORT_SIGNED: return stream.ReadInt16();
                default: return float.NaN;
            }
        }

        public static List<AlpineTransform> ReadTransformPool(ByteArray param1)
        {
            if (param1 == null)
            {
                return null;
            }
            int loc2 = param1.ReadUnsignedByte(), loc3 = param1.ReadUnsignedShort(), loc5 = 0;
            Debug.WriteLine("Number Encoding: " + loc2 + ", Transform List Length: " + loc3);
            List<AlpineTransform> loc4 = new List<AlpineTransform>(loc3);
            while (loc5 < loc3)
            {
                loc4.Insert(loc5, new AlpineTransform(param1.ReadFloat(), param1.ReadFloat(), param1.ReadFloat(), ReadNumber(param1, loc2), ReadNumber(param1, loc2), ReadNumber(param1, loc2), ReadNumber(param1, loc2), param1.ReadFloat()));
                Debug.WriteLine("Transform " + loc5 + " found - " + loc4[loc5].ToString());
                loc5++;
            }
            return loc4;
        }

        public static List<AlpineTransform>? ReadTransformPool(byte[] param1)
        {
            if (param1 == null)
            {
                return null;
            }
            MemoryStream paramStream = new MemoryStream(param1);
            BinaryReader stream = new BinaryReader(paramStream, System.Text.Encoding.Default);
            int loc2 = stream.ReadByte(), loc3 = stream.ReadInt16(), loc5 = 0;
            List<AlpineTransform> loc4 = new List<AlpineTransform>(loc3);
            while (loc5 < loc3)
            {
                loc4.Insert(loc5, new AlpineTransform(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle()));
                loc5++;
            }
            stream.Close();
            return loc4;
        }

        public static ByteArray? ReadVectorOfNumbers(ByteArray param1, int param2 = -1, int debug_length = 3)
        {
            float loc5 = float.NaN;
            if (param1 == null)
            {
                return null;
            }
            int loc3 = (int)param1.Length;
            if (param2 == -1)
            {
                param2 = param1.ReadUnsignedByte();
                loc3--;
            }
            switch (param2)
            {
                case (int)Encoding.ENCODING_NONE:
                    loc3 /= 4;
                    break;
                case (int)Encoding.ENCODING_BYTE:
                case (int)Encoding.ENCODING_BYTE_SIGNED:
                case (int)Encoding.UNENCODED_BYTE:
                case (int)Encoding.UNENCODED_BYTE_SIGNED:
                    break;
                case (int)Encoding.ENCODING_SHORT:
                case (int)Encoding.ENCODING_SHORT_SIGNED:
                case (int)Encoding.UNENCODED_SHORT:
                case (int)Encoding.UNENCODED_SHORT_SIGNED:
                    loc3 /= 4;
                    break;
            }
            ByteArray loc4 = AlpineUtils.CreateByteArray(loc3 * 4);
            int loc6 = 0;
            List<float> paramList = new List<float>();
            while (loc6 < loc3)
            {
                switch (param2)
                {
                    case (int)Encoding.ENCODING_NONE:
                        loc5 = param1.ReadFloat();
                        break;
                    case (int)Encoding.ENCODING_BYTE: 
                        loc5 = param1.ReadUnsignedByte() / 255;
                        break;
                    case (int)Encoding.ENCODING_BYTE_SIGNED: 
                        loc5 = param1.ReadByte() / 127;
                        break;
                    case (int)Encoding.ENCODING_SHORT:
                        loc5 = param1.ReadShort() / 65535;
                        break;
                    case (int)Encoding.ENCODING_SHORT_SIGNED:
                        loc5 = param1.ReadShort() / 32767;
                        break;
                    case (int)Encoding.UNENCODED_BYTE:
                        loc5 = param1.ReadUnsignedByte();
                        break;
                    case (int)Encoding.UNENCODED_BYTE_SIGNED:
                        loc5 = param1.ReadByte();
                        break;
                    case (int)Encoding.UNENCODED_SHORT:
                        loc5 = param1.ReadUnsignedShort();
                        break;
                    case (int)Encoding.UNENCODED_SHORT_SIGNED:
                        loc5 = param1.ReadShort();
                        break;
                }
                paramList.Add(loc5);
                loc4.WriteFloat(loc5);
                loc6++;
            }
            int count = 0;
            List<float> tempList = new List<float>();
            Debug.WriteLine("ReadVectorOfNumbers: Encoding: " + param2 + ", Amount of numbers read: " + paramList.Count);
            Debug.WriteLine("ReadVectorOfNumbers: [ ");
            foreach (float value in paramList)
            {
                if (count < debug_length)
                {
                    tempList.Add(value);
                    count++;
                }
                else
                {
                    Debug.WriteLine(string.Join(' ', tempList));
                    tempList = new List<float>();
                    tempList.Add(value);
                    count = 1;
                }

            }
            Debug.WriteLine(string.Join(' ', tempList) + " ]");
            return loc4;
        }

        public static List<float>? ReadNumberVectorToList(ByteArray param1, int param2 = -1, int debug_length = 3)
        {
            float loc5 = float.NaN;
            if (param1 == null)
            {
                return null;
            }
            int loc3 = (int)param1.Length;
            if (param2 == -1)
            {
                param2 = param1.ReadUnsignedByte();
                loc3--;
            }
            switch (param2)
            {
                case (int)Encoding.ENCODING_NONE:
                    loc3 /= 4;
                    break;
                case (int)Encoding.ENCODING_BYTE:
                case (int)Encoding.ENCODING_BYTE_SIGNED:
                case (int)Encoding.UNENCODED_BYTE:
                case (int)Encoding.UNENCODED_BYTE_SIGNED:
                    break;
                case (int)Encoding.ENCODING_SHORT:
                case (int)Encoding.ENCODING_SHORT_SIGNED:
                case (int)Encoding.UNENCODED_SHORT:
                case (int)Encoding.UNENCODED_SHORT_SIGNED:
                    loc3 /= 4;
                    break;
            }
            ByteArray loc4 = AlpineUtils.CreateByteArray(loc3 * 4);
            int loc6 = 0;
            List<float> paramList = new List<float>();
            while (loc6 < loc3)
            {
                switch (param2)
                {
                    case (int)Encoding.ENCODING_NONE:
                        loc5 = param1.ReadFloat();
                        break;
                    case (int)Encoding.ENCODING_BYTE:
                        loc5 = param1.ReadUnsignedByte() / 255;
                        break;
                    case (int)Encoding.ENCODING_BYTE_SIGNED:
                        loc5 = param1.ReadByte() / 127;
                        break;
                    case (int)Encoding.ENCODING_SHORT:
                        loc5 = param1.ReadShort() / 65535;
                        break;
                    case (int)Encoding.ENCODING_SHORT_SIGNED:
                        loc5 = param1.ReadShort() / 32767;
                        break;
                    case (int)Encoding.UNENCODED_BYTE:
                        loc5 = param1.ReadUnsignedByte();
                        break;
                    case (int)Encoding.UNENCODED_BYTE_SIGNED:
                        loc5 = param1.ReadByte();
                        break;
                    case (int)Encoding.UNENCODED_SHORT:
                        loc5 = param1.ReadUnsignedShort();
                        break;
                    case (int)Encoding.UNENCODED_SHORT_SIGNED:
                        loc5 = param1.ReadShort();
                        break;
                }
                paramList.Add(loc5);
                loc4.WriteFloat(loc5);
                loc6++;
            }
            int count = 0;
            List<float> tempList = new List<float>();
            Debug.WriteLine("ReadNumberVectorToList: Encoding: " + param2 + ", Amount of numbers read: " + paramList.Count);
            Debug.WriteLine("ReadNumberVectorToList: [ ");
            foreach (float value in paramList)
            {
                if (count < debug_length)
                {
                    tempList.Add(value);
                    count++;
                }
                else
                {
                    Debug.WriteLine(string.Join(' ', tempList));
                    tempList = new List<float>();
                    tempList.Add(value);
                    count = 1;
                }

            }
            Debug.WriteLine(string.Join(' ', tempList) + " ]");
            return paramList;
        }

        public static List<int> ReadVectorOfInt(ByteArray param1, int param2 = -1)
        {
            if (param1 == null)
            {
                return null;
            }
            int loc3 = (int)param1.Length;
            if (param2 == -1)
            {
                param2 = param1.ReadUnsignedByte();
                loc3--;
            }
            switch (param2)
            {
                case (int)Encoding.ENCODING_NONE:
                    loc3 /= 4;
                    break;
                case (int)Encoding.ENCODING_BYTE:
                case (int)Encoding.ENCODING_BYTE_SIGNED:
                case (int)Encoding.ENCODING_SHORT:
                case (int)Encoding.ENCODING_SHORT_SIGNED:
                    break;
                case (int)Encoding.UNENCODED_BYTE:
                case (int)Encoding.UNENCODED_BYTE_SIGNED:
                case (int)Encoding.UNENCODED_SHORT:
                case (int)Encoding.UNENCODED_SHORT_SIGNED:
                    loc3 /= 2;
                    break;
            }
            List<int> loc4 = new List<int>(loc3);
            int loc5 = 0;
            while (loc5 < loc3)
            {
                switch (param2)
                {
                    case (int)Encoding.ENCODING_NONE:
                        loc4.Insert(loc5, param1.ReadInt());
                        break;
                    case (int)Encoding.ENCODING_BYTE:
                    case (int)Encoding.ENCODING_BYTE_SIGNED:
                        loc4.Insert(loc5, param1.ReadUnsignedByte());
                        break;
                    case (int)Encoding.ENCODING_SHORT:
                    case (int)Encoding.ENCODING_SHORT_SIGNED:
                        loc4.Insert(loc5, param1.ReadByte());
                        break;
                    case (int)Encoding.UNENCODED_BYTE:
                    case (int)Encoding.UNENCODED_BYTE_SIGNED:
                        loc4.Insert(loc5, param1.ReadUnsignedShort());
                        break;
                    case (int)Encoding.UNENCODED_SHORT:
                    case (int)Encoding.UNENCODED_SHORT_SIGNED:
                        loc4.Insert(loc5, param1.ReadShort());
                        break;
                }
                loc5++;
            }
            int count = 0;
            List<int> tempList = new List<int>();
            Debug.WriteLine("ReadVectorOfInt: Encoding: " + param2 + ", Amount of numbers read: " + loc4.Count);
            Debug.WriteLine("ReadVectorOfInt: [ ");
            foreach (int value in loc4)
            {
                if (count < 3)
                {
                    tempList.Add(value);
                    count++;
                }
                else
                {
                    Debug.WriteLine(string.Join(' ', tempList));
                    tempList = new List<int>();
                    count = 0;
                }
            }
            Debug.WriteLine(" ]");
            return loc4;
        }
    }
}
