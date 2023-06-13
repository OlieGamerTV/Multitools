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

namespace Multi_Tool.Tools
{
    class CompressionTool
    {
        private string exportPath, fileName, filePath, algorithmType;
        private CompressionLevel currentFileCompLevel;
        private byte[] dataText;
        private OutputLog output;
        private SERFExtractor serfExtractor;
        private string algorithm;

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
                    case AlgorithmEnums.ZLIB:
                    case AlgorithmEnums.DEFLATE:
                    case AlgorithmEnums.GZIP:
                    case AlgorithmEnums.LZMA:
                        Compression_Comp(filePath);
                        break;
                    case AlgorithmEnums.SERF:
                        serfExtractor.PassData(exportPath, fileName, filePath, dataText);
                        serfExtractor.RipSerfFile();
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
                }
            }
        }

        public void Compression_Decomp()
        {
            try
            {
                byte[] decomped;
                output.WriteToOutput("Opening File: " + filePath);
                Stream readStream = File.Open(filePath, FileMode.Open);
                Stream writeStream = File.Open(exportPath + "/" + fileName, FileMode.Create);
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
                fileDialog.FileName = "Select the compressed file you want to extract embedded files from.";
                // Code for looking for a file.
                if (fileDialog.ShowDialog() == true)
                {
                    string dataPath = Path.GetFullPath(fileDialog.FileName);
                    filePath = dataPath;
                    output.WriteToOutput("Selected path: " + dataPath);
                    string file = Path.GetFileName(filePath);
                    output.WriteToOutput("Selected file: " + file);
                    fileName = file;
                    output.WriteToOutput("Selected file to rip set to: " + filePath);
                    dataText = File.ReadAllBytes(filePath);
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
                fileDialog.Title = "Export Compressed File";
                fileDialog.ValidateNames = true;
                fileDialog.CheckFileExists = false;
                fileDialog.Filter = "Common File Compressions (*.zlib;*.deflate;*.gz)|*.zlib;*.deflate;*.gz|All Files (*.*)|*.*";
                fileDialog.FileName = "compressed.zlib";
                // Code for looking for a export path.
                if (fileDialog.ShowDialog() == true)
                {
                    string? path = fileDialog.FileName;
                    output.WriteToOutput("Selected export path: " + path);
                    exportPath = path;
                    switch (Path.GetExtension(fileDialog.FileName))
                    {
                        default:
                        case ".zlib":
                            SetAlgorithm(AlgorithmEnums.ZLIB);
                            break;
                        case ".deflate":
                            SetAlgorithm(AlgorithmEnums.DEFLATE);
                            break;
                        case ".gz":
                            SetAlgorithm(AlgorithmEnums.GZIP);
                            break;
                    }
                }
                else
                {
                    Debug.WriteLine("Selected directory does not exist!");
                }
            }
        }
    }
}
