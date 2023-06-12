using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ionic.Zlib;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multi_Tool.Enumerators;
using System.Buffers.Binary;

namespace Utilities.Flash
{
    public class ByteArray
    {
        private int position = 0, length;
        public string endian = Endian.BIG_ENDIAN;
        public List<byte> data;

        public ByteArray(byte[] param1 = null) : base()
        {
            data = new List<byte>();
            if (param1 != null)
            {
                data.AddRange(param1);
            }
            position = 0;
            if (param1 != null)
            {
                length = data.Count;
            }
        }

        public void Clear()
        {
            data = new List<byte>();
            position = 0;
            length = 0;
        }

        public void Compress(string algorithm = null)
        {
            ByteArray tempData;
            switch (algorithm)
            {
                case "deflate":
                    break;
                case "lzma":
                    break;
                case "zlib":
                    MemoryStream inputStream = new MemoryStream(data.ToArray());
                    MemoryStream outputStream = new MemoryStream();
                    ZlibStream zLib = new ZlibStream(inputStream, CompressionMode.Compress);
                    zLib.CopyTo(outputStream);
                    data = new List<byte>();
                    data.AddRange(outputStream.ToArray());
                    length = (int)outputStream.Length;
                    position = 0;
                    break;
            }
        }

        public void Uncompress(string algorithm = CompressionAlgorithm.ZLIB)
        {
            try
            {
                MemoryStream outputStream;
                switch (algorithm)
                {
                    case CompressionAlgorithm.DEFLATE: // For Decompressing Data from DEFLATE streams.
                        outputStream = new MemoryStream();
                        DeflateStream deflate = new DeflateStream(outputStream, CompressionMode.Decompress, CompressionLevel.Default, true);
                        Debug.WriteLine("Decompressing bytes from current ByteArray, Data Length: " + data.Count);
                        deflate.Write(data.ToArray(), 0, data.Count);
                        Debug.WriteLine("Decompressed bytes from current ByteArray, writing new data.");
                        deflate.Flush();
                        data = new List<byte>();
                        data.AddRange(outputStream.GetBuffer());
                        length = (int)outputStream.Length;
                        position = 0;
                        deflate.Close();
                        break;
                    case CompressionAlgorithm.LZMA: // For Decompressing Data from LZMA streams.
                        break;
                    case CompressionAlgorithm.ZLIB: // For Decompressing Data from ZLIB streams.
                        outputStream = new MemoryStream();
                        ZlibStream zLib = new ZlibStream(outputStream, CompressionMode.Decompress, CompressionLevel.Default, true);
                        Debug.WriteLine("Decompressing bytes from current ByteArray, Data Length: " + data.Count);
                        zLib.Write(data.ToArray(), 0, data.Count);
                        Debug.WriteLine("Decompressed bytes from current ByteArray, writing new data.");
                        zLib.Flush();
                        data = new List<byte>();
                        data.AddRange(outputStream.GetBuffer());
                        length = (int)outputStream.Length;
                        position = 0;
                        zLib.Close();
                        break;
                }
            }
            catch(Exception e)
            {
                throw new Exception("ByteArray.Uncompress: Error uncompressing data - " + e);
            }
        }

        /// This upcoming section is for reading data from the ByteArray.

        public sbyte ReadByte()
        {
            sbyte temp = (sbyte)data[position];
            position++;
            return temp;
        }

        public byte ReadUnsignedByte()
        {
            byte temp = data[position];
            position++;
            return temp;
        }

        public byte[] ReadBytes(uint offset = 0, uint length = 0)
        {
            byte[] temp = new byte[length];
            try
            {
                for (int i = 0; i < length; i++)
                {
                    temp[i] = data[(int)(offset + position)];
                    position++;
                }
                return temp;
            }
            catch(Exception ex)
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read. " + ex);
            }
        }

