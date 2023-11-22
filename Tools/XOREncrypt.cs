using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Tools
{
    public class XOREncrypt
    {
        public static void Convert(string filePath, string exportPath, string keyA = "", string keyB = "")
        {
            Stream file = File.OpenRead(filePath);
            BinaryReader data = new BinaryReader(file);

            string fileName = Path.GetFileName(filePath);
            FileStream text = new FileStream(exportPath + "\\" + fileName, FileMode.OpenOrCreate);

            long length = file.Length;
            for(int i = 0; i < length; i++)
            {
                text.WriteByte((byte)(data.ReadByte() ^ (byte)keyA[i & 0x7F] ^ (byte)keyB[i % 57]));
            }

            text.Flush();
            text.Close();
        }
    }
}
