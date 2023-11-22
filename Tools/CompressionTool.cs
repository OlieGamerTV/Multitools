using Ionic.Zlib;
using Microsoft.Win32;
using Multi_Tool.Enumerators;
using Multi_Tool.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using DSDecmpProg = DSDecmp.Program;
using System.Collections.Generic;

namespace Multi_Tool.Tools
{
    class CompressionTool
    {
        private string exportPath, fileName, filePath, algorithmType;
        private string[] filePaths;
        private CompressionLevel currentFileCompLevel;
        private byte[] dataText;
        private OutputLog output;
        private SERFExtractor serfExtractor;
        private string algorithm;
        public string xorKeyA, xorKeyB;

        public CompressionTool(ListView list) : base()
        {
            output = new OutputLog(list);
            serfExtractor = new SERFExtractor(list);
        }

        public async Task StartZlibFile(bool decomp)
        {
            if (filePath == null)
            {
                GetDirs(true);
            }
            if (exportPath == null)
            {
                GetDirs(false);
            }
            if (decomp)
            {
                switch (algorithm)
                {
                    default:
                    case AlgorithmEnums.AUTOMATIC:
                        CheckCompression();
                        break;
                    case AlgorithmEnums.ZLIB:
                    case AlgorithmEnums.DEFLATE:
                    case AlgorithmEnums.GZIP:
                    case AlgorithmEnums.LZMA:
                        Compression_Decomp(filePaths, exportPath);
                        break;
                    case AlgorithmEnums.LZ:
                        HandleLZData(filePaths, exportPath);
                        break;
                    case AlgorithmEnums.SERF:
                        serfExtractor.PassData(exportPath, fileName, filePath, dataText);
                        serfExtractor.RipSerfFile();
                        break;
                    case AlgorithmEnums.KEYXOR:
                        HandleXORData(filePaths, exportPath);
                        break;
                }
            }
            else
            {
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                    case AlgorithmEnums.DEFLATE:
                    case AlgorithmEnums.GZIP:
                    case AlgorithmEnums.LZMA:
                        Compression_Comp(filePath);
                        break;
                    case AlgorithmEnums.SERF:
                        //serf.StartSERFRip();
                        break;
                    case AlgorithmEnums.KEYXOR:
                        HandleXORData(filePaths, exportPath);
                        break;
                }
            }
        }

        private void HandleLZData(string[] filesIn, string flrOut)
        {
            foreach (string path in filesIn)
            {
                output.WriteToOutput("-----------------------------------------", string.Format("Reading File [{0}]", Path.GetFileName(path)));
                FileStream file = File.OpenRead(path);
                BinaryReader br = new BinaryReader(file);
                int magic = br.ReadInt32();
                br.Close();
                if (magic == DSDecmpProg.COMP_MAGIC || magic == DSDecmpProg.INV_COMP_MAGIC)
                {
                    DSDecmpProg.DecompressCOMP(path, flrOut);
                }
                else
                {
                    DSDecmpProg.Decompress(path, flrOut);
                }
                output.WriteToOutput(string.Format("Uncompressed file [{0}] exported to [{1}].", Path.GetFileName(path), flrOut));
            }
        }

        private void HandleXORData(string[] filesIn, string flrOut)
        {
            foreach (string path in filesIn)
            {
                output.WriteToOutput("-----------------------------------------", string.Format("Reading File [{0}]", Path.GetFileName(path)));
                XOREncrypt.Convert(path, flrOut, xorKeyA, xorKeyB);
                output.WriteToOutput(string.Format("Converted file [{0}] exported to [{1}].", Path.GetFileName(path), flrOut));
            }
        }

