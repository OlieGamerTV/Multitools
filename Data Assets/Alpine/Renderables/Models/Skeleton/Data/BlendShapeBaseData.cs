using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class BlendShapeBaseData : MeshData
    {
        public List<BlendShapeData> blendShapes;
        public BlendShapeBaseData(dynamic param1 = null) : base()
        {
            blendShapes = new List<BlendShapeData>();
        }
        
        public BlendShapeData? GetBlendShapeByName(string param1)
        {
            foreach (BlendShapeData loc2 in blendShapes)
            {
                if (loc2.name == param1)
                {
                    return loc2;
                }
            }
            return null;
        }
    }
}
