using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class TextureInputName
    {
        public static string COLOR_MAP = "COLOR", DIFFUSE_MAP = "Diffuse", NORMAL_MAP = "NormalMap", SPECULAR_MAP = "Specular", RAMP_MAP = "Toon", NORMAL_AND_SPECULAR_MAP = "Normal_Specular";
        private static List<string> RESERVED = new List<string> { COLOR_MAP, DIFFUSE_MAP, NORMAL_MAP, SPECULAR_MAP, RAMP_MAP, NORMAL_AND_SPECULAR_MAP };

        public TextureInputName() : base() { }
    }
}
