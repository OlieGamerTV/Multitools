using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ionic.Zlib;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using Utilities.Flash;
using System.CodeDom;
using Multi_Tool.Enumerators;

namespace Multi_Tool.Tools
{
    internal class DPack
    {
        private ByteArray? fileData;
        private ListView exportedText;
        CompressionTool compressTool = null;
        private bool makeDataCopy, toDecomp, exportFiles;
        private string[] compressionAlgos;
        private string? filePath, fileName, exportPath;
        private const uint magic_code = 0x4B415044, ZLIB_CODE = 0x789C;

        public DPack(ListView outList = null) : base()
        {
            exportedText = outList;
            compressTool = new CompressionTool(outList);
        }

        public void StartTool(ListView listView, bool unpack)
        {
            if (unpack)
            {
                if(filePath == null)
                {
                    GetDir(true);
                }
                if(exportPath == null)
                {
                    GetDir(false);
                }
                UnpackToFile(fileData.data.ToArray(), makeDataCopy);
            }
            else
            {
                Dictionary<string, ByteArray> files = GetMultipleDir();
                List<DPackItem> list = new List<DPackItem>();
                for(int i = 0; i < files.Count; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files.Keys.ToArray()[i]);
                    DPackItem item = new DPackItem(name, files[files.Keys.ToArray()[i]]);
                    list.Add(item);
                }
                Pack(list.ToArray(), true, false);
            }
        }

