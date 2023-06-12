using Utilities.Flash;
using System;
using Alpine.Materials;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class VertexData
    {
        public ByteArray vertices, normals, tangents, vcolors, weights, joints;
        public Dictionary<string, TextureCoordinateData> texCoords;
        public Dictionary<string, string>? textures;
        public bool hasAnimationData;
        public int id, verticesId = -1, normalsId = -1, tangentsId = -1, vcolorsId = -1, weightsId = -1, jointsId = -1;

        public VertexData() : base()
        {
            id = -1;
            hasAnimationData = false;
        }

        public int numVertices
        {
            get { return (int)vertices.Length / (3 * 4); }
        }

        public void Destroy()
        {
            vertices = null;
            normals = null;
            tangents = null;
            vcolors = null;
            texCoords = null;
            textures = null;
            weights = null;
            joints = null;
        }

        public void Build(ByteArray param1, ByteArray param2, ByteArray param3, ByteArray param4)
        {
            vertices = param1;
            normals = param2;
            tangents = param3;
            texCoords = new Dictionary<string, TextureCoordinateData>();
            if(param4 != null)
            {
                texCoords[VertexAttributeInputName.TEXTURE_COORDINATE] = new TextureCoordinateData(VertexAttributeInputName.TEXTURE_COORDINATE, param4);
            }
        }
    }
}
