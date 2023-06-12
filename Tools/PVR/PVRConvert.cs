using Microsoft.Win32;
using Multi_Tool.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

internal class PVRConvert
{
    private byte[] dataText;
    private string exportPath, fileName, filePath;
    private PVRTypes.PVRFileTypes pvrFileType;
    private PVRTypes pvrStrings = new PVRTypes();
    private bool isRunning;
    public bool useOtherPixel = false;
    uint curPixelFormat;
    OutputLog outputLog;
    ComboBox pixelFormatList, pvrFileList, pvrDesiredPixList;

    public PVRConvert(ListView output = null, ComboBox pixel = null, ComboBox file = null, ComboBox desiredPixel = null) : base()
    {
        outputLog = new OutputLog(output);
        pixelFormatList = pixel;
        pvrFileList = file;
        pvrDesiredPixList = desiredPixel;
    }

    public string[] SetCurrentPixFormat(int curDesiredVersion)
    {
        string[] tempNames;
        switch (curDesiredVersion)
        {
            case -1:
                return null;
            case 0:
                tempNames = pvrStrings.pixelFormatListPVR3;
                return tempNames;
            case 1:
            case 2:
                tempNames = pvrStrings.pixelFormatListPVRLegacy;
                return tempNames;
            case 3:
                tempNames = pvrStrings.pixelFormatListPVRDreamcast;
                return tempNames;
            default:
                return null;

        }
    }