        public Dictionary<string, byte[]> UnpackToFile(byte[] param1, bool param2 = true, bool createExportDir = true)
        {
            byte[]? temp = null, bytes = null, packedBytes = param1;
            bool uncompressSuccess = false, makeCopy = param2;
            uint magic = 0, numFiles = 0;
            int i = 0, t = 0;
            Dictionary<string, byte[]>? items = null;
            uint[]? lengths = null;
            string[]? names = null;
            if (packedBytes == null)
            {
                return null;
            }
            if (makeCopy)
            {
                temp = new byte[param1.Length];
                temp = param1;
                packedBytes = temp;
            }
            try
            {
                Stream readStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                MemoryStream stream = new MemoryStream();
                if (toDecomp)
                {
                    ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Decompress);
                    zlib.CopyTo(stream);
                }
                else
                {

                }
                packedBytes = stream.GetBuffer();
                BinaryReader binRead = new BinaryReader(readStream, System.Text.Encoding.Default);
                if (packedBytes == null)
                {
                    Debug.WriteLine("DPack.unpack: Error. Couldn't uncompress DPack");
                    WriteToOutput("DPack.unpack: Couldn't uncompress DPack");
                    return null;
                }
                stream.Write(packedBytes);
                magic = binRead.ReadUInt32();
                Debug.WriteLine(magic.ToString());
                if (magic != magic_code)
                {
                    return null;
                }
                WriteToOutput("Magic Code - " + magic);
                stream.Position += 4;
                numFiles = binRead.ReadUInt16();
                Debug.WriteLine(numFiles.ToString());
                stream.Position += 2;
                lengths = new uint[numFiles];
                i = 0;
                t = 6;
                while (i < numFiles)
                {
                    lengths[i] = binRead.ReadUInt32();
                    Debug.WriteLine(lengths[i].ToString());
                    t += 4;
                    i++;
                }
                names = new string[numFiles];
                i = 0;
                while (i < numFiles)
                {
                    ushort charLength = binRead.ReadUInt16();
                    names[i] = Encoding.UTF8.GetString(binRead.ReadBytes(charLength));
                    WriteToOutput("Data Chunk Name - " + names[i] + " | Data Length - " + lengths[i]);
                    Debug.WriteLine(names[i]);
                    t += 3;
                    i++;
                }
                WriteToOutput(names.Length + " data points found: " + string.Join(',', names));
                items = new Dictionary<string, byte[]>();
                i = 0;
                while (i < numFiles)
                {
                    bytes = new byte[lengths[i]];
                    bytes = binRead.ReadBytes((int)lengths[i]);
                    items[names[i]] = bytes;
                    string ext = GetFileExt(bytes, names, i);
                    Stream writeStream;
                    writeStream = File.Open(exportPath + "/" + names[i] + ext, FileMode.Create);
                    writeStream.Write(bytes);
                    WriteToOutput("File " + names[i] + ext + " successfully exported.");
                    t += (int)lengths[i];
                    Debug.WriteLine(BitConverter.ToString(bytes));
                    i++;
                }
                return items;
            }
            catch (Exception err)
            {
                Debug.WriteLine("DPack.unpack: Error unpacking pack file. Error = " + err);
                WriteToOutput("DPack.unpack: Error unpacking pack file.");
                WriteToOutput("Error = " + err);
                return null;
            }
        }

        public Dictionary<string, ByteArray> Unpack(ByteArray param1, bool param2 = true)
        {
            ByteArray? temp = null, bytes = null, packedBytes = param1;
            bool uncompressSuccess = false, makeCopy = param2;
            uint magic = 0, numFiles = 0;
            int i = 0, t = 0;
            Dictionary<string, ByteArray>? items = null;
            uint[]? lengths = null;
            string[]? names = null;
            if (packedBytes == null)
            {
                WriteToOutput("DPack.unpack: Error. No data detected.");
                return null;
            }
            if (makeCopy)
            {
                temp = param1;
                packedBytes = temp;
            }
            try
            {
                packedBytes.endian = Endian.LITTLE_ENDIAN;
                if (packedBytes == null)
                {
                    Debug.WriteLine("DPack.unpack: Error. Couldn't uncompress DPack");
                    WriteToOutput("DPack.unpack: Error. Couldn't uncompress DPack");
                    return null;
                }
                magic = packedBytes.ReadUnsignedShort();
                WriteToOutput("ZLib Check - " + magic);
                Debug.WriteLine(magic.ToString());
                if (magic == ZLIB_CODE)
                {
                    packedBytes.Position -= 2;
                    WriteToOutput("DPack.unpack: Unpacking file...");
                    Debug.WriteLine("DPack.unpack: Unpacking file...");
                    packedBytes.Uncompress();
                    WriteToOutput("DPack.unpack: Uncompressed file.");
                    Debug.WriteLine("DPack.unpack: Uncompressed file...");
                    magic = packedBytes.ReadUnsignedInt();
                    WriteToOutput("Unpacked Magic Code - " + magic);
                    Debug.WriteLine(magic.ToString());
                }
                else
                {
                    WriteToOutput("DPack.unpack: File is already unpacked. Continuing...");
                    Debug.WriteLine("DPack.unpack: File is already unpacked. Continuing...");
                    packedBytes.Position -= 2;
                    magic = packedBytes.ReadUnsignedInt();
                    WriteToOutput("Unpacked Magic Code - " + magic);
                    Debug.WriteLine(magic.ToString());
                }
                if (magic != magic_code)
                {
                    WriteToOutput("DPack.unpack: Error. Invalid Magic Code returned: " + magic);
                    return null;
                }
                numFiles = packedBytes.ReadUnsignedShort();
                Debug.WriteLine("Number of files: " + numFiles.ToString());
                lengths = new uint[numFiles];
                i = 0;
                t = 6;
                while (i < numFiles)
                {
                    lengths[i] = packedBytes.ReadUnsignedInt();
                    t += 4;
                    i++;
                }
                names = new string[numFiles];
                i = 0;
                while (i < numFiles)
                {
                    names[i] = packedBytes.ReadUTF();
                    Debug.WriteLine("Data Chunk Name - " + names[i] + ", Data Length - " + lengths[i]);
                    WriteToOutput("Data Chunk Name - " + names[i] + ", Data Length - " + lengths[i]);
                    t += 3;
                    i++;
                }
                WriteToOutput(names.Length + " data points found: " + string.Join(',', names));
                items = new Dictionary<string, ByteArray>();
                i = 0;
                while (i < numFiles)
                {
                    bytes = new ByteArray(packedBytes.ReadBytes(0, lengths[i]));
                    bytes.wrapAround = true;
                    items[names[i]] = bytes;
                    t += (int)lengths[i];
                    Debug.WriteLine("Item " + i + ": " + BitConverter.ToString(bytes.data.ToArray()));
                    i++;
                }
                Debug.WriteLine("DPack.unpack: Unpacking has been successful.");
                WriteToOutput("DPack.unpack: Unpacking has been successful.");
                return items;
            }
            catch (Exception err)
            {
                WriteToOutput("DPack.unpack: Error unpacking pack file.");
                WriteToOutput("Error = " + err);
                throw new Exception("DPack.unpack: Error unpacking pack file. Error = " + err);
            }
        }

        public byte[] Pack(object[] array, bool writeFile, bool compress)
        {
            try
            {
                if (array == null)
                {
                    return null;
                }
                MemoryStream temp = new MemoryStream();
                ZlibStream zLib = new ZlibStream(temp, CompressionMode.Compress);
                BinaryWriter binary;
                binary = new BinaryWriter(temp);
                byte[] loc3 = null;
                binary.Write(magic_code);
                WriteToOutput("Magic Code: " + magic_code + " written to memory.");
                binary.Write((short)array.Length);
                WriteToOutput("Total Length : " + (short)array.Length + " written to memory.");
                int i = 0;
                foreach (DPackItem loc2 in array)
                {
                    if (loc2 == null)
                    {
                        Debug.WriteLine("DPack.pack: Invalid item");
                        return null;
                    }
                    binary.Write((uint)loc2.GetBytes().Length);
                    WriteToOutput("Data Length of file # " + i + " : " + (short)array.Length + " written to memory.");
                }
                foreach (DPackItem loc2 in array)
                {
                    short length = (short)loc2.GetName().Length;
                    binary.Write(length);
                    binary.Write(Encoding.UTF8.GetBytes(loc2.GetName()));
                }
                foreach (DPackItem loc2 in array)
                {
                    binary.Write(loc2.GetBytes().data.ToArray());
                }
                loc3 = temp.ToArray();
                zLib.Dispose();
                zLib.Close();
                temp.Dispose();
                temp.Close();
                if (writeFile)
                {
                    if (exportPath == null)
                    {
                        GetDir(false);
                    }
                    FileStream writeStream = File.Open(exportPath + "/" + "packed.dpak", FileMode.Create);
                    writeStream.Write(loc3);
                    writeStream.Flush();
                    writeStream.Dispose();
                    writeStream.Close();
                    WriteToOutput("File 'packed.dpak' successfully created.");
                }
                return loc3;
            }
            catch (Exception err)
            {
                WriteToOutput("DPack.unpack: Error unpacking pack file.");
                WriteToOutput("Error = " + err);
                throw new Exception("DPack.unpack: Error packing file. Error = " + err);
            }
        }

        private string GetFileExt(byte[] param1, string[] param2, int index)
        {
            switch (param2[index])
            {
                //TPK Data Identifiers
                case "t":
                    return ".jxr";
                case "j":
                case "m":
                    return ".json";

                //BKV Data Identifiers
                case "Color-0":
                case "Color-1":
                case "Toon-0":
                case "Toon-1":
                    if (param1[0] == 80 && param1[1] == 86 && param1[2] == 82)
                    {
                        return ".pvr";
                    }
                    else if (param1[0] == 137 && param1[1] == 80 && param1[2] == 78 && param1[3] == 71)
                    {
                        return ".png";
                    }
                    else
                    {
                        return "";
                    }
                case "desc":
                    return ".bkv";
                default:
                    return "";
            }
        }

        private void WriteToOutput(object entry)
        {
            if (entry != null && exportedText != null)
            {
                ListViewItem item = new ListViewItem();
                item.Content = entry;
                exportedText.Items.Add(item);
            }
        }

        public void GetDir(bool isFile)
        {
            
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            if (isFile)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.CheckPathExists = true;
                fileDialog.Title = "Open";
                fileDialog.Filter = "Disney / Alpine Asset Packages (*.tpk;*.mmf;*.mmt;*.mma;*.dpak)|*.tpk;*.mmf;*.mmt;*.mma;*.dpak|All Files (*.*)|*.*";
                fileDialog.ValidateNames = true;
                fileDialog.CheckFileExists = true;
                fileDialog.FileName = "Select the compressed file you want to extract embedded files from.";
                // Code for looking for a file.
                if (fileDialog.ShowDialog() == true)
                {
                    string path = Path.GetFullPath(fileDialog.FileName);
                    Debug.WriteLine("Selected path: " + path);
                    string file = Path.GetFileNameWithoutExtension(path);
                    Debug.WriteLine("Selected file: " + file);
                    fileName = file;
                    filePath = path;
                    Debug.WriteLine("Selected file to rip set to: " + path);
                    fileData = new ByteArray(File.ReadAllBytes(path));
                }
                else
                {
                    Debug.WriteLine("No file selected or file does not exist!");
                }
            }
            else
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = "Disney / Alpine Asset Packages (*.tpk;*.mmf;*.mmt;*.mma;*.dpak)|*.tpk;*.mmf;*.mmt;*.mma;*.dpak";
                fileDialog.Title = "Export";
                fileDialog.FileName = "Enter the folder you want to export to and press Open.";
                // Code for looking for a export path.
                if (fileDialog.ShowDialog() == true)
                {
                    string? path = Path.GetDirectoryName(fileDialog.FileName);
                    Debug.WriteLine("Selected export path: " + path);
                    exportPath = path;
                }
                else
                {
                    Debug.WriteLine("Selected directory does not exist!");
                }
            }
        }

        public Dictionary<string, ByteArray> GetMultipleDir()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckPathExists = true;
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            fileDialog.Title = "Export";
            fileDialog.ValidateNames = false;
            fileDialog.Multiselect = true;
            fileDialog.CheckFileExists = false;
            fileDialog.FileName = "Select the files you want to package to and press Open.";
            Dictionary<string, ByteArray> files = new Dictionary<string, ByteArray>();
            // Code for looking for a export path.
            if (fileDialog.ShowDialog() == true)
            {
                
                foreach (string file in fileDialog.FileNames)
                {
                    ByteArray array = new ByteArray(File.ReadAllBytes(file));
                    files.Add(file, array);
                }
                Debug.WriteLine("Selected export path: " + fileDialog.FileNames[0]);
                Debug.WriteLine("Selected files: " + string.Join(", ", fileDialog.SafeFileNames));
            }
            else
            {
                Debug.WriteLine("Selected directory does not exist!");
            }
            return files;
        }

        public string GetCurrentExportDir
        {
            get { return exportPath; }
        }

        public string GetCurrentFileName
        {
            get { return fileName; }
        }
    }
}
