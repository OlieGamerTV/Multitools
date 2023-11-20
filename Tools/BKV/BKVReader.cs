using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Windows.Controls;
using Microsoft.Win32;
using Multi_Tool.Utilities;
using Utilities.Flash;
using System.Xml;
using System.Windows.Markup;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml.XPath;
using Multi_Tool.Enumerators;

namespace Multi_Tool.Tools.BKV
{
    internal class BKVReader
    {
        public const int version = 0;
        private const int header = 608324438, header_inv = 1447772708, key_mask = 32768;
        private ByteArray __data, fileData;
        private string fileName;
        string exportPath;
        private int __stringPoolIndex, dataPos = 0;
        Dictionary<int, List<BKVValue>> arrays = null;
        Dictionary<int, BKVTable> tables = null;
        Dictionary<int, string> strings = null;
        BKVTable rootTable;
        OutputLog output = new OutputLog();

        public BKVReader(ListView list = null)
        {
            if (list != null)
            {
                output = new OutputLog(list);
            }
        }

        public bool IsBKV(byte[] param1)
        {
            if (param1.Length < 4)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            uint loc2 = (uint)dataPos;
            dataPos = 0;
            int loc3 = BitConverter.ToInt32(param1);
            Debug.WriteLine(loc3);
            return loc3 == header || loc3 == header_inv;
        }

        public bool IsBKV(ByteArray param1)
        {
            if (param1.Length < 4)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            int loc3 = param1.ReadInt();
            param1.Position = 0;
            Debug.WriteLine(loc3);
            return loc3 == header || loc3 == header_inv;
        }