    private void SetPixelFormats()
    {
        pixelFormatList.Items.Clear();
        switch (pvrFileType)
        {
            case PVRTypes.PVRFileTypes.NOFILE:
                return;
            case PVRTypes.PVRFileTypes.PVRDREAMCAST:
                for (int i = 0; i < pvrStrings.pixelFormatListPVRDreamcast.Length; i++)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = pvrStrings.pixelFormatListPVRDreamcast[i];
                    item.Tag = i;
                    pixelFormatList.Items.Add(item);
                }
                pixelFormatList.SelectedIndex = (int)curPixelFormat;
                break;
            case PVRTypes.PVRFileTypes.PVRVER1:
            case PVRTypes.PVRFileTypes.PVRVER2:
                for (int i = 0; i < pvrStrings.pixelFormatListPVRLegacy.Length; i++)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = pvrStrings.pixelFormatListPVRLegacy[i];
                    item.Tag = i;
                    pixelFormatList.Items.Add(item);
                }
                pixelFormatList.SelectedIndex = (int)curPixelFormat;
                break;
            case PVRTypes.PVRFileTypes.PVRVER3:
                for (int i = 0; i < pvrStrings.pixelFormatListPVR3.Length; i++)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = pvrStrings.pixelFormatListPVR3[i];
                    item.Tag = i;
                    pixelFormatList.Items.Add(item);
                }
                pixelFormatList.SelectedIndex = (int)curPixelFormat;
                break;
        }
        outputLog.WriteToOutput("Current Pixel Format = " + curPixelFormat + " / " + pixelFormatList.Text);
    }

    private void CheckFileVer()
    {
        FileStream reader = new FileStream(filePath, FileMode.Open);
        byte[] headerTemp = new byte[4];
        for (int i = 0; i < headerTemp.Length; i++)
        {
            headerTemp[i] = dataText[i];
        }
        uint header = BitConverter.ToUInt32(headerTemp, 0);
        outputLog.WriteToOutput("Header found: " + header);
        byte[] pixelTemp;
        switch (header)
        {
            default:
                outputLog.WriteToOutput("Inserted file is not a PVR file.");
                pvrFileType = PVRTypes.PVRFileTypes.NOFILE;
                pvrFileList.SelectedIndex = 0;
                break;
            case 44:
                outputLog.WriteToOutput("PVR file is Version 1.");
                pvrFileType = PVRTypes.PVRFileTypes.PVRVER1;
                pvrFileList.SelectedIndex = 3;
                pixelTemp = new byte[1];
                pixelTemp[0] = dataText[16];
                curPixelFormat = BitConverter.ToUInt32(pixelTemp, 0);
                outputLog.WriteToOutput("Current Pixel Format = " + curPixelFormat);
                break;
            case 52:
                outputLog.WriteToOutput("PVR file is Version 2.");
                pvrFileType = PVRTypes.PVRFileTypes.PVRVER2;
                pvrFileList.SelectedIndex = 2;
                pixelTemp = new byte[1];
                pixelTemp[0] = dataText[16];
                curPixelFormat = BitConverter.ToUInt32(pixelTemp, 0);
                outputLog.WriteToOutput("Current Pixel Format = " + curPixelFormat);
                break;
            case 55727696:
            case 1347834371:
                outputLog.WriteToOutput("PVR file is Version 3.");
                pvrFileList.SelectedIndex = 1;
                pixelTemp = new byte[8];
                for (int i = 0; i < pixelTemp.Length; i++)
                {
                    pixelTemp[i] = dataText[i + 8];
                }
                curPixelFormat = BitConverter.ToUInt32(pixelTemp, 0);
                pvrFileType = PVRTypes.PVRFileTypes.PVRVER3;
                break;
            case 1414682192:
                outputLog.WriteToOutput("PVR file is for Sega Dreamcast.");
                pvrFileList.SelectedIndex = 4;
                pixelTemp = new byte[4];
                pixelTemp[0] = dataText[8];
                curPixelFormat = BitConverter.ToUInt32(pixelTemp, 0);
                pvrFileType = PVRTypes.PVRFileTypes.PVRDREAMCAST;
                break;
        }
        SetPixelFormats();
        reader.Dispose();
        reader.Close();
    }

    public async void StartConvert()
    {
        if (!isRunning)
        {
            outputLog.WriteToOutput("Starting Ripping Coroutine...");
            await ConvertFileToPVR3();
        }
        else
        {
            outputLog.WriteToOutput("Ripping Coroutine is already running.");
        }
    }

    public async Task<byte[]> ConvertFileToPVR3()
    {
        FileStream writer = new FileStream(exportPath + "/" + fileName + "_converted.pvr", FileMode.Create);
        int index = 0;
        byte[] newHeader = new byte[52];
        await Task.Delay(TimeSpan.FromSeconds(0.2f));

        // Getting the necessary information for our header from the file
        outputLog.WriteToOutput("Getting info from the selected file");
        await Task.Delay(TimeSpan.FromSeconds(1f));
        int t = 0;
        byte[] pixelFormat = new byte[8], height = new byte[4], width = new byte[4], mipMapCount = new byte[4], numSurf = new byte[4], defOne = new byte[4];
        defOne[0] = 1; defOne[1] = 0; defOne[2] = 0; defOne[3] = 0;
        switch (pvrFileType)
        {
            case PVRTypes.PVRFileTypes.PVRDREAMCAST:
                Buffer.BlockCopy(dataText, 8, pixelFormat, 0, 1);
                Buffer.BlockCopy(dataText, 12, width, 0, 2);
                Buffer.BlockCopy(dataText, 14, height, 0, 2);
                mipMapCount[0] = 1;
                numSurf[0] = 1;
                break;
            case PVRTypes.PVRFileTypes.PVRVER1:
                Buffer.BlockCopy(dataText, 4, height, 0, 4);
                Buffer.BlockCopy(dataText, 8, width, 0, 4);
                Buffer.BlockCopy(dataText, 12, mipMapCount, 0, 4);
                if (BitConverter.ToUInt32(mipMapCount, 0) == 0)
                {
                    mipMapCount[0] = 1;
                }
                Buffer.BlockCopy(dataText, 16, pixelFormat, 0, 1);
                Buffer.BlockCopy(dataText, 48, numSurf, 0, 4);
                break;
            case PVRTypes.PVRFileTypes.PVRVER2:
                Buffer.BlockCopy(dataText, 4, height, 0, 4);
                Buffer.BlockCopy(dataText, 8, width, 0, 4);
                Buffer.BlockCopy(dataText, 12, mipMapCount, 0, 4);
                if (BitConverter.ToUInt32(mipMapCount, 0) == 0)
                {
                    mipMapCount[0] = 1;
                }
                Buffer.BlockCopy(dataText, 16, pixelFormat, 0, 1);
                Buffer.BlockCopy(dataText, 48, numSurf, 0, 4);
                break;
        }

        // Prepare our new PVR file header for use
        outputLog.WriteToOutput("Preparing new header.");
        await Task.Delay(TimeSpan.FromSeconds(1f));
        newHeader[0] = 80; newHeader[1] = 86; newHeader[2] = 82; newHeader[3] = 3;
        if (useOtherPixel)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(pvrDesiredPixList.SelectedIndex), 0, pixelFormat, 0, 4);
        }
        else
        {
            Buffer.BlockCopy(pixelFormat, 0, newHeader, 8, 1);
            if (BitConverter.ToUInt64(pixelFormat) > 50)
            {
                pixelFormat[0] = 0;
            }
        }
        Buffer.BlockCopy(height, 0, newHeader, 24, 4);
        Buffer.BlockCopy(width, 0, newHeader, 28, 4);
        Buffer.BlockCopy(defOne, 0, newHeader, 32, 4);
        Buffer.BlockCopy(numSurf, 0, newHeader, 36, 4);
        Buffer.BlockCopy(defOne, 0, newHeader, 40, 4);
        Buffer.BlockCopy(mipMapCount, 0, newHeader, 44, 4);
        Buffer.BlockCopy(newHeader, 0, dataText, 972, 52);
        outputLog.WriteToOutput(BitConverter.ToString(newHeader));

        // Write our new file data
        outputLog.WriteToOutput("Writing data to file.");
        await Task.Delay(TimeSpan.FromSeconds(1f));
        if (pvrFileType == PVRTypes.PVRFileTypes.PVRDREAMCAST)
        {
            for (int i = 972; i < dataText.Length; i++)
            {
                writer.WriteByte(dataText[i]);
            }
        }
        writer.Flush();
        writer.Close();
        outputLog.WriteToOutput("Data wrote to file.");
        return null;
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
                filePath = path;
                Debug.WriteLine("Selected path: " + filePath);
                string file = Path.GetFileNameWithoutExtension(path);
                Debug.WriteLine("Selected file: " + file);
                fileName = file;
                Debug.WriteLine("Selected file to rip set to: " + path);
                dataText = File.ReadAllBytes(path);
                CheckFileVer();
                return;
            }
            else
            {
                Debug.WriteLine("No file selected or file does not exist!");
                return;
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
                return;
            }
            else
            {
                Debug.WriteLine("Selected directory does not exist!");
                return;
            }
        }
    }
}
