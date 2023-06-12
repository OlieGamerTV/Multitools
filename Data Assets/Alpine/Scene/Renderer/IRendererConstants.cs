namespace Alpine.Scene.Renderer;
public interface IRendererConstants
{
    //Context3DTriangleFace
    public static string CULLING_BACK = "back", CULLING_FRONT = "front", CULLING_NONE = "none";
    //Context3DCompareMode
    public static string COMPARE_ALWAYS = "always", COMPARE_NEVER = "never";
    public static string COMPARE_EQUAL = "equal", COMPARE_NOT_EQUAL = "notEqual";
    public static string COMPARE_GREATER = "greater", COMPARE_GREATER_EQUAL = "greaterEqual";
    public static string COMPARE_LESS = "less", COMPARE_LESS_EQUAL = "lessEqual";
    //Context3DBlendFactor
    public static string BLENDFACTOR_ONE = "one", BLENDFACTOR_ZERO = "zero";
    public static string BLENDFACTOR_DESTINATION_ALPHA = "destinationAlpha", BLENDFACTOR_DESTINATION_COLOR = "destinationColor";
    public static string BLENDFACTOR_ONE_MINUS_DESTINATION_ALPHA = "oneMinusDestinationAlpha", BLENDFACTOR_ONE_MINUS_DESTINATION_COLOR = "oneMinusDestinationColor";
    public static string BLENDFACTOR_SOURCE_ALPHA = "sourceAlpha", BLENDFACTOR_SOURCE_COLOR = "sourceColor";
    public static string BLENDFACTOR_ONE_MINUS_SOURCE_ALPHA = "oneMinusSourceAlpha";
    //Context3DProgramType
    public static string PROGRAM_VERTEX = "vertex", PROGRAM_FRAGMENT = "fragment";
    //Context3DVertexBufferFormat
    public static string VERTEXBUFFER_FLOAT1 = "float1", VERTEXBUFFER_FLOAT2 = "float2", VERTEXBUFFER_FLOAT3 = "float3", VERTEXBUFFER_FLOAT4 = "float4";
    //IRendererConstants
    public static int CUBEMAP_WHOLE = -1;
    public static int CUBEMAP_X_POSITIVE = 0, CUBEMAP_X_NEGATIVE = 1;
    public static int CUBEMAP_Y_POSITIVE = 2, CUBEMAP_Y_NEGATIVE = 3;
    public static int CUBEMAP_Z_POSITIVE = 4, CUBEMAP_Z_NEGATIVE = 5;
    public static int ANTIALIAS_NONE = 0, ANTIALIAS_LOW = 2, ANTIALIAS_HIGH = 4, ANTIALIAS_MAX = 16;
    public static string TEXTURE_UNCOMPRESSED = "bgra";
    public static string TEXTURE_COMPRESSED = "compressed";
    public static string TEXTURE_COMPRESSED_ALPHA = "compressedAlpha";
    public static string TEXTURE_BGR_PACKED = "bgrPacked565";
    public static string TEXTURE_BGRA_PACKED = "bgraPacked4444";
}