        public void GetFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                string path = fileDialog.FileName;
                fileName = Path.GetFileNameWithoutExtension(path);
                fileData = new ByteArray(File.ReadAllBytes(path));
                IsBKV(fileData);
            }
        }

        private void GetExportPath()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "XML File (*.xml)|*.xml|JSON File (*.json)|*.json|No File (*.*)|*.*";
            fileDialog.Title = "Export As";
            fileDialog.FileName = "bkv_export";
            // Code for looking for a export path.
            if (fileDialog.ShowDialog() == true)
            {
                string? path = Path.GetFullPath(fileDialog.FileName);
                Debug.WriteLine("Selected export path: " + path);
                exportPath = path;
            }
            else
            {
                Debug.WriteLine("Selected directory does not exist!");
                exportPath = "";
            }
        }

        public void StartRun(bool toExport = false)
        {
            if (fileData == null)
            {
                GetFile();
            }
            if (!toExport)
            {
                LoadBytes(fileData.data.ToArray());
                return;
            }
            GetExportPath();
            string tempExt = Path.GetExtension(exportPath.ToLower());
            switch (tempExt)
            {
                case (".xml"):
                    {
                        WriteBytesToXML(fileData.data.ToArray());
                        return;
                    }
                case (".json"):
                    {
                        WriteBytesToJson(fileData.data.ToArray());
                        return;
                    }
                default:
                    {
                        LoadBytes(fileData.data.ToArray());
                        return;
                    }
            }
        }

        public bool LoadBytes(byte[] param1)
        {
            output.ClearLog();
            MemoryStream memory = new MemoryStream(param1);
            string loc11 = null, loc23 = null;
            uint loc12 = 0, loc13 = 0, loc14 = 0, loc15 = 0, loc19 = 0, loc21 = 0, loc22 = 0, loc24 = 0;
            int loc18 = 0, loc26 = 0;
            List<BKVValue> loc16 = null, loc17 = null;
            BKVTable loc20 = null;
            BKVValue loc25 = new BKVValue();
            if (param1 == null || param1.Length < 10)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            ///memory.Write(param1);
            BinaryReader reader = new BinaryReader(memory);
            __data = new ByteArray(param1);
            int loc2 = reader.ReadInt32();
            if (loc2 != header && loc2 != header_inv)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file header does not match.");
                return false;
            }
            output.WriteToOutput(loc2);
            int loc3 = reader.ReadSByte();
            if (loc3 != version)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, version does not match.");
                return false;
            }
            output.WriteToOutput(loc3);
            output.WriteToOutput(reader.ReadByte());
            uint loc4 = 0;
            uint loc5 = loc4 = reader.ReadUInt32();
            uint loc6 = 0;
            Debug.WriteLine("__stringPoolIndex: " + __stringPoolIndex);
            uint loc7 = loc6 = (uint)(this.__stringPoolIndex = (int)reader.BaseStream.Position);
            Debug.WriteLine("__stringPoolIndex [After Stream Position: " + __stringPoolIndex);
            strings = new Dictionary<int, string>();
            output.WriteToOutput(loc4, loc5, loc6, loc7);
            int i = 0;
            int[] offset = new int[loc5];
            offset[i] = 0;
            while (loc5 > 0)
            {
                loc11 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc5));
                string[] bigString = loc11.Split('\0');
                loc11 = bigString[0];
                offset[i + 1] = offset[i] + loc11.Length + 1;
                Debug.WriteLine(loc11);
                Debug.WriteLine(loc11.Length);
                strings[offset[i]] = loc11;
                Debug.WriteLine("strings.Values: " + strings.GetValueOrDefault(offset[i]));
                loc12 = (uint)loc11.Length;
                output.WriteToOutput("String: " + loc11 + " | Offset: " + offset[i]);
                loc5 -= loc12 + 1;
                Debug.WriteLine("loc5 (UINT) " + loc5);
                Debug.WriteLine("loc5 (Converted to INT) " + (int)loc5);
                loc7 += loc12 + 1;
                Debug.WriteLine("loc7 (UINT) " + loc7);
                i++;
                reader.BaseStream.Position = loc7;
            }
            uint loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc9 = (uint)reader.BaseStream.Position;
            arrays = new Dictionary<int, List<BKVValue>>();
            if (loc9 != loc8)
            {
                while (reader.BaseStream.Position < loc8)
                {
                    loc13 = (uint)reader.BaseStream.Position;
                    loc14 = reader.ReadUInt16();
                    output.WriteToOutput("Loc14: " + loc14);
                    loc15 = reader.ReadByte();
                    output.WriteToOutput("Loc15: " + loc15);
                    loc16 = new List<BKVValue>((int)loc14);
                    loc17 = new List<BKVValue>((int)loc14);
                    loc18 = 0;
                    while (loc18 < loc14)
                    {
                        loc17[loc18] = new BKVValue((int)loc15, param1, new WeakReference(this));
                        loc18++;
                    }
                    arrays[(int)(loc13 - loc9)] = loc17;
                }
            }
            loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc10 = (uint)reader.BaseStream.Position;
            tables = new Dictionary<int, BKVTable>();
            i = 0;
            Debug.WriteLine("reader Position (Before Tables Reading): " + reader.BaseStream.Position);
            while (reader.BaseStream.Position < loc8)
            {
                output.WriteToOutput("--------------------------- Bytes Read: " + i);
                loc19 = (uint)reader.BaseStream.Position;
                loc20 = new BKVTable();
                Debug.WriteLine("reader Position (Before UINT16 1st Read): " + reader.BaseStream.Position);
                loc21 = reader.ReadUInt16();
                i += 2;
                Debug.WriteLine("reader Position (After UINT16 1st Read): " + reader.BaseStream.Position);
                loc18 = 0;
                while (loc18 < loc21)
                {
                    loc22 = reader.ReadUInt16();
                    i += 2;
                    Debug.WriteLine("reader Position (After UINT16 2nd Read): " + reader.BaseStream.Position);
                    loc23 = "null";
                    Debug.WriteLine("Loc22 & key_mask: " + (loc22 & key_mask));
                    if ((loc22 & key_mask) == 0)
                    {
                        if (!strings.ContainsKey((int)loc22))
                        {
                            loc26 = (int)reader.BaseStream.Position;
                            loc23 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc4 - (int)loc22));
                            string[] bigString = loc11.Split('\0');
                            loc23 = bigString[0];
                            reader.BaseStream.Position = loc26;
                        }
                        else
                        {
                            loc23 = strings[(int)loc22];
                        }
                    }
                    loc24 = reader.ReadByte();
                    i += 1;
                    Debug.WriteLine(loc24);
                    switch (loc24)
                    {
                        case 0: // Bool - False
                            output.WriteToOutput(loc23 + " : " + "Bool - False.");
                            break;
                        case 1: // Bool - True
                            output.WriteToOutput(loc23 + " : " + "Bool - True.");
                            break;
                        case 2: // Float
                            i += 4;
                            output.WriteToOutput(loc23 + " : " + reader.ReadSingle());
                            break;
                        case 3: // Byte
                            i += 1;
                            output.WriteToOutput(loc23 + " : " + reader.ReadByte());
                            break;
                        case 4: // Short
                            i += 2;
                            output.WriteToOutput(loc23 + " : " + reader.ReadUInt16());
                            break;
                        case 5: // Integer
                            i += 4;
                            output.WriteToOutput(loc23 + " : " + reader.ReadUInt32());
                            break;
                        case 6: // String
                            i += 2;
                            output.WriteToOutput(loc23 + " : " + strings[reader.ReadUInt16()]);
                            break;
                        case 7: // Table
                            i += 2;
                            ushort tableOffset = reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + "[Table], Data offset - " + tableOffset);
                            break;
                        case 8: // Array
                            i += 2;
                            reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + "[Array]");
                            break;
                    }
                    loc25 = new BKVValue((int)loc24, param1, new WeakReference(this));
                    Debug.WriteLine(loc25);
                    loc20.AddPair(loc23, loc25);
                    loc18++;
                }
                tables[(int)(loc19 - loc10)] = loc20;
                output.WriteToOutput("Table written to: " + (loc19 - loc10));
                if (rootTable == null)
                {
                    rootTable = loc20;
                }
            }
            reader.Close();
            Debug.WriteLine("BKV Reader: Loading has returned true!");
            output.WriteToOutput("------------------------ Total Bytes Read: " + i);
            output.WriteToOutput("BKV Reader: Loading has returned true!");
            Debug.WriteLine("Tables: " + tables.Count);
            return true;
        }

        public bool Load(ByteArray param1)
        {
            Debug.WriteLine("BKV Reader: Starting Loading...");
            output.WriteToOutput("--------------------------------------------", "BKV Reader: Starting loading...");
            string loc11 = null, loc23 = null;
            uint loc12 = 0, loc13 = 0, loc14 = 0, loc15 = 0, loc19 = 0, loc21 = 0, loc22 = 0, loc24 = 0;
            int loc18 = 0, loc26 = 0;
            List<BKVValue> loc16 = null, loc17 = null;
            BKVTable loc20 = null;
            BKVValue loc25 = new BKVValue();
            if (param1 == null || param1.Length < 10)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            int loc2 = param1.ReadInt();
            Debug.WriteLine("BKV File Header: " + loc2);
            if (loc2 != header && loc2 != header_inv)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file header does not match.");
                return false;
            }
            output.WriteToOutput("BKV File Header: " + loc2);
            int loc3 = param1.ReadByte();
            if (loc3 != version)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, version does not match.");
                return false;
            }
            if (loc2 == header_inv)
            {
                param1.endian = param1.endian == Endian.BIG_ENDIAN ? Endian.LITTLE_ENDIAN : Endian.BIG_ENDIAN;
            }
            output.WriteToOutput("BKV File Version: " + loc3, param1.ReadUnsignedByte());
            uint loc4 = 0;
            uint loc5 = loc4 = param1.ReadUnsignedInt();
            uint loc6 = 0;
            Debug.WriteLine("__stringPoolIndex: " + __stringPoolIndex);
            uint loc7 = loc6 = (uint)(this.__stringPoolIndex = (int)param1.Position);
            Debug.WriteLine("__stringPoolIndex [After Stream Position: " + __stringPoolIndex);
            strings = new Dictionary<int, string>();
            output.WriteToOutput("loc4: " + loc4, "loc5: " + loc5, "loc6: " + loc6, "loc7: " + loc7);
            Debug.WriteLine("loc4: " + loc4 + ", loc5: " + loc5 + ", loc6: " + loc6 + ", loc7: " + loc7);
            int i = 0;
            int[] offset = new int[loc5];
            offset[i] = 0;
            while (loc5 > 0)
            {
                Debug.WriteLine("loc4: " + loc4 + ", loc5: " + loc5 + ", loc6: " + loc6 + ", loc7: " + loc7);
                loc11 = param1.ReadUTFBytes(loc5);
                Debug.WriteLine(loc11);
                strings[(int)(loc7 - loc6)] = loc11;
                loc12 = (uint)loc11.Length + 1;
                loc5 -= loc12;
                loc7 += loc12;
                param1.Position = (int)loc7;
            }
            int loc8 = (int)(param1.Position + param1.ReadUnsignedInt());
            int loc9 = param1.Position;
            arrays = new Dictionary<int, List<BKVValue>>();
            if (loc9 != loc8)
            {
                while (param1.Position < loc8)
                {
                    loc13 = (uint)param1.Position;
                    loc14 = param1.ReadUnsignedShort();
                    output.WriteToOutput("Loc14: " + loc14);
                    loc15 = param1.ReadUnsignedByte();
                    output.WriteToOutput("Loc15: " + loc15);
                    loc16 = new List<BKVValue>((int)loc14);
                    loc17 = new List<BKVValue>((int)loc14);
                    loc18 = 0;
                    while (loc18 < loc14)
                    {
                        loc17[loc18] = new BKVValue((int)loc15, param1, new WeakReference(this, true));
                        loc18++;
                    }
                    arrays[(int)(loc13 - loc9)] = loc17;
                }
            }
            loc8 = (int)(param1.Position + param1.ReadUnsignedInt());
            int loc10 = param1.Position;
            tables = new Dictionary<int, BKVTable>();
            i = 0;
            Debug.WriteLine("reader Position (Before Tables Reading): " + param1.Position);
            while (param1.Position < loc8)
            {
                output.WriteToOutput("--------------------------- Bytes Read: " + i);
                loc19 = (uint)param1.Position;
                loc20 = new BKVTable();
                Debug.WriteLine("reader Position (Before UINT16 1st Read): " + param1.Position);
                loc21 = param1.ReadUnsignedShort();
                i += 2;
                Debug.WriteLine("reader Position (After UINT16 1st Read): " + param1.Position);
                loc18 = 0;
                while (loc18 < loc21)
                {
                    loc22 = param1.ReadUnsignedShort();
                    i += 2;
                    Debug.WriteLine("reader Position (After UINT16 2nd Read): " + param1.Position);
                    loc23 = string.Empty;
                    Debug.WriteLine("Loc22 & key_mask: " + (loc22 & key_mask));
                    if ((loc22 & key_mask) == 0)
                    {
                        if (!strings.ContainsKey((int)loc22))
                        {
                            Debug.WriteLine("Name not found in strings, obtaining from position: " + (loc4 - loc22));
                            loc26 = (int)param1.Position;
                            param1.Position = (int)(loc6 + loc22);
                            loc23 = param1.ReadUTFBytes(loc4 - loc22);
                            param1.Position = loc26;
                        }
                        else
                        {
                            loc23 = strings[(int)loc22];
                        }
                    }
                    loc24 = param1.ReadUnsignedByte();
                    output.WriteToOutput("Attribute Name: " + loc23 + ", Value: " + loc24);
                    i += 1;
                    Debug.WriteLine(loc24);
                    loc25 = new BKVValue((int)loc24, param1, new WeakReference(this, true));
                    Debug.WriteLine(loc25);
                    loc20.AddPair(loc23, loc25);
                    loc18++;
                }
                tables[(int)(loc19 - loc10)] = loc20;
                if (this.rootTable == null)
                {
                    rootTable = loc20;
                }
            }
            Debug.WriteLine("BKV Reader: Loading has returned true!");
            output.WriteToOutput("------------------------ Total Bytes Read: " + i, "BKV Reader: Loading has returned true!");
            Debug.WriteLine("Tables: " + tables.Count);
            return true;
        }

        public bool WriteBytesToXML(byte[] param1)
        {
            if (exportPath == "")
            {
                return false;
            }
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(exportPath, settings);
            output.ClearLog();
            MemoryStream memory = new MemoryStream(param1);
            string loc11 = null, loc23 = null;
            uint loc12 = 0, loc13 = 0, loc14 = 0, loc15 = 0, loc19 = 0, loc21 = 0, loc22 = 0, loc24 = 0;
            int loc18 = 0, loc26 = 0;
            List<BKVValue> loc16 = null, loc17 = null;
            BKVTable loc20 = null;
            BKVValue loc25 = new BKVValue();
            if (param1 == null || param1.Length < 10)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            ///memory.Write(param1);
            BinaryReader reader = new BinaryReader(memory);
            __data = new ByteArray(param1);
            int loc2 = reader.ReadInt32();
            if (loc2 != header && loc2 != header_inv)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file header does not match.");
                return false;
            }
            output.WriteToOutput(loc2);
            int loc3 = reader.ReadSByte();
            if (loc3 != version)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, version does not match.");
                return false;
            }
            writer.WriteStartDocument();
            writer.WriteStartElement(fileName);
            output.WriteToOutput(loc3);
            output.WriteToOutput(reader.ReadByte());
            uint loc4 = 0;
            uint loc5 = loc4 = reader.ReadUInt32();
            uint loc6 = 0;
            Debug.WriteLine("__stringPoolIndex: " + __stringPoolIndex);
            uint loc7 = loc6 = (uint)(this.__stringPoolIndex = (int)reader.BaseStream.Position);
            Debug.WriteLine("__stringPoolIndex [After Stream Position: " + __stringPoolIndex);
            strings = new Dictionary<int, string>();
            output.WriteToOutput(loc4);
            output.WriteToOutput(loc5);
            output.WriteToOutput(loc6);
            output.WriteToOutput(loc7);
            int i = 0;
            int[] offset = new int[loc5];
            offset[i] = 0;
            while (loc5 > 0)
            {
                loc11 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc5));
                string[] bigString = loc11.Split('\0');
                loc11 = bigString[0];
                offset[i + 1] = offset[i] + loc11.Length + 1;
                Debug.WriteLine(loc11);
                Debug.WriteLine(loc11.Length);
                strings[offset[i]] = loc11;
                Debug.WriteLine("strings.Values: " + strings.GetValueOrDefault(offset[i]));
                loc12 = (uint)loc11.Length;
                output.WriteToOutput("String: " + loc11 + " | Offset: " + offset[i]);
                loc5 -= loc12 + 1;
                Debug.WriteLine("loc5 (UINT) " + loc5);
                Debug.WriteLine("loc5 (Converted to INT) " + (int)loc5);
                loc7 += loc12 + 1;
                Debug.WriteLine("loc7 (UINT) " + loc7);
                i++;
                reader.BaseStream.Position = loc7;
            }
            uint loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc9 = (uint)reader.BaseStream.Position;
            arrays = new Dictionary<int, List<BKVValue>>();
            if (loc9 != loc8)
            {
                while (reader.BaseStream.Position < loc8)
                {
                    loc13 = (uint)reader.BaseStream.Position;
                    loc14 = reader.ReadUInt16();
                    output.WriteToOutput("Loc14: " + loc14);
                    loc15 = reader.ReadByte();
                    output.WriteToOutput("Loc15: " + loc15);
                    loc16 = new List<BKVValue>((int)loc14);
                    loc17 = new List<BKVValue>((int)loc14);
                    loc18 = 0;
                    while (loc18 < loc14)
                    {
                        loc17[loc18] = new BKVValue((int)loc15, param1, new WeakReference(this));
                        loc18++;
                    }
                    arrays[(int)(loc13 - loc9)] = loc17;
                }
            }
            loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc10 = (uint)reader.BaseStream.Position;
            List<int> dataOffsets = new List<int>();
            tables = new Dictionary<int, BKVTable>();
            Dictionary<string, XElement> keyValues = new Dictionary<string, XElement>();
            XElement valueTree;
            i = 0;
            Debug.WriteLine("reader Position (Before Tables Reading): " + reader.BaseStream.Position);
            while (reader.BaseStream.Position < loc8)
            {
                output.WriteToOutput("--------------------------- Bytes Read: " + i);
                Debug.WriteLine("--------------------------- Bytes Read: " + i);
                int datOffset = i;
                valueTree = new XElement("offset-" + i);
                dataOffsets.Add(i);
                loc19 = (uint)reader.BaseStream.Position;
                loc20 = new BKVTable();
                Debug.WriteLine("reader Position (Before UINT16 1st Read): " + reader.BaseStream.Position);
                loc21 = reader.ReadUInt16();
                i += 2;
                Debug.WriteLine("reader Position (After UINT16 1st Read): " + reader.BaseStream.Position);
                loc18 = 0; 
                while (loc18 < loc21)
                {
                    loc22 = reader.ReadUInt16();
                    i += 2;
                    Debug.WriteLine("reader Position (After UINT16 2nd Read): " + reader.BaseStream.Position);
                    loc23 = "null";
                    Debug.WriteLine("Loc22 & key_mask: " + (loc22 & key_mask));
                    if ((loc22 & key_mask) == 0)
                    {
                        if (!strings.ContainsKey((int)loc22))
                        {
                            loc26 = (int)reader.BaseStream.Position;
                            loc23 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc4 - (int)loc22));
                            string[] bigString = loc11.Split('\0');
                            loc23 = bigString[0];
                            reader.BaseStream.Position = loc26;
                        }
                        else
                        {
                            loc23 = strings[(int)loc22];

                        }
                    }
                    loc24 = reader.ReadByte();
                    i += 1;
                    Debug.WriteLine(loc24);
                    XElement node = new XElement(loc23, "");
                    dynamic temp = null;
                    switch (loc24)
                    {
                        case 0: // Bool - False
                            output.WriteToOutput(loc23 + " : Bool - False.");
                            Debug.WriteLine(loc23 + " : Bool - False.");
                            node = new XElement(loc23, "false");
                            break;
                        case 1: // Bool - True
                            output.WriteToOutput(loc23 + " : Bool - True.");
                            Debug.WriteLine(loc23 + " : Bool - True.");
                            node = new XElement(loc23, "true");
                            break;
                        case 2: // Float
                            i += 4;
                            temp = reader.ReadSingle();
                            Debug.WriteLine(loc23 + " : " + temp as string);
                            output.WriteToOutput(loc23 + " : " + temp);
                            node = new XElement(loc23, temp.ToString());
                            break;
                        case 3: // Byte
                            i += 1;
                            temp = reader.ReadByte();
                            Debug.WriteLine(loc23 + " : " + temp as string);
                            output.WriteToOutput(loc23 + " : " + temp);
                            node = new XElement(loc23, temp.ToString());
                            break;
                        case 4: // Short
                            i += 2;
                            temp = reader.ReadUInt16();
                            Debug.WriteLine(loc23 + " : " + temp as string);
                            output.WriteToOutput(loc23 + " : " + temp);
                            node = new XElement(loc23, temp.ToString());
                            break;
                        case 5: // Integer
                            i += 4;
                            temp = reader.ReadUInt32();
                            Debug.WriteLine(loc23 + " : " + temp as string);
                            output.WriteToOutput(loc23 + " : " + temp);
                            node = new XElement(loc23, temp.ToString());
                            break;
                        case 6: // String
                            i += 2;
                            temp = strings[reader.ReadUInt16()];
                            Debug.WriteLine(loc23 + " : " + temp as string);
                            output.WriteToOutput(loc23 + " : " + temp);
                            node = new XElement(loc23, temp);
                            break;
                        case 7: // Table
                            i += 2;
                            temp = reader.ReadUInt16();
                            Debug.WriteLine(loc23 + " : " + "[Table], Offset - " + temp as string);
                            output.WriteToOutput(loc23 + " : " + "[Table], Offset - " + temp.ToString());
                            if (keyValues.ContainsKey("offset-" + temp as string))
                            {
                                keyValues["offset-" + temp as string].Name = "null";
                                valueTree.Add(keyValues["offset-" + temp as string]);
                            }
                            else
                            {
                                node = new XElement(loc23, "offset-" + temp as string);
                            }
                            break;
                        case 8: // Array
                            i += 2;
                            temp = reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + "[Array]");
                            break;
                    }
                    valueTree.Add(node);
                    loc25 = new BKVValue((int)loc24, param1, new WeakReference(this));
                    loc20.AddPair(loc23, loc25);
                    loc18++;
                }
                keyValues.Add("offset-" + datOffset.ToString(), valueTree);
                tables[(int)(loc19 - loc10)] = loc20;
                if (rootTable == null)
                {
                    rootTable = loc20;
                }
            }
            reader.Close();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            Debug.WriteLine("BKV Reader: Loading has returned true!");
            output.WriteToOutput("------------------------ Total Bytes Read: " + i);
            output.WriteToOutput("BKV Reader: Loading has returned true!");
            Debug.WriteLine("Tables: " + tables.Count);
            XDocument xmlDocument = XDocument.Load(exportPath);
            string xPath = fileName, currentXPath = xPath;
            xmlDocument.Element(fileName).Add(keyValues["offset-0"].Elements());
            currentXPath = xPath;
            foreach (XElement element in xmlDocument.XPathSelectElement(xPath).Elements())
            {
                Debug.WriteLine("Current XPath: " + xPath);
                Debug.WriteLine("Current Element: " + element.Name.LocalName + " - " + element.Value);
                Debug.WriteLine("");
                if (keyValues.ContainsKey(element.Value))
                {
                    string key = element.Value, keyName = element.Name.LocalName;
                    xmlDocument.XPathSelectElement(xPath + "/" + element.Name).RemoveNodes();
                    xmlDocument.XPathSelectElement(xPath + "/" + keyName).Add(keyValues[key]);
                }
            }
            xmlDocument.Element(fileName).Descendants().Where(x => string.Equals(x.Name.LocalName, "null") && x.HasElements == false).Remove();
            xmlDocument.Save(exportPath);
            return true;
        }

        public bool WriteBytesToJson(byte[] param1)
        {
            if (exportPath == "")
            {
                return false;
            }
            TextWriter sb = File.CreateText(exportPath);
            JsonWriter writer = new JsonTextWriter(sb);
            output.ClearLog();
            MemoryStream memory = new MemoryStream(param1);
            string loc11 = null, loc23 = null;
            uint loc12 = 0, loc13 = 0, loc14 = 0, loc15 = 0, loc19 = 0, loc21 = 0, loc22 = 0, loc24 = 0;
            int loc18 = 0, loc26 = 0;
            List<BKVValue> loc16 = null, loc17 = null;
            BKVTable loc20 = null;
            BKVValue loc25 = new BKVValue();
            if (param1 == null || param1.Length < 10)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file is not a BKV.");
                return false;
            }
            ///memory.Write(param1);
            BinaryReader reader = new BinaryReader(memory);
            __data = new ByteArray(param1);
            int loc2 = reader.ReadInt32();
            if (loc2 != header && loc2 != header_inv)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, file header does not match.");
                return false;
            }
            output.WriteToOutput(loc2);
            int loc3 = reader.ReadSByte();
            if (loc3 != version)
            {
                Debug.WriteLine("BKV Reader: Loading has returned false, version does not match.");
                return false;
            }
            writer.WriteStartObject();
            writer.WritePropertyName(fileName);
            writer.WriteStartArray();
            output.WriteToOutput(loc3);
            output.WriteToOutput(reader.ReadByte());
            uint loc4 = 0;
            uint loc5 = loc4 = reader.ReadUInt32();
            uint loc6 = 0;
            Debug.WriteLine("__stringPoolIndex: " + __stringPoolIndex);
            uint loc7 = loc6 = (uint)(this.__stringPoolIndex = (int)reader.BaseStream.Position);
            Debug.WriteLine("__stringPoolIndex [After Stream Position: " + __stringPoolIndex);
            strings = new Dictionary<int, string>();
            output.WriteToOutput(loc4);
            output.WriteToOutput(loc5);
            output.WriteToOutput(loc6);
            output.WriteToOutput(loc7);
            int i = 0;
            int[] offset = new int[loc5];
            offset[i] = 0;
            while (loc5 > 0)
            {
                loc11 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc5));
                string[] bigString = loc11.Split('\0');
                loc11 = bigString[0];
                offset[i + 1] = offset[i] + loc11.Length + 1;
                Debug.WriteLine(loc11);
                Debug.WriteLine(loc11.Length);
                strings[offset[i]] = loc11;
                Debug.WriteLine("strings.Values: " + strings.GetValueOrDefault(offset[i]));
                loc12 = (uint)loc11.Length;
                output.WriteToOutput("String: " + loc11 + " | Offset: " + offset[i]);
                loc5 -= loc12 + 1;
                Debug.WriteLine("loc5 (UINT) " + loc5);
                Debug.WriteLine("loc5 (Converted to INT) " + (int)loc5);
                loc7 += loc12 + 1;
                Debug.WriteLine("loc7 (UINT) " + loc7);
                i++;
                reader.BaseStream.Position = loc7;
            }
            uint loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc9 = (uint)reader.BaseStream.Position;
            arrays = new Dictionary<int, List<BKVValue>>();
            if (loc9 != loc8)
            {
                while (reader.BaseStream.Position < loc8)
                {
                    loc13 = (uint)reader.BaseStream.Position;
                    loc14 = reader.ReadUInt16();
                    output.WriteToOutput("Loc14: " + loc14);
                    loc15 = reader.ReadByte();
                    output.WriteToOutput("Loc15: " + loc15);
                    loc16 = new List<BKVValue>((int)loc14);
                    loc17 = new List<BKVValue>((int)loc14);
                    loc18 = 0;
                    while (loc18 < loc14)
                    {
                        loc17[loc18] = new BKVValue((int)loc15, param1, new WeakReference(this, true));
                        loc18++;
                    }
                    arrays[(int)(loc13 - loc9)] = loc17;
                }
            }
            loc8 = (uint)reader.BaseStream.Position + reader.ReadUInt32();
            uint loc10 = (uint)reader.BaseStream.Position;
            tables = new Dictionary<int, BKVTable>();
            i = 0;
            Debug.WriteLine("reader Position (Before Tables Reading): " + reader.BaseStream.Position);
            Dictionary<string, string> xmlData = new Dictionary<string, string>();
            while (reader.BaseStream.Position < loc8)
            {
                output.WriteToOutput("--------------------------- Bytes Read: " + i);
                loc19 = (uint)reader.BaseStream.Position;
                loc20 = new BKVTable();
                Debug.WriteLine("reader Position (Before UINT16 1st Read): " + reader.BaseStream.Position);
                loc21 = reader.ReadUInt16();
                i += 2;
                Debug.WriteLine("reader Position (After UINT16 1st Read): " + reader.BaseStream.Position);
                loc18 = 0;
                writer.WriteStartObject();
                writer.WritePropertyName("offset-" + (i - 2).ToString());
                writer.WriteStartArray();
                while (loc18 < loc21)
                {
                    loc22 = reader.ReadUInt16();
                    i += 2;
                    Debug.WriteLine("reader Position (After UINT16 2nd Read): " + reader.BaseStream.Position);
                    loc23 = "null";
                    Debug.WriteLine("Loc22 & key_mask: " + (loc22 & key_mask));
                    if ((loc22 & key_mask) == 0)
                    {
                        if (!strings.ContainsKey((int)loc22))
                        {
                            loc26 = (int)reader.BaseStream.Position;
                            loc23 = Encoding.UTF8.GetString(reader.ReadBytes((int)loc4 - (int)loc22));
                            string[] bigString = loc11.Split('\0');
                            loc23 = bigString[0];
                            reader.BaseStream.Position = loc26;
                        }
                        else
                        {
                            loc23 = strings[(int)loc22];

                        }
                    }
                    loc24 = reader.ReadByte();
                    i += 1;
                    Debug.WriteLine(loc24);
                    writer.WriteStartObject();
                    writer.WritePropertyName(loc23);
                    dynamic temp;
                    switch (loc24)
                    {
                        case 0: // Bool - False
                            output.WriteToOutput(loc23 + " : " + "Bool - False.");
                            writer.WriteStartObject();
                            writer.WriteValue(false);
                            break;
                        case 1: // Bool - True
                            output.WriteToOutput(loc23 + " : " + "Bool - True.");
                            writer.WriteValue(true);
                            break;
                        case 2: // Float
                            i += 4;
                            temp = reader.ReadSingle();
                            output.WriteToOutput(loc23 + " : " + temp);
                            writer.WriteValue(temp);
                            break;
                        case 3: // Byte
                            i += 1;
                            temp = reader.ReadByte();
                            output.WriteToOutput(loc23 + " : " + temp);
                            writer.WriteValue(temp);
                            break;
                        case 4: // Short
                            i += 2;
                            temp = reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + temp);
                            writer.WriteValue(temp);
                            break;
                        case 5: // Integer
                            i += 4;
                            temp = reader.ReadUInt32();
                            output.WriteToOutput(loc23 + " : " + temp);
                            writer.WriteValue(temp);
                            break;
                        case 6: // String
                            i += 2;
                            temp = strings[reader.ReadUInt16()];
                            output.WriteToOutput(loc23 + " : " + temp);
                            writer.WriteValue(temp);
                            break;
                        case 7: // Table
                            i += 2;
                            temp = reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + "[Table]");
                            writer.WriteValue("tableoffset-" + temp);
                            break;
                        case 8: // Array
                            i += 2;
                            temp = reader.ReadUInt16();
                            output.WriteToOutput(loc23 + " : " + "[Array]");
                            writer.WriteValue(temp.ToString() + " - Array");
                            break;
                    }
                    writer.WriteEndObject();
                    loc25 = new BKVValue((int)loc24, param1, new WeakReference(this, true));
                    Debug.WriteLine(loc25);
                    loc20.AddPair(loc23, loc25);
                    loc18++;
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
                tables[(int)(loc19 - loc10)] = loc20;
                if (!this.rootTable.Equals(loc20))
                {
                    rootTable = loc20;
                }
            }
            reader.Close();
            writer.WriteEndArray();
            writer.WriteEndObject();
            sb.Flush();
            sb.Close();
            Debug.WriteLine("BKV Reader: Loading has returned true!");
            output.WriteToOutput("------------------------ Total Bytes Read: " + i, "BKV Reader: Loading has returned true!");
            Debug.WriteLine("Tables: " + tables.Count);
            return true;
        }

        public BKVTable GetRoot()
        {
            return rootTable;
        }

        public string GetString(int param1)
        {
            if (!strings.ContainsKey(param1))
            {
                if (__data == null) return "! Data Not Found !";
                __data.Position = (__stringPoolIndex + param1);
                strings[param1] = __data.ReadUTFBytes((uint)(__data.Length - __stringPoolIndex - param1));
            }
            return strings[param1];
        }

        public BKVTable GetTable(int param1)
        {
            return tables[param1];
        }

        public List<BKVValue> GetArray(int param1)
        {
            return arrays[param1];
        }

        public override string ToString()
        {
            return rootTable.ToString();
        }
    }
}
