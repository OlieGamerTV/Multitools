using Microsoft.Win32;
using Multi_Tool.Enumerators;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Multi_Tool.Utilities;

namespace Multi_Tool.Tools
{
    class EmbeddedFileRip
    {
        string? exportPath = "", fileName = "";
        ImageEnums.FileTypes desiredFileType;
        byte[]? dataText = null;
        OutputLog output = null;
        bool saveChar = false;

        public EmbeddedFileRip(ListView list) : base()
        {
            output = new OutputLog(list);
        }

        public async Task RipFile() // This event is used to search for and extract embedded files.
        {
            if (dataText == null || dataText.Length < 4)
            {
                GetDirs(true);
            }
            if (exportPath == null || exportPath == string.Empty)
            {
                GetDirs(false);
            }
            output.ClearLog();
            Debug.WriteLine("Starting up the extracter...");
            output.WriteToOutput("Exported Files:");
            int[] index = new int[10];
            int dataLength = 0;
            int currentOffset = 0;
            // Create our export directory path
            if (!Directory.Exists(exportPath + "/" + fileName + "/" + desiredFileType))
            {
                Directory.CreateDirectory(exportPath + "/" + fileName + "/" + desiredFileType);
            }
            string currentExport = exportPath + "/" + fileName + "/" + desiredFileType;
            await Task.Delay(TimeSpan.FromSeconds(2));
            FileStream writer = new FileStream(currentExport + "/export_" + index + "." + desiredFileType, FileMode.Create);
            await Task.Delay(TimeSpan.FromSeconds(2));
            // The code we use to search through the entire data and look for embedded files.
            try
            {
                for (int i = 0; i < dataText.Length; i++)
                {
                    if (!saveChar)
                    {

                    }
                    else
                    {
                        writer.WriteByte(dataText[i]);
                    }
                    switch (desiredFileType)
                    {
                        case ImageEnums.FileTypes.png: // Code for embedded .png files.
                            switch (dataText[i])
                            {
                                case 137:
                                    if (dataText[i + 1] == 80 && dataText[i + 2] == 78 && dataText[i + 3] == 71 && !saveChar)
                                    {
                                        if (!File.Exists(currentExport + "/export_" + index[0] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/export_" + index[0] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.2f));
                                        writer.WriteByte(dataText[i]);
                                        saveChar = true;
                                        Debug.WriteLine(".png header detected. Writing data to file.");
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                                case 130:
                                    if (dataText[i - 1] == 96 && dataText[i - 2] == 66 && dataText[i - 3] == 174 && dataText[i - 4] == 68 && dataText[i - 5] == 78 && dataText[i - 6] == 69 && dataText[i - 7] == 73 && saveChar)
                                    {
                                        Debug.WriteLine(".png trail detected. Finishing current file writing and creating next file.");
                                        saveChar = false;
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                            }
                            break;
                        case ImageEnums.FileTypes.jpg: // Code for embedded .jpg files
                            switch (dataText[i])
                            {
                                case 255:
                                    if (dataText[i + 1] == 216 && dataText[i + 2] == 255 && dataText[i + 3] == 224 && dataText[i + 4] == 0 && dataText[i + 5] == 16 && !saveChar)
                                    {
                                        if (!File.Exists(currentExport + "/export_" + index[0] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/export_" + index[0] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.2f));
                                        writer.WriteByte(dataText[i]);
                                        saveChar = true;
                                        Debug.WriteLine(".jpg header detected. Writing data to file.");
                                    }
                                    else if (dataText[i + 1] == 216 && dataText[i + 2] == 255 && dataText[i + 3] == 224 && dataText[i + 4] == 0 && dataText[i + 5] == 16 && saveChar)
                                    {
                                        Debug.WriteLine("New .jpg header detected, stopping writing on current file.");
                                        saveChar = false;
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                                case 217:
                                    if (dataText[i - 1] == 255 && saveChar)
                                    {
                                        Debug.WriteLine(".jpg trail detected. Finishing current file writing and creating next file.");
                                        saveChar = false;
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                            }
                            break;
                        case ImageEnums.FileTypes.pvr: // Code for embedded .pvr files.
                            Debug.WriteLine("This hasn't been coded yet.");
                            output.WriteToOutput("This option hasn't been coded in yet.");
                            break;
                        case ImageEnums.FileTypes.dds: // Code for embedded .dds files.
                            switch (dataText[i])
                            {
                                case 68:
                                    if (dataText[i + 1] == 68 && dataText[i + 2] == 83 && dataText[i + 3] == 32 && !saveChar)
                                    {
                                        if (!File.Exists(currentExport + "/export_" + index[0] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/export_" + index[0] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.2f));
                                        writer.WriteByte(dataText[i]);
                                        saveChar = true;
                                        Debug.WriteLine(".dds header detected. Writing data to file.");
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                                case 115:
                                    if (dataText[i - 1] == 100 && dataText[i - 2] == 100 && dataText[i - 3] == 46 && saveChar)
                                    {
                                        Debug.WriteLine(".dds trail detected. Finishing current file writing and creating next file.");
                                        saveChar = false;
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                            }
                            break;
                        case ImageEnums.FileTypes.wav: // Code for embedded .wav files.
                            switch (dataText[i])
                            {
                                case 82:
                                    if (dataText[i + 1] == 73 && dataText[i + 2] == 70 && dataText[i + 3] == 70 && dataText[i + 8] == 87 && dataText[i + 9] == 65 && dataText[i + 10] == 86 && dataText[i + 11] == 69 && !saveChar)
                                    {
                                        if (!File.Exists(currentExport + "/export_" + index[0] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/export_" + index[0] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.05f));
                                        byte[] fileSize = new byte[4];
                                        for (int h = 0; h < 4; h++)
                                        {
                                            fileSize[h] = dataText[i + 4 + h];
                                        }
                                        writer.Write(dataText, i, BitConverter.ToInt32(fileSize));
                                        Debug.WriteLine(".wav header detected. Writing data to file.");
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                                case 128:
                                    if (dataText[i + 1] == 0 && dataText[i + 2] == 0 && dataText[i - 1] == 0 && dataText[i - 2] == 0 && saveChar)
                                    {
                                        Debug.WriteLine(".wav trail detected. Finishing current file writing and creating next file.");
                                        saveChar = false;
                                        output.WriteToOutput("export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                        writer.Flush();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                            }
                            break;
                        case ImageEnums.FileTypes.swf:
                            switch (dataText[i])
                            {
                                case 70:
                                    if (dataText[i + 1] == 87 && dataText[i + 2] == 83 && (dataText[i + 3] >= 1 && dataText[i + 3] <= 36) && !saveChar)
                                    {
                                        byte[] fileSize = new byte[4];
                                        for (int h = 0; h < 4; h++)
                                        {
                                            fileSize[h] = dataText[i + 4 + h];
                                        }
                                        if ((BitConverter.ToInt32(fileSize) + i) > dataText.Length || BitConverter.ToInt32(fileSize) < 0)
                                        {
                                            output.WriteToOutput("Invalid File, data size: " + BitConverter.ToInt32(fileSize) + " at offset: " + i + ". Skipping FWS Flash file.");
                                            break;
                                        }
                                        if (!File.Exists(currentExport + "/FWS_export_" + index[0] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/FWS_export_" + index[0] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.05f));
                                        output.WriteToOutput("SWF File found. Version " + dataText[i + 3] + ", SWF uses no compression.", "Data size: " + BitConverter.ToInt32(fileSize) + ", offset: " + i);
                                        writer.Write(dataText, i, BitConverter.ToInt32(fileSize));
                                        Debug.WriteLine(".swf header detected. Writing data to file.");
                                        output.WriteToOutput("FWS_export_" + index[0] + "." + desiredFileType + " exported successfully.");
                                        index[0]++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                                case 67:
                                    if (dataText[i + 1] == 87 && dataText[i + 2] == 83 && (dataText[i + 3] >= 6 && dataText[i + 3] <= 36) && !saveChar)
                                    {
                                        byte[] fileSize = new byte[4];
                                        for (int h = 0; h < 4; h++)
                                        {
                                            fileSize[h] = dataText[i + 4 + h];
                                        }
                                        if ((BitConverter.ToInt32(fileSize) + i) > dataText.Length || BitConverter.ToInt32(fileSize) < 0)
                                        {
                                            output.WriteToOutput("Invalid File, data size: " + BitConverter.ToInt32(fileSize) + " at offset: " + i + ". Skipping CWS Flash file.");
                                            break;
                                        }
                                        if (!File.Exists(currentExport + "/CWS_export_" + index[1] + "." + desiredFileType))
                                        {
                                            writer = new FileStream(currentExport + "/CWS_export_" + index[1] + "." + desiredFileType, FileMode.Create);
                                        }
                                        await Task.Delay(TimeSpan.FromSeconds(0.05f));
                                        output.WriteToOutput("SWF File found. Version " + dataText[i + 3] + ", SWF uses ZLIB compression.", "Data size: " + BitConverter.ToInt32(fileSize) + ", offset: " + i);
                                        writer.Write(dataText, i, BitConverter.ToInt32(fileSize));
                                        Debug.WriteLine(".swf header detected. Writing data to file.");
                                        output.WriteToOutput("CWS_export_" + index[1] + "." + desiredFileType + " exported successfully.");
                                        index[1]++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                Debug.WriteLine("Filedata has been written to " + currentExport);
                output.WriteToOutput("------------------------------", index + " files have been extracted from the selected file.", "Export complete.");
                writer.Close();
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                output.WriteToOutput("EmbeddedFileRip: An error occurred while ripping file.", "Error : " + ex);
            }
        }

        public void UpdateFileSeek(string param1) // Code to alter the file extension we apply to the end of the exported file.
        {
            switch (param1)
            {
                case "PNG":
                    desiredFileType = ImageEnums.FileTypes.png;
                    break;
                case "JPG":
                    desiredFileType = ImageEnums.FileTypes.jpg;
                    break;
                case "PVR":
                    desiredFileType = ImageEnums.FileTypes.pvr;
                    break;
                case "DDS":
                    desiredFileType = ImageEnums.FileTypes.dds;
                    break;
                case "WAV":
                    desiredFileType = ImageEnums.FileTypes.wav;
                    break;
                case "SWF":
                    desiredFileType = ImageEnums.FileTypes.swf;
                    break;
            }
            output.WriteToOutput("Filetype option changed to " + desiredFileType, "Selected option: " + param1);
            Debug.WriteLine("Filetype Option changed to " + desiredFileType);
        }

        public void GetDirs(bool isFile)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckPathExists = true;
            // Set the Dialog Browser flags accordingly depending on if we're looking for a file or an export path, and open the File Dialog.
            if (isFile)
            {
                fileDialog.Title = "Open";
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
                    Debug.WriteLine("Selected file to rip set to: " + path);
                    dataText = File.ReadAllBytes(path);
                }
                else
                {
                    Debug.WriteLine("No file selected or file does not exist!");
                }
            }
            else
            {
                fileDialog.Title = "Export";
                fileDialog.ValidateNames = false;
                fileDialog.CheckFileExists = false;
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

    }
}
