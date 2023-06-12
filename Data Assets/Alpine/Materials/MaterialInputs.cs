using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    public class MaterialInputs
    {
        Material? material;
        Dictionary<string, List<float>>? constantInputMappings;
        Dictionary<string, int>? textureInputsMappings;
        Dictionary<string, float>? vertexAttributeInputsMappings;
        public int numGeneratedInputs;
        bool hasUVOffset;
        Dictionary<string, TextureCoordinateData>? textureCoordinateMapping;
        bool constantInputMappingsDirty, textureInputMappingsDirty, vertexAttributeInputMappingsDirty;
        bool modified;
        List<float> constantInputCache;
        List<int> textureInputCache;
        List<int> vertexAttributeInputCache;

        public MaterialInputs(Material? param1 = null) : base()
        {
            material = param1;
            textureInputsMappings = new Dictionary<string, int>();
            constantInputMappings = new Dictionary<string, List<float>>();
            vertexAttributeInputsMappings = new Dictionary<string, float>();
            constantInputMappingsDirty = true;
            textureInputMappingsDirty = true;
            vertexAttributeInputMappingsDirty = true;
        }

        public static MaterialInputs Default(Material param1)
        {
            var loc2 = new MaterialInputs(param1);
            loc2.MapConstantInput(ConstantInputName.COLOR, new Vector4(1, 1, 1, 1));
            loc2.MapConstantInput(ConstantInputName.DIFFUSE, new Vector4(0.8f, 0.8f, 0.8f, 1));
            loc2.MapConstantInput(ConstantInputName.AMBIENT, new Vector4(0.8f, 0.8f, 0.8f, 1));
            loc2.MapConstantInput(ConstantInputName.SPECULAR, new Vector4(0.35f, 0.35f, 0.35f, 10));
            return loc2;
        }

        public void MapConstantInput(string param1, Vector4? param2)
        {
            if(param2 == null)
            {
                return;
            }
            else
            {
                constantInputMappings[param1] = new List<float>(4){param2.Value.X, param2.Value.Y, param2.Value.Z, param2.Value.W};
            }
            constantInputMappingsDirty = true;
            modified = true;
        }

        public bool HasConstantInput(string param1)
        {
            return constantInputMappings.ContainsKey(param1);
        }

        public void MapTextureInput(string param1, int param2)
        {
            if (param2 == -1)
            {
                return;
            }
            else
            {
                this.textureInputsMappings[param1] = param2;
            }
            textureInputMappingsDirty = true;
            modified = true;
        }

        public void MapVertexAttributeInput(string param1, int param2)
        {
            if(param2 == -1)
            {
                vertexAttributeInputsMappings.Remove(param1);
            }
            else
            {
                vertexAttributeInputsMappings[param1] = param2;
            }
            vertexAttributeInputMappingsDirty = true;
            modified = true;
        }

        public void MapTextureCoordinate(string param1, int param2, string param3, int param4 = 0)
        {
            TextureCoordinateData loc5;
            if ((loc5 = textureCoordinateMapping[param1]) == null)
            {
                textureCoordinateMapping[param1] = loc5 = new TextureCoordinateData(param1);
            }
            loc5.id = param2;
            loc5.channels[param4] = param3;
            modified = true;
        }
    }
}
