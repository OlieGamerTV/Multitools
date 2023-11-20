using Multi_Tool.Utilities;
using Multi_Tool.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace Multi_Tool.Tools
{
    internal class SERFExtractor
    {
        int fileAmount;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        OutputLog output;
        bool saveChar, isRipRunning;
        private byte[] dataText;
        string exportPath, fileName, filePath, desiredFileType = "";
        private uint magic_code = 1179796819;

        public SERFExtractor(ListView list = null)
        {
            if(list != null)
            {
                output = new OutputLog(list);

            }
        }

        public void RipSerfFile()
        {
            if (exportPath == null)
            {
                Debug.WriteLine("Export Path returned null, Stopping Coroutine to prevent problems.");
                return;
            }
            Stream readStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binRead = new BinaryReader(readStream, Encoding.Default);
            try
            {
                uint magic = binRead.ReadUInt32();
                if (magic != magic_code)
                {
                    output.WriteToOutput("Header Code doesn't match Magic Code, stopping export.");
                    return;
                }
                Debug.WriteLine("Header Code matches Magic Code, checking Data Chunks...");
                output.WriteToOutput("Data Chunks:");
                byte[] tempHead;
                byte[] temp;
                int unknown0x04 = binRead.ReadInt32(), unknown0x08 = binRead.ReadInt32();
                fileAmount = binRead.ReadInt32();
                int unknown0x10 = binRead.ReadInt32();
                int offset = 0, dataSize = 0;
                int i = 0;
                Mouse.OverrideCursor = Cursors.AppStarting;
                // This loop searches for a data specification chunk, copies the data the spec chunk details and then moves to the next one.
                while (i < fileAmount)
                {
                    binRead.BaseStream.Position += 256;
                    offset = binRead.ReadInt32(); dataSize = binRead.ReadInt32();
                    int dupeSize = binRead.ReadInt32();
                    long backupOffset = binRead.BaseStream.Position;
                    Debug.WriteLine("Data Chunk Located");
                    output.WriteToOutput("Data Chunk " + i + " (Offset = " + offset + ", Data Size = " + dataSize + ", Dupe Data Size = " + dupeSize + ")");
                    binRead.BaseStream.Position = offset;
                    tempHead = binRead.ReadBytes(16);
                    temp = binRead.ReadBytes(dataSize - 16);
                    IdentifyExt(tempHead);
                    if (!Directory.Exists(exportPath + "/" + fileName + "/" + desiredFileType))
                    {
                        Directory.CreateDirectory(exportPath + "/" + fileName + "/" + desiredFileType);
                    }
                    FileStream writer = new FileStream(exportPath + "/" + fileName + "/" + desiredFileType + "/export_" + i + "." + desiredFileType, FileMode.Create);
                    output.WriteToOutput("export_" + i + "." + desiredFileType + " exported successfully.");
                    writer.Write(tempHead);
                    writer.Write(temp);
                    writer.Flush();
                    writer.Close();
                    binRead.BaseStream.Position = backupOffset;
                    i++;
                }
                Mouse.OverrideCursor = null;
                binRead.Close();
                readStream.Close();
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                output.WriteToOutput("Error while reading/writing from the SERF file. Error: " + ex);
            }
        }

        private void IdentifyExt(byte[] sample)
        {
            string temp = BitConverter.ToString(sample);
            if (temp.Contains(SERFInputNames.MAGIC_RIFF))
            {
                if (temp.Contains(SERFInputNames.MAGIC_WAV))
                {
                    desiredFileType = "wav";
                }
                else if (temp.Contains(SERFInputNames.MAGIC_XWMA))
                {
                    desiredFileType = "xwma";
                }
                else if (temp.Contains(SERFInputNames.MAGIC_AVI))
                {
                    desiredFileType = "avi";
                }
            }
            else if (temp.Contains(SERFInputNames.MAGIC_PNG))
            {
                desiredFileType = "png";
            }
            else if (temp.Contains(SERFInputNames.MAGIC_JPG) || temp.Contains(SERFInputNames.MAGIC_JPG_EXIF) 
                || temp.Contains(SERFInputNames.MAGIC_JPG_CIFF) || temp.Contains(SERFInputNames.MAGIC_JPG_SPIFF))
            {
                desiredFileType = "jpg";
            }
            else if (temp.Contains(SERFInputNames.MAGIC_BMP))
            {
                desiredFileType = "bmp";
            }
            else if (temp.Contains(SERFInputNames.MAGIC_XML))
            {
                desiredFileType = "xml";
            }
            else
            {
                desiredFileType = "unknown";
            }
            Debug.WriteLine(desiredFileType);
        }

        public void PassData(string export, string file, string originalPath, byte[] fileData)
        {
            if (export != null)
            {
                exportPath = export;
            }
            if (file != null)
            {
                fileName = file;
            }
            if (originalPath != null)
            {
                filePath = originalPath;
            }
            if (fileData != null)
            {
                dataText = fileData;
            }
        }
    }
}
