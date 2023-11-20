using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Multi_Tool.Externals.PVRTexTool.PVRTexLibDefines;

namespace Multi_Tool.Externals.PVRTexTool
{
    struct MetaDataBlock
    {
        uint DevFOURCC;
        uint u32Key;
        uint u32DataSize;
        byte[] Data;

        public MetaDataBlock(uint DevFOURCC = PVRTEX_CURR_IDENT, uint u32Key = 0, uint u32DataSize = 0, byte[] Data = null)
        {
            this.DevFOURCC = DevFOURCC;
            this.u32Key = u32Key;
            this.u32DataSize = u32DataSize;
            this.Data = Data;
        }
    }

    struct PVRHeader_CreateParams
    {
        ulong pixelFormat;     ///< pixel format
        uint width;           ///< texture width
        uint height;          ///< texture height
        uint depth;           ///< texture depth
        uint numMipMaps;      ///< number of MIP maps
        uint numArrayMembers; ///< number of array members
        uint numFaces;        ///< number of faces
        PVRTexLibColourSpace colourSpace;     ///< colour space
        PVRTexLibVariableType channelType;     ///< channel type
        bool preMultiplied;   ///< has the RGB been pre-multiplied by the alpha?
    };

    struct PVRTexLib_OpenGLFormat
    {
        uint internalFormat;  ///< GL internal format
        uint format;          ///< GL format
        uint type;            ///< GL type
    };

    class PVRTextureHeader
    {
        public PVRTextureHeader()
        {
            PVRHeader_CreateParams @params;

        }

        public PVRTextureHeader(PVRHeader_CreateParams[] headerParams) { }

        public PVRTextureHeader(ulong pixelFormat, uint width, uint height, uint depth = 1U, uint numMipMaps = 1U, uint numArrayMembers = 1U, uint numFaces = 1U, PVRTexLibColourSpace colourSpace = PVRTexLibColourSpace.PVRTLCS_sRGB, PVRTexLibVariableType channelType = PVRTexLibVariableType.PVRTLVT_UnsignedByteNorm, bool preMultiplied = false) { }
    }

    internal class PVRTexLib
    {
        public const string PVRLibDLLLoc = @".\PVRTexLib.dll";
        PVRTextureHeaderV3[] PVRTexLib_PVRTextureHeader;

        public static PVRTextureHeaderV3 ReadPVRHeader(byte[] data)
        { 
            MemoryStream fileData = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(fileData);
            PVRTextureHeaderV3 pvrHeader = new PVRTextureHeaderV3();
            pvrHeader.u32Version = reader.ReadUInt32();
            if (pvrHeader.u32Version == 0x03525650 || pvrHeader.u32Version == 0x50565203)
            {
                pvrHeader.u32Flags = reader.ReadUInt32();
                pvrHeader.u64PixelFormat = reader.ReadUInt64();
                pvrHeader.u32ColourSpace = reader.ReadUInt32();
                pvrHeader.u32ChannelType = reader.ReadUInt32();
                pvrHeader.u32Height = reader.ReadUInt32();
                pvrHeader.u32Width = reader.ReadUInt32();
                pvrHeader.u32Depth = reader.ReadUInt32();
                pvrHeader.u32NumSurfaces = reader.ReadUInt32();
                pvrHeader.u32NumFaces = reader.ReadUInt32();
                pvrHeader.u32MIPMapCount = reader.ReadUInt32();
                pvrHeader.u32MetaDataSize = reader.ReadUInt32();
            }
            return pvrHeader;
        }

        public static PVRTextureHeaderV3 ReadPVRHeader(Stream data)
        {
            BinaryReader reader = new BinaryReader(data);
            PVRTextureHeaderV3 pvrHeader = new PVRTextureHeaderV3();
            pvrHeader.u32Version = reader.ReadUInt32();
            if (pvrHeader.u32Version == 0x03525650 || pvrHeader.u32Version == 0x50565203)
            {
                pvrHeader.u32Flags = reader.ReadUInt32();
                pvrHeader.u64PixelFormat = reader.ReadUInt64();
                pvrHeader.u32ColourSpace = reader.ReadUInt32();
                pvrHeader.u32ChannelType = reader.ReadUInt32();
                pvrHeader.u32Height = reader.ReadUInt32();
                pvrHeader.u32Width = reader.ReadUInt32();
                pvrHeader.u32Depth = reader.ReadUInt32();
                pvrHeader.u32NumSurfaces = reader.ReadUInt32();
                pvrHeader.u32NumFaces = reader.ReadUInt32();
                pvrHeader.u32MIPMapCount = reader.ReadUInt32();
                pvrHeader.u32MetaDataSize = reader.ReadUInt32();
            }
            return pvrHeader;
        }

        public static Dictionary<int, byte[]> ReadPVRTextureData(byte[] data)
        {
            Dictionary<int, byte[]> pvrData = new Dictionary<int, byte[]>();
            MemoryStream imageData = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(imageData);
            PVRTextureHeaderV3 pvrHeader = ReadPVRHeader(reader.ReadBytes(52));
            reader.BaseStream.Position += pvrHeader.u32MetaDataSize;
            for (int i = 0; i < pvrHeader.u32MIPMapCount; i++)
            {
                for (int j = 0; j < pvrHeader.u32NumSurfaces; j++)
                {
                    for (int k = 0; k < pvrHeader.u32NumFaces; k++)
                    {
                        uint dataSize = ((pvrHeader.u32Width * pvrHeader.u32Height) * pvrHeader.u32Depth) / 2;
                        pvrData.Add(k, reader.ReadBytes((int)dataSize));
                    }
                }
            }
            return pvrData;
        }

        public static Dictionary<int, byte[]> ReadPVRTextureData(byte[] data, PVRTextureHeaderV3 pvrCurrHeader)
        {
            Dictionary<int, byte[]> pvrData = new Dictionary<int, byte[]>();
            MemoryStream imageData = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(imageData);
            PVRTextureHeaderV3 pvrHeader = pvrCurrHeader;
            reader.BaseStream.Position = 52 + pvrHeader.u32MetaDataSize;
            for (int i = 0; i < pvrHeader.u32MIPMapCount; i++)
            {
                for (int j = 0; j < pvrHeader.u32NumSurfaces; j++)
                {
                    for (int k = 0; k < pvrHeader.u32NumFaces; k++)
                    {
                        uint dataSize = ((pvrHeader.u32Width * pvrHeader.u32Height) * pvrHeader.u32Depth) / 2;
                        pvrData.Add(k, reader.ReadBytes((int)dataSize));
                    }
                }
            }
            return pvrData;
        }
    }
}
