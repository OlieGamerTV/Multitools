using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class BlendShapeBase : Mesh
    {
        public List<BlendShape> blendShapes;
        public int numBlendShapes, numTargets;
        public bool blending, blendNormals = false, offsetBlendShapes;
        public List<float> bakedOffset, bakedBuffer;

        public BlendShapeBase(MeshData param2 = null) : base(param2)
        {
            blendShapes = null;
        }

        public BlendShape AddBlendShape(string param1, ModelRenderable param2, bool param3)
        {
            int loc5 = 0, loc6 = 0, loc7 = 0, loc8 = 0;
            RuntimeVertexData loc9 = null;
            ByteArray loc10 = null;
            if (blendShapes == null)
            {
                blendShapes = new List<BlendShape> ();
            }
            else if(HasBlendShape(param1))
            {
                return GetBlendShape(param1);
            }
            return null;
        }

        public bool HasBlendShape(string param1)
        {
            if (blendShapes == null)
            {
                return false;
            }
            int loc2 = blendShapes.Count, loc3 = 0;
            while (loc3 < loc2)
            {
                if (blendShapes[loc3].name == param1)
                {
                    return true;
                }
                loc3++;
            }
            return false;
        }

        public BlendShape GetBlendShape(string param1)
        {
            if (blendShapes == null)
            {
                return null;
            }
            int loc2 = blendShapes.Count, loc3 = 0;
            while (loc3 < loc2)
            {
                if (blendShapes[loc3].name == param1)
                {
                    return blendShapes[loc3];
                }
                loc3++;
            }
            return null;
        }

        public int NumActiveTargets()
        {
            if (blendShapes == null)
            {
                return 0;
            }
            int loc1 = 0, loc2 = blendShapes.Count, loc3 = 0;
            while (loc3 < loc2)
            {
                loc3++;
            }
            return loc1;
        }
    }
}
