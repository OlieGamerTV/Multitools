using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class VertexAttributeInputName
    {
        public static string VERTEX_COLOR = "VColor", VERTEX_NORMAL = "Normal", TEXTURE_COORDINATE = "_TEXC", VERTEX_TANGENT = "Tangent";
        public static List<string> RESERVED = new List<string> { VERTEX_COLOR, VERTEX_NORMAL, TEXTURE_COORDINATE, VERTEX_TANGENT };

        public VertexAttributeInputName() : base()
        {
        }
    }
}