        public double ReadDouble()
        {
            byte[] temp = new byte[8];
            if (data != null || position < data.Count)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 8;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadDoubleLittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadDoubleBigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public float ReadFloat()
        {
            if (data != null || position < data.Count)
            {
                byte[] temp = new byte[4];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 4;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadSingleLittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadSingleBigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public int ReadInt()
        {
            byte[] temp = new byte[4];
            if (data != null || position < data.Count)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 4;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadInt32LittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadInt32BigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public uint ReadUnsignedInt()
        {
            if (data != null || position < data.Count)
            {
                byte[] temp = new byte[4];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 4;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadUInt32LittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadUInt32BigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public short ReadShort()
        {
            if (data != null || position < data.Count)
            {
                byte[] temp = new byte[2];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 2;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadInt16LittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadInt16BigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public ushort ReadUnsignedShort()
        {
            if (data != null || position < data.Count)
            {
                byte[] temp = new byte[2];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += 2;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        return BinaryPrimitives.ReadUInt16LittleEndian(temp);
                    case Endian.BIG_ENDIAN:
                    default:
                        return BinaryPrimitives.ReadUInt16BigEndian(temp);
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public string ReadUTF()
        {
            byte[] temp = new byte[2];
            if (data != null || position < data.Count)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                ushort length;
                switch (endian)
                {
                    case Endian.LITTLE_ENDIAN:
                        length = BinaryPrimitives.ReadUInt16LittleEndian(temp);
                        break;
                    case Endian.BIG_ENDIAN:
                    default:
                        length = BinaryPrimitives.ReadUInt16BigEndian(temp);
                        break;
                }
                position += 2;
                temp = new byte[length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = data[position + i];
                }
                position += length;
                return Encoding.UTF8.GetString(temp);
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }
        public string ReadUTFBytes(uint length)
        {
            int tempPos = position;
            if (data != null || position < data.Count)
            {

                string tempStr = Encoding.UTF8.GetString(ReadBytes(0, length));
                string[] bigString = tempStr.Split('\0');
                position = tempPos;
                position += bigString[0].Length + 1;
                return bigString[0];
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public string ReadUTFBytes(int length)
        {
            int tempPos = position;
            if (data != null || position < data.Count)
            {

                string tempStr = Encoding.UTF8.GetString(ReadBytes(0, (uint)length));
                string[] bigString = tempStr.Split('\0');
                position = tempPos;
                position += bigString[0].Length + 1;
                return bigString[0];
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        public bool ReadBoolean()
        {
            int temp = data[position];
            if (data != null || position < data.Count)
            {
                position++;
                if (temp >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new EndOfStreamException("EOF ERROR: There is not enough sufficient data available to read");
            }
        }

        /// This upcoming section is for writing data to the ByteArray.

        public void WriteBoolean(bool value)
        {
            if (value)
            {
                data.Insert(position, 1);
            }
            else
            {
                data.Insert(position, 0);
            }
            position++;
        }

        public void WriteByte(byte value)
        {
            if (data != null)
            {
                data.Insert(position, value);
                position++;
            }
        }

        public void WriteBytes(byte[] bytes, uint offset = 0, uint length = 0)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                list.Insert((int)(offset + i), bytes[offset + i]);
            }
            data.InsertRange((int)(position - offset), list);
            position += list.Count;
        }

        public void WriteBytes(ByteArray bytes, uint offset = 0, int length = 0)
        {
            List<byte> tempBytes = new List<byte>(data.Count + (int)(bytes.length - offset));
            int specLength = length;
            if (specLength == 0)
            {
                specLength = bytes.length;
            }
            for (int i = 0; i < data.Count; i++)
            {
                tempBytes.Add(data[i]);
            }
            tempBytes.InsertRange((int)position, bytes.data);
            data = tempBytes;
            position += data.Count;
        }

        public void WriteFloat(float value)
        {
            byte[] valueData = new byte[4];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteSingleLittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteSingleBigEndian(valueData, value);
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteDouble(double value)
        {
            byte[] valueData = new byte[8];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteDoubleLittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteDoubleBigEndian(valueData, value);
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteInt(int value)
        {
            byte[] valueData = new byte[4];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteInt32LittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteInt32BigEndian(valueData, value);
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteUnsignedInt(uint value)
        {
            byte[] valueData = new byte[4];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteUInt32LittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteUInt32BigEndian(valueData, value);
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteShort(short value)
        {
            byte[] valueData = new byte[2];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteInt16LittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteInt16BigEndian(valueData, value);
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteShort(int value)
        {
            byte[] valueData = new byte[2];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteInt16LittleEndian(valueData, Convert.ToInt16(value));
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteInt16BigEndian(valueData, Convert.ToInt16(value));
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteShort(uint value)
        {
            byte[] valueData = new byte[2];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteInt16LittleEndian(valueData, Convert.ToInt16(value));
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteInt16BigEndian(valueData, Convert.ToInt16(value));
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteShort(long value)
        {
            byte[] valueData = new byte[2];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteInt16LittleEndian(valueData, Convert.ToInt16(value));
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteInt16BigEndian(valueData, Convert.ToInt16(value));
                    break;
            }
            data.InsertRange(position, valueData);
            position += valueData.Length;
        }

        public void WriteUnsignedShort(ushort value)
        {
            byte[] valueData = new byte[2];
            switch (endian)
            {
                case Endian.LITTLE_ENDIAN:
                    BinaryPrimitives.TryWriteUInt16LittleEndian(valueData, value);
                    break;
                case Endian.BIG_ENDIAN:
                default:
                    BinaryPrimitives.TryWriteUInt16BigEndian(valueData, value);
                    break;
            }
            List<byte> tempBytes;
            if (data != null)
            {
                tempBytes = new List<byte>(data.Count + valueData.Length);
                for (int i = 0; i < data.Count; i++)
                {
                    tempBytes.Add(data[i]);
                    position = data.Count;
                }
            }
            else
            {
                tempBytes = new List<byte>(valueData.Length);
                position = 0;
            }
            tempBytes.InsertRange((int)position, valueData);
            data = tempBytes;
            position += data.Count;
        }

        public void WriteUTF(string value)
        {
            byte[] valueData = Encoding.UTF8.GetBytes(value);
            byte[] stringLength = BitConverter.GetBytes(value.Length);
            List<byte> tempBytes;
            if (data != null)
            {
                tempBytes = new List<byte>(data.Count + valueData.Length);
                for (int i = 0; i < data.Count; i++)
                {
                    tempBytes.Add(data[i]);
                    position = data.Count;
                }
            }
            else
            {
                tempBytes = new List<byte>(valueData.Length);
                position = 0;
            }
            for (int i = 0; i < stringLength.Length; i++)
            {
                tempBytes.Add(stringLength[i]);
            }
            tempBytes.InsertRange((int)position, valueData);
            data = tempBytes;
            position += data.Count;
        }

        public void WriteUTFBytes(string value)
        {
            byte[] valueData = Encoding.UTF8.GetBytes(value);
            List<byte> tempBytes;
            if (data != null)
            {
                tempBytes = new List<byte>(data.Count + valueData.Length);
                for (int i = 0; i < data.Count; i++)
                {
                    tempBytes.Add(data[i]);
                    position = data.Count;
                }
            }
            else
            {
                tempBytes = new List<byte>(valueData.Length);
            }
            tempBytes.InsertRange((int)position, valueData);
            data = tempBytes;
            position += data.Count;
        }

        //

        public uint BytesAvailable
        {
            get { return (uint)(data.Count - position); }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public int Position
        {
            get { return  position; }
            set { position = value; }
        }
    }
}