        #region Compression/Decompression
        public void Compression_Decomp(string[] filesIn, string flrOut)
        {
            foreach (string path in filesIn)
            {
                if (Path.GetExtension(path) == ".swf")
                {
                    Compression_SWFDecomp(path, flrOut);
                }
                else
                {
                    try
                    {
                        byte[] decomped;
                        output.WriteToOutput("Opening File: " + path);
                        Stream readStream = File.Open(path, FileMode.Open);
                        Stream writeStream = File.Open(flrOut + "/" + Path.GetFileName(path), FileMode.Create);
                        MemoryStream memory = new MemoryStream();
                        switch (algorithm)
                        {
                            case AlgorithmEnums.ZLIB:
                                ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Decompress);
                                zlib.CopyTo(writeStream);
                                zlib.Dispose();
                                zlib.Close();
                                break;
                            case AlgorithmEnums.DEFLATE:
                                DeflateStream def = new DeflateStream(readStream, CompressionMode.Decompress);
                                def.CopyTo(memory);
                                def.Dispose();
                                def.Close();
                                break;
                            case AlgorithmEnums.GZIP:
                                GZipStream gzip = new GZipStream(readStream, CompressionMode.Decompress);
                                gzip.CopyTo(memory);
                                gzip.Dispose();
                                gzip.Close();
                                break;
                        }
                        decomped = memory.GetBuffer();
                        writeStream.Write(memory.GetBuffer());
                        readStream.Close();
                        writeStream.Flush();
                        memory.Dispose();
                        memory.Close();
                    }
                    catch (Exception ex)
                    {
                        output.WriteToOutput("CompressionTool: Decompression failed. Error: " + ex);
                        return;
                    }
                }
            }
        }

        public void Compression_SWFDecomp(string fileIn, string flrOut)
        {
            try
            {
                byte[] decomped;
                output.WriteToOutput("Opening File: " + fileIn);
                Stream readStream = File.Open(fileIn, FileMode.Open);
                BinaryReader binaryReader = new BinaryReader(readStream);
                byte[] magicBytes = binaryReader.ReadBytes(3);
                string magic = System.Text.Encoding.UTF8.GetString(magicBytes);
                if (magic == "FWS")
                {
                    output.WriteToOutput($"CompressionTool: SWF is already uncompressed [{magic}]. No action needed.");
                    return;
                }
                byte flashVer = binaryReader.ReadByte();
                byte[] fileSize = binaryReader.ReadBytes(4);
                output.WriteToOutput($"CompressionTool: {magic} - {Convert.ToInt32(flashVer)}");
                BinaryWriter bw = new BinaryWriter(File.Open(flrOut + "/" + Path.GetFileName(fileIn), FileMode.Create));
                MemoryStream memory = new MemoryStream();
                bw.Write(System.Text.Encoding.UTF8.GetBytes("FWS"));
                bw.Write(flashVer);
                bw.Write(fileSize);
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                        ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Decompress);
                        zlib.CopyTo(bw.BaseStream);
                        zlib.Dispose();
                        zlib.Close();
                        break;
                    case AlgorithmEnums.DEFLATE:
                        DeflateStream def = new DeflateStream(readStream, CompressionMode.Decompress);
                        def.CopyTo(memory);
                        def.Dispose();
                        def.Close();
                        break;
                    case AlgorithmEnums.GZIP:
                        GZipStream gzip = new GZipStream(readStream, CompressionMode.Decompress);
                        gzip.CopyTo(memory);
                        gzip.Dispose();
                        gzip.Close();
                        break;
                }
                decomped = memory.GetBuffer();
                bw.Write(memory.GetBuffer());
                binaryReader.Dispose();
                binaryReader.Close();
                bw.Flush();
                bw.Close();
                memory.Dispose();
                memory.Close();
            }
            catch (Exception ex)
            {
                output.WriteToOutput("CompressionTool: Decompression failed. Error: " + ex);
                return;
            }
        }

        public void Compression_SWFDecomp(string[] filesIn, string flrOut)
        {
            foreach (string path in filesIn)
            {
                try
                {
                    byte[] decomped;
                    output.WriteToOutput("Opening File: " + path);
                    Stream readStream = File.Open(path, FileMode.Open);
                    Stream writeStream = File.Open(flrOut + "/" + Path.GetFileName(path), FileMode.Create);
                    MemoryStream memory = new MemoryStream();
                    switch (algorithm)
                    {
                        case AlgorithmEnums.ZLIB:
                            ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Decompress);
                            zlib.CopyTo(writeStream);
                            zlib.Dispose();
                            zlib.Close();
                            break;
                        case AlgorithmEnums.DEFLATE:
                            DeflateStream def = new DeflateStream(readStream, CompressionMode.Decompress);
                            def.CopyTo(memory);
                            def.Dispose();
                            def.Close();
                            break;
                        case AlgorithmEnums.GZIP:
                            GZipStream gzip = new GZipStream(readStream, CompressionMode.Decompress);
                            gzip.CopyTo(memory);
                            gzip.Dispose();
                            gzip.Close();
                            break;
                    }
                    decomped = memory.GetBuffer();
                    writeStream.Write(memory.GetBuffer());
                    readStream.Close();
                    writeStream.Flush();
                    memory.Dispose();
                    memory.Close();
                }
                catch (Exception ex)
                {
                    output.WriteToOutput("CompressionTool: Decompression failed. Error: " + ex);
                    return;
                }
            }
        }

        public void Compression_Comp(string filePath)
        {
            if (filePath == null)
            {
                GetDirs(true);
            }
            try
            {
                output.WriteToOutput("--------------------------", "Opening File: " + filePath);
                Stream readStream = File.Open(filePath, FileMode.Open);
                Stream writeStream = File.Open(exportPath, FileMode.Create);
                MemoryStream memory = new MemoryStream();
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                        output.WriteToOutput("Compressing file using ZLIB... Compression Level: " + (int)currentFileCompLevel);
                        ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Compress, currentFileCompLevel);
                        zlib.CopyTo(writeStream);
                        zlib.Dispose();
                        zlib.Close();
                        break;
                    case AlgorithmEnums.DEFLATE:
                        output.WriteToOutput("Compressing file using Deflate... Compression Level: " + (int)currentFileCompLevel);
                        DeflateStream def = new DeflateStream(readStream, CompressionMode.Compress, currentFileCompLevel);
                        def.CopyTo(memory);
                        def.Dispose();
                        def.Close();
                        break;
                    case AlgorithmEnums.GZIP:
                        output.WriteToOutput("Compressing file using GZIP... Compression Level: " + (int)currentFileCompLevel);
                        GZipStream gzip = new GZipStream(readStream, CompressionMode.Compress, currentFileCompLevel);
                        gzip.CopyTo(memory);
                        gzip.Dispose();
                        gzip.Close();
                        break;
                }
                writeStream.Write(memory.GetBuffer());
                readStream.Close();
                writeStream.Flush();
                memory.Dispose();
                memory.Close();
                output.WriteToOutput("Compression finished!", "Compressed file has been saved to: " + exportPath);
            }
            catch (Exception ex)
            {
                output.WriteToOutput("CompressionTool: Compression failed. Error: " + ex);
                return;
            }
        }

        public byte[]? Util_CompDecompData(bool decomp, string filePath)
        {
            if (decomp)
            {
                byte[] decomped;
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                        using (var compressedStream = new MemoryStream(File.ReadAllBytes(filePath)))
                        using (var zlib = new ZlibStream(compressedStream, CompressionMode.Decompress))
                        using (var resultStream = new MemoryStream())
                        {
                            zlib.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    case AlgorithmEnums.DEFLATE:
                        using (var compressedStream = new MemoryStream(File.ReadAllBytes(filePath)))
                        using (var def = new DeflateStream(compressedStream, CompressionMode.Decompress))
                        using (var resultStream = new MemoryStream())
                        {
                            def.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                    case AlgorithmEnums.GZIP:
                        using (var compressedStream = new MemoryStream(File.ReadAllBytes(filePath)))
                        using (var gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
                        using (var resultStream = new MemoryStream())
                        {
                            gzip.CopyTo(resultStream);
                            return resultStream.ToArray();
                        }
                }
                return null;
            }
            else
            {
                byte[] comped;
                byte[] decomped = File.ReadAllBytes(filePath);
                MemoryStream memory = new MemoryStream(decomped);
                MemoryStream resultMemory = new MemoryStream();
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                        ZlibStream zlib = new ZlibStream(memory, CompressionMode.Compress, currentFileCompLevel);
                        zlib.CopyTo(resultMemory);
                        zlib.Dispose();
                        zlib.Close();
                        break;
                    case AlgorithmEnums.DEFLATE:
                        DeflateStream def = new DeflateStream(memory, CompressionMode.Compress, currentFileCompLevel);
                        def.CopyTo(resultMemory);
                        def.Dispose();
                        def.Close();
                        break;
                    case AlgorithmEnums.GZIP:
                        GZipStream gzip = new GZipStream(memory, CompressionMode.Compress);
                        gzip.CopyTo(resultMemory);
                        gzip.Dispose();
                        gzip.Close();
                        break;
                }
                comped = resultMemory.GetBuffer();
                resultMemory.Dispose();
                resultMemory.Close();
                memory.Dispose();
                memory.Close();
                return comped;
            }
        }

        public static byte[] FileDecompress(string fileIn, string algorithm = "ZLIB")
        {
            try
            {
                byte[] decomped;
                Debug.WriteLine("Opening File: " + fileIn);
                Stream readStream = File.Open(fileIn, FileMode.Open);
                MemoryStream memory = new MemoryStream();
                switch (algorithm)
                {
                    case AlgorithmEnums.ZLIB:
                        ZlibStream zlib = new ZlibStream(readStream, CompressionMode.Decompress);
                        zlib.CopyTo(memory);
                        zlib.Dispose();
                        zlib.Close();
                        break;
                    case AlgorithmEnums.DEFLATE:
                        DeflateStream def = new DeflateStream(readStream, CompressionMode.Decompress);
                        def.CopyTo(memory);
                        def.Dispose();
                        def.Close();
                        break;
                    case AlgorithmEnums.GZIP:
                        GZipStream gzip = new GZipStream(readStream, CompressionMode.Decompress);
                        gzip.CopyTo(memory);
                        gzip.Dispose();
                        gzip.Close();
                        break;
                }
                decomped = memory.GetBuffer();
                readStream.Close();
                memory.Dispose();
                memory.Close();
                return decomped;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CompressionTool: Decompression failed. Error: " + ex);
                return null;
            }
        }
        #endregion

        private void CheckCompression()
        {
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
            foreach (string file in filePaths)
            {
                BinaryReader check = new BinaryReader(File.Open(file, FileMode.Open));
                Span<byte> buffer = new Span<byte>(check.ReadBytes(4));
                string magic = Convert.ToHexString(buffer);
                if (magic.Contains(Magic.ZLIB_C7F2))
                {
                    algorithm = AlgorithmEnums.ZLIB;
                    currentFileCompLevel = CompressionLevel.Level7;
                    if (!files.ContainsKey(AlgorithmEnums.ZLIB))
                    {
                        files.Add(AlgorithmEnums.ZLIB, new List<string>());
                    }
                    files[AlgorithmEnums.ZLIB].Add(file);
                }
                if (magic.Contains(Magic.ZLIB_C6F2))
                {
                    algorithm = AlgorithmEnums.ZLIB;
                    currentFileCompLevel = CompressionLevel.Level6;
                    if (!files.ContainsKey(AlgorithmEnums.ZLIB))
                    {
                        files.Add(AlgorithmEnums.ZLIB, new List<string>());
                    }
                    files[AlgorithmEnums.ZLIB].Add(file);
                }
                if (magic.Contains(Magic.SERF_MAGIC_HEX))
                {
                    algorithm = AlgorithmEnums.SERF;
                    currentFileCompLevel = CompressionLevel.Level0;
                    if (!files.ContainsKey(AlgorithmEnums.SERF))
                    {
                        files.Add(AlgorithmEnums.SERF, new List<string>());
                    }
                    files[AlgorithmEnums.SERF].Add(file);
                }
                output.WriteToOutput($"File Buffer: {magic} - [{algorithm}, {currentFileCompLevel}].");
            }
        }

        public void SetAlgorithm(string value)
        {
            algorithm = value;
            Debug.WriteLine("Algorithm Compression set to: " + algorithm.ToString().ToUpper());
        }

        public void SetCompLevel(int level)
        {
            switch (level)
            {
                case 0:
                    currentFileCompLevel = CompressionLevel.Level0;
                    return;
                case 1:
                    currentFileCompLevel = CompressionLevel.Level1;
                    return;
                case 2:
                    currentFileCompLevel = CompressionLevel.Level2;
                    return;
                case 3:
                    currentFileCompLevel = CompressionLevel.Level3;
                    return;
                case 4:
                    currentFileCompLevel = CompressionLevel.Level4;
                    return;
                case 5:
                    currentFileCompLevel = CompressionLevel.Level5;
                    return;
                default:
                case 6:
                    currentFileCompLevel = CompressionLevel.Level6;
                    return;
                case 7:
                    currentFileCompLevel = CompressionLevel.Level7;
                    return;
                case 8:
                    currentFileCompLevel = CompressionLevel.Level8;
                    return;
                case 9:
                    currentFileCompLevel = CompressionLevel.Level9;
                    return;
            }
        }

        public void GetDirs(bool isFile)
        {
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            if (isFile)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.CheckPathExists = true;
                fileDialog.Title = "Open";
                fileDialog.ValidateNames = true;
                fileDialog.CheckFileExists = true;
                fileDialog.Multiselect = true;
                fileDialog.FileName = "Select the compressed file(s) you want to uncompress.";
                // Code for looking for a file.
                if (fileDialog.ShowDialog() == true)
                {
                    string dataPath = Path.GetFullPath(fileDialog.FileName);
                    filePath = dataPath;
                    output.WriteToOutput("Selected path: " + dataPath);
                    filePaths = fileDialog.FileNames;
                    string file = Path.GetFileName(filePath);
                    output.WriteToOutput("Selected file amount: " + filePaths.Length);
                    fileName = file;
                    output.WriteToOutput("Selected files: [" + string.Join(",\n", fileDialog.FileNames) + "]");
                    dataText = File.ReadAllBytes(filePath);
                }
                else
                {
                    Debug.WriteLine("No file selected or file does not exist!");
                }
            }
            else
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.CheckPathExists = true;
                fileDialog.Title = "Export Compressed File(s)";
                fileDialog.ValidateNames = true;
                fileDialog.CheckFileExists = false;
                fileDialog.FileName = "Enter the folder you want to decompress to and press Open.";
                // Code for looking for a export path.
                if (fileDialog.ShowDialog() == true)
                {
                    string? path = fileDialog.FileName;
                    exportPath = Path.GetFullPath(fileDialog.FileName).Remove(fileDialog.FileName.LastIndexOf('\\'));
                    output.WriteToOutput("Selected export path: " + exportPath);
                }
                else
                {
                    Debug.WriteLine("Selected directory does not exist!");
                }
            }
        }
    }
}
