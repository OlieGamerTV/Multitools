using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class ConstantInputName
    {
        public static string COLOR = "COLOR", DIFFUSE = "Diffuse", AMBIENT = "Ambient", SPECULAR = "Specular", TRANSPARENCY = "Transparency";
        private static List<string> RESERVED = new List<string> {COLOR,DIFFUSE,AMBIENT,SPECULAR,TRANSPARENCY};

        public ConstantInputName() : base()
        {
        }
    }
}
