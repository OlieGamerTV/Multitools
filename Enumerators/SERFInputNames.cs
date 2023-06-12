using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Enumerators
{
    internal class SERFInputNames
    {
        public static string MAGIC_RIFF = "52-49-46-46", 
            MAGIC_WAV = "57-41-56-45-66-6D-74-20", 
            MAGIC_AVI = "41-56-49-20-4C-49-53-54";
        public static string MAGIC_PNG = "89-50-4E-47-0D-0A-1A-0A";
        public static string MAGIC_JPG = "FF-D8-FF-E0", 
            MAGIC_JPG_EXIF = "FF-D8-FF-E1", 
            MAGIC_JPG_CIFF = "FF-D8-FF-E2", 
            MAGIC_JPG_SPIFF = "FF-D8-FF-E8";
        public static string MAGIC_BMP = "42-4D";
        public static string MAGIC_TGA = "54-52-55-45-56-49-53-49-4F-4E-2D-58-46-49-4C-45";
        public static string MAGIC_XML = "3C-70-72-6F-66-69-6C-65";
    }
}
