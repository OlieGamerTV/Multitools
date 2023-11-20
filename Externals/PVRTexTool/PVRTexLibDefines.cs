using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Externals.PVRTexTool
{
    internal class PVRTexLibDefines
    {
        public const uint PVRTEX3_IDENT = 0x03525650U;
        public const uint PVRTEX3_IDENT_REV = 0x50565203U;
        public const uint PVRTEX_CURR_IDENT = PVRTEX3_IDENT;
        public const uint PVRTEX_CURR_IDENT_REV = PVRTEX3_IDENT_REV;
        public const ulong PVRTEX_PFHIGHMASK = 0xffffffff00000000ul;

        public enum PVRTexLibMetaData
        {
            PVRTLMD_TextureAtlasCoords = 0,
            PVRTLMD_BumpData,
            PVRTLMD_CubeMapOrder,
            PVRTLMD_TextureOrientation,
            PVRTLMD_BorderData,
            PVRTLMD_Padding,
            PVRTLMD_PerChannelType,
            PVRTLMD_SupercompressionGlobalData,
            PVRTLMD_MaxRange,
            PVRTLMD_NumMetaDataTypes
        };

        public enum PVRTexLibAxis
        {
            PVRTLA_X = 0,
            PVRTLA_Y = 1,
            PVRTLA_Z = 2
        };

        public enum PVRTexLibOrientation
        {
            PVRTLO_Left = 1 << PVRTexLibAxis.PVRTLA_X,
            PVRTLO_Right = 0,
            PVRTLO_Up = 1 << PVRTexLibAxis.PVRTLA_Y,
            PVRTLO_Down = 0,
            PVRTLO_Out = 1 << PVRTexLibAxis.PVRTLA_Z,
            PVRTLO_In = 0
        };

        public PVRTexLibOrientation pvrTexLibOrientation;

        public enum PVRTexLibColourSpace
        {
            PVRTLCS_Linear,
            PVRTLCS_sRGB,
            PVRTLCS_BT601,
            PVRTLCS_BT709,
            PVRTLCS_BT2020,
            PVRTLCS_NumSpaces
        };

        public enum PVRTexLibChannelName
        {
            PVRTLCN_NoChannel,
            PVRTLCN_Red,
            PVRTLCN_Green,
            PVRTLCN_Blue,
            PVRTLCN_Alpha,
            PVRTLCN_Luminance,
            PVRTLCN_Intensity,
            PVRTLCN_Depth,
            PVRTLCN_Stencil,
            PVRTLCN_Unspecified,
            PVRTLCN_NumChannels
        };

        public enum PVRTexLibPixelFormat
        {
            PVRTLPF_PVRTCI_2bpp_RGB,
            PVRTLPF_PVRTCI_2bpp_RGBA,
            PVRTLPF_PVRTCI_4bpp_RGB,
            PVRTLPF_PVRTCI_4bpp_RGBA,
            PVRTLPF_PVRTCII_2bpp,
            PVRTLPF_PVRTCII_4bpp,
            PVRTLPF_ETC1,
            PVRTLPF_DXT1,
            PVRTLPF_DXT2,
            PVRTLPF_DXT3,
            PVRTLPF_DXT4,
            PVRTLPF_DXT5,

            //These formats are identical to some DXT formats.
            PVRTLPF_BC1 = PVRTLPF_DXT1,
            PVRTLPF_BC2 = PVRTLPF_DXT3,
            PVRTLPF_BC3 = PVRTLPF_DXT5,
            PVRTLPF_BC4,
            PVRTLPF_BC5,

            /* Currently unsupported: */
            PVRTLPF_BC6,
            PVRTLPF_BC7,
            /* ~~~~~~~~~~~~~~~~~~ */

            // Packed YUV formats
            PVRTLPF_UYVY_422, // https://www.fourcc.org/pixel-format/yuv-uyvy/
            PVRTLPF_YUY2_422, // https://www.fourcc.org/pixel-format/yuv-yuy2/

            PVRTLPF_BW1bpp,
            PVRTLPF_SharedExponentR9G9B9E5,
            PVRTLPF_RGBG8888,
            PVRTLPF_GRGB8888,
            PVRTLPF_ETC2_RGB,
            PVRTLPF_ETC2_RGBA,
            PVRTLPF_ETC2_RGB_A1,
            PVRTLPF_EAC_R11,
            PVRTLPF_EAC_RG11,

            PVRTLPF_ASTC_4x4,
            PVRTLPF_ASTC_5x4,
            PVRTLPF_ASTC_5x5,
            PVRTLPF_ASTC_6x5,
            PVRTLPF_ASTC_6x6,
            PVRTLPF_ASTC_8x5,
            PVRTLPF_ASTC_8x6,
            PVRTLPF_ASTC_8x8,
            PVRTLPF_ASTC_10x5,
            PVRTLPF_ASTC_10x6,
            PVRTLPF_ASTC_10x8,
            PVRTLPF_ASTC_10x10,
            PVRTLPF_ASTC_12x10,
            PVRTLPF_ASTC_12x12,

            PVRTLPF_ASTC_3x3x3,
            PVRTLPF_ASTC_4x3x3,
            PVRTLPF_ASTC_4x4x3,
            PVRTLPF_ASTC_4x4x4,
            PVRTLPF_ASTC_5x4x4,
            PVRTLPF_ASTC_5x5x4,
            PVRTLPF_ASTC_5x5x5,
            PVRTLPF_ASTC_6x5x5,
            PVRTLPF_ASTC_6x6x5,
            PVRTLPF_ASTC_6x6x6,

            PVRTLPF_BASISU_ETC1S,
            PVRTLPF_BASISU_UASTC,

            PVRTLPF_RGBM,
            PVRTLPF_RGBD,

            PVRTLPF_PVRTCI_HDR_6bpp,
            PVRTLPF_PVRTCI_HDR_8bpp,
            PVRTLPF_PVRTCII_HDR_6bpp,
            PVRTLPF_PVRTCII_HDR_8bpp,

            // The memory layout for 10 and 12 bit YUV formats that are packed into a WORD (16 bits) is denoted by MSB or LSB:
            // MSB denotes that the sample is stored in the most significant <N> bits
            // LSB denotes that the sample is stored in the least significant <N> bits
            // All YUV formats are little endian

            // Packed YUV formats
            PVRTLPF_VYUA10MSB_444,
            PVRTLPF_VYUA10LSB_444,
            PVRTLPF_VYUA12MSB_444,
            PVRTLPF_VYUA12LSB_444,
            PVRTLPF_UYV10A2_444,    // Y410
            PVRTLPF_UYVA16_444,     // Y416
            PVRTLPF_YUYV16_422,     // Y216
            PVRTLPF_UYVY16_422,
            PVRTLPF_YUYV10MSB_422,  // Y210
            PVRTLPF_YUYV10LSB_422,
            PVRTLPF_UYVY10MSB_422,
            PVRTLPF_UYVY10LSB_422,
            PVRTLPF_YUYV12MSB_422,
            PVRTLPF_YUYV12LSB_422,
            PVRTLPF_UYVY12MSB_422,
            PVRTLPF_UYVY12LSB_422,

            /*
                Reserved for future expansion
            */

            // 3 Plane (Planar) YUV formats
            PVRTLPF_YUV_3P_444 = 270,
            PVRTLPF_YUV10MSB_3P_444,
            PVRTLPF_YUV10LSB_3P_444,
            PVRTLPF_YUV12MSB_3P_444,
            PVRTLPF_YUV12LSB_3P_444,
            PVRTLPF_YUV16_3P_444,
            PVRTLPF_YUV_3P_422,
            PVRTLPF_YUV10MSB_3P_422,
            PVRTLPF_YUV10LSB_3P_422,
            PVRTLPF_YUV12MSB_3P_422,
            PVRTLPF_YUV12LSB_3P_422,
            PVRTLPF_YUV16_3P_422,
            PVRTLPF_YUV_3P_420,
            PVRTLPF_YUV10MSB_3P_420,
            PVRTLPF_YUV10LSB_3P_420,
            PVRTLPF_YUV12MSB_3P_420,
            PVRTLPF_YUV12LSB_3P_420,
            PVRTLPF_YUV16_3P_420,
            PVRTLPF_YVU_3P_420,

            /*
                Reserved for future expansion
            */

            // 2 Plane (Biplanar/semi-planar) YUV formats
            PVRTLPF_YUV_2P_422 = 480,   // P208
            PVRTLPF_YUV10MSB_2P_422,    // P210
            PVRTLPF_YUV10LSB_2P_422,
            PVRTLPF_YUV12MSB_2P_422,
            PVRTLPF_YUV12LSB_2P_422,
            PVRTLPF_YUV16_2P_422,       // P216
            PVRTLPF_YUV_2P_420,         // NV12
            PVRTLPF_YUV10MSB_2P_420,    // P010
            PVRTLPF_YUV10LSB_2P_420,
            PVRTLPF_YUV12MSB_2P_420,
            PVRTLPF_YUV12LSB_2P_420,
            PVRTLPF_YUV16_2P_420,       // P016
            PVRTLPF_YUV_2P_444,
            PVRTLPF_YVU_2P_444,
            PVRTLPF_YUV10MSB_2P_444,
            PVRTLPF_YUV10LSB_2P_444,
            PVRTLPF_YVU10MSB_2P_444,
            PVRTLPF_YVU10LSB_2P_444,
            PVRTLPF_YVU_2P_422,
            PVRTLPF_YVU10MSB_2P_422,
            PVRTLPF_YVU10LSB_2P_422,
            PVRTLPF_YVU_2P_420,         // NV21
            PVRTLPF_YVU10MSB_2P_420,
            PVRTLPF_YVU10LSB_2P_420,

            //Invalid value
            PVRTLPF_NumCompressedPFs
        };

        public enum PVRTexLibVariableType
        {
            PVRTLVT_UnsignedByteNorm,
            PVRTLVT_SignedByteNorm,
            PVRTLVT_UnsignedByte,
            PVRTLVT_SignedByte,
            PVRTLVT_UnsignedShortNorm,
            PVRTLVT_SignedShortNorm,
            PVRTLVT_UnsignedShort,
            PVRTLVT_SignedShort,
            PVRTLVT_UnsignedIntegerNorm,
            PVRTLVT_SignedIntegerNorm,
            PVRTLVT_UnsignedInteger,
            PVRTLVT_SignedInteger,
            PVRTLVT_SignedFloat,
            PVRTLVT_Float = PVRTLVT_SignedFloat, //the name Float is now deprecated.
            PVRTLVT_UnsignedFloat,
            PVRTLVT_NumVarTypes,

            PVRTLVT_Invalid = 255
        };

        public enum PVRTexLibCompressorQuality
        {
            PVRTLCQ_PVRTCFastest = 0,   //!< PVRTC fastest
            PVRTLCQ_PVRTCFast,          //!< PVRTC fast
            PVRTLCQ_PVRTCLow,           //!< PVRTC low
            PVRTLCQ_PVRTCNormal,        //!< PVRTC normal
            PVRTLCQ_PVRTCHigh,          //!< PVRTC high
            PVRTLCQ_PVRTCVeryHigh,      //!< PVRTC very high
            PVRTLCQ_PVRTCThorough,      //!< PVRTC thorough
            PVRTLCQ_PVRTCBest,          //!< PVRTC best
            PVRTLCQ_NumPVRTCModes,      //!< Number of PVRTC modes

            PVRTLCQ_ETCFast = 0,        //!< ETC fast
            PVRTLCQ_ETCNormal,          //!< ETC normal
            PVRTLCQ_ETCSlow,            //!< ETC slow
            PVRTLCQ_NumETCModes,        //!< Number of ETC modes

            PVRTLCQ_ASTCVeryFast = 0,   //!< ASTC very fast
            PVRTLCQ_ASTCFast,           //!< ASTC fast
            PVRTLCQ_ASTCMedium,         //!< ASTC medium
            PVRTLCQ_ASTCThorough,       //!< ASTC thorough
            PVRTLCQ_ASTCExhaustive,     //!< ASTC exhaustive
            PVRTLCQ_NumASTCModes,       //!< Number of ASTC modes

            PVRTLCQ_BASISULowest = 0,   //!< BASISU lowest quality
            PVRTLCQ_BASISULow,          //!< BASISU low quality
            PVRTLCQ_BASISUNormal,       //!< BASISU normal quality
            PVRTLCQ_BASISUHigh,         //!< BASISU high quality
            PVRTLCQ_BASISUBest,         //!< BASISU best quality
            PVRTLCQ_NumBASISUModes,     //!< Number of BASISU modes
        };

        public enum PVRTexLibResizeMode
        {
            PVRTLRM_Nearest,    //!< Nearest filtering
            PVRTLRM_Linear,     //!< Linear filtering 
            PVRTLRM_Cubic,      //!< Cubic filtering, uses Catmull-Rom splines.
            PVRTLRM_Modes       //!< Number of resize modes
        };

        public enum PVRTexLibFileContainerType
        {
            PVRTLFCT_PVR,       //!< PVR: https://docs.imgtec.com/Specifications/PVR_File_Format_Specification/topics/pvr_intro.html
            PVRTLFCT_KTX,       //!< KTX version 1: https://www.khronos.org/registry/KTX/specs/1.0/ktxspec_v1.html 
            PVRTLFCT_KTX2,      //!< KTX version 2: https://github.khronos.org/KTX-Specification/
            PVRTLFCT_ASTC,      //!< ASTC compressed textures only: https://github.com/ARM-software/astc-encoder
            PVRTLFCT_BASIS,     //!< Basis Universal compressed textures only: https://github.com/BinomialLLC/basis_universal
            PVRTLFCT_DDS,       //!< DirectDraw Surface: https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-reference
            PVRTLFCT_CHeader    //!< C style header
        };

        public enum PVRTexLibColourDiffMode
        {
            PVRTLCDM_Abs,   //!< Absolute
            PVRTLCDM_Signed //!< Signed
        };

        public enum PVRTexLibLegacyApi
        {
            PVRTLLAPI_OGLES = 1, //!< OpenGL ES 1.x
            PVRTLLAPI_OGLES2,    //!< OpenGL ES 2.0
            PVRTLLAPI_D3DM,      //!< Direct 3D M
            PVRTLLAPI_OGL,       //!< Open GL
            PVRTLLAPI_DX9,       //!< DirextX 9
            PVRTLLAPI_DX10,      //!< DirectX 10
            PVRTLLAPI_OVG,       //!< Open VG
            PVRTLLAPI_MGL,       //!< MGL
        };

        public struct PVRTextureHeaderV3
        {
            public uint u32Version;          ///< Version of the file header, used to identify it.
            public uint u32Flags;            ///< Various format flags.
            public ulong u64PixelFormat;      ///< The pixel format, 8cc value storing the 4 channel identifiers and their respective sizes.
            public uint u32ColourSpace;      ///< The Colour Space of the texture, currently either linear RGB or sRGB.
            public uint u32ChannelType;      ///< Variable type that the channel is stored in. Supports signed/unsigned int/short/byte or float for now.
            public uint u32Height;           ///< Height of the texture.
            public uint u32Width;            ///< Width of the texture.
            public uint u32Depth;            ///< Depth of the texture. (Z-slices)
            public uint u32NumSurfaces;      ///< Number of members in a Texture Array.
            public uint u32NumFaces;     ///< Number of faces in a Cube Map. Maybe be a value other than 6.
            public uint u32MIPMapCount;      ///< Number of MIP Maps in the texture - NB: Includes top level.
            public uint u32MetaDataSize; ///< Size of the accompanying meta data.
        };
    }
}
