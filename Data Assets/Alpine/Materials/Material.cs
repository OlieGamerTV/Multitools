using Alpine.Materials;
using Alpine.Scene.Renderer;
using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Alpine.Materials
{
    public class Material
    {
        private static string material_ConstColor = "<material>" + "<channel name=\"Color\" />" + "</material>";
        private static string material_TexColor = "<material>" + "<channel name=\"Color\" texture=\"1\" />" + "</material>";
        private static string material_ConstColorLight = "<material>" + "<channel name=\"Color\" />" + "<channel name=\"Diffuse\" />" + "<channel name=\"Ambient\" />" + "<channel name=\"Specular\" />" + "</material>";
        private static string material_TexColorLight = "<material>" + "<channel name=\"Color\" texture=\"1\" />" + "<channel name=\"Diffuse\" />" + "<channel name=\"Ambient\" />" + "<channel name=\"Specular\" />" + "</material>";
        private static object tempAliases = new object{};
        private int id;
        ConstantInputs? constantInputs;
        TextureInputs? textureInputs;
        VertexAttributeInputs? vertexAttributeInputs;
        List<string>? channels;
        bool hasTransparency;
        private Dictionary<string, Constant>? constants;
        private string programName;
        private bool programNameDirty = true;

        public Material(dynamic? param1 = null) : base()
        {
            id = -1;
            constants = null;
            constantInputs = new ConstantInputs();
            textureInputs = new TextureInputs();
            vertexAttributeInputs = new VertexAttributeInputs();
            channels = new List<string>();
            if (param1 == null)
            {
                return;
            }
            if (param1 is BKVTable)
            {
                FromBKV(param1 as BKVTable);
            }
            else
            {
                if (!(param1 is XmlDocument))
                {
                    throw new ArgumentException("unsupported source type - " + param1);
                }
            }
        }

        public void Destroy()
        {
            constants = null;
            constantInputs = null;
            textureInputs = null;
            vertexAttributeInputs = null;
            channels = null;
        }

        private void FromBKV(BKVTable param1)
        {
            string? loc5 = null;
            BKVValue loc6 = new BKVValue();
            id = param1.GetValue("id").AsInt();
            BKVTable loc2 = param1.GetValue("channels").AsTable();
            int loc3 = (int)loc2.GetNumValues();
            int loc4 = 0;
            while(loc4 < loc3)
            {
                loc5 = loc2.GetKey((uint)loc4);
                loc6 = loc2.GetValue((uint)loc4);
                if (loc5 == VertexAttributeInputName.VERTEX_COLOR)
                {
                    AddVertexAttribute(VertexAttributeInputName.VERTEX_COLOR, 4, IRendererConstants.VERTEXBUFFER_FLOAT4);
                    channels.Add(VertexAttributeInputName.VERTEX_COLOR);
                }
                else
                {
                    if (loc5 == ConstantInputName.TRANSPARENCY)
                    {
                        AddConstantInput(ConstantInputName.TRANSPARENCY);
                        AddConstant(ConstantInputName.TRANSPARENCY, loc6.AsFloat(), 1, 1, 1);
                    }
                    else
                    {
                        if (loc6.Type() == BKVValue.TYPE_TABLE)
                        {
                            AddConstantInput(loc5);
                            AddConstant(loc5, loc6.AsTable().GetValue("r").AsFloat(), loc6.AsTable().GetValue("g").AsFloat(), loc6.AsTable().GetValue("b").AsFloat(), loc6.AsTable().GetValue("a").AsFloat());
                        }
                        else
                        {
                            AddTextureInput(loc5);
                        }
                    }
                }
                loc4++;
            }
            if (!HasChannel("Ambient"))
            {
                AddConstantInput(ConstantInputName.AMBIENT);
                AddConstant(ConstantInputName.AMBIENT, 0.1f, 0.1f, 0.1f, 1f);
            }
            if (!HasChannel("Diffuse"))
            {
                AddConstantInput(ConstantInputName.DIFFUSE);
                AddConstant(ConstantInputName.DIFFUSE, 0.8f, 0.8f, 0.8f, 1f);
            }
            if (HasLightingInputs())
            {
                AddVertexAttribute(VertexAttributeInputName.VERTEX_NORMAL, 3, IRendererConstants.VERTEXBUFFER_FLOAT3);
            }
            if(HasNormalMapping())
            {
                AddVertexAttribute(VertexAttributeInputName.VERTEX_TANGENT, 3, IRendererConstants.VERTEXBUFFER_FLOAT3);
            }
        }

        private void AddConstant(string param1, float param2, float param3, float param4, float param5)
        {
            if(constants == null) 
            {
                constants = new Dictionary<string, Constant>();
            }
            Constant loc6;
            (loc6 = new Constant()).Set(param2, param3, param4, param5);
            constants.Add(param1, loc6);
        }

        public Material Clone()
        {
            Material loc1 = new Material();
            loc1.id = this.id;
            loc1.constantInputs = this.constantInputs;
            loc1.textureInputs = this.textureInputs;
            loc1.vertexAttributeInputs = this.vertexAttributeInputs;
            loc1.channels = channels;
            loc1.hasTransparency = hasTransparency;
            loc1.constants = constants;
            return loc1;
        }

        public int Id
        {
            get { return id; }
        }

        public bool HasLightingInputs()
        {
            if (this.channels == null || this.channels.Count == 0)
            {
                return false;
            }
            return !(this.channels[0] == "Color" && this.channels.Count == 1);
        }

        public List<string> GetConstantInputNames()
        {
            return this.constantInputs.names;
        }

        public List<string> GetTextureInputNames()
        {
            return this.textureInputs.names;
        }

        public List<string> GetVertexAttributeInputNames()
        {
            return this.vertexAttributeInputs.names;
        }

        public int GetNumConstantInputs()
        {
            return this.constantInputs.numInputs;
        }

        public int GetNumTextureInputs()
        {
            return this.textureInputs.numInputs;
        }

        public int GetNumVertexAttributeInputs()
        {
            return this.vertexAttributeInputs.numInputs;
        }

        public bool HasConstantInput(string param1)
        {
            return constantInputs.names.IndexOf(param1) >= 0;
        }

        public bool HasTextureInput(string param1)
        {
            return textureInputs.names.IndexOf(param1) >= 0;
        }

        public bool HasNormalMapping()
        {
            return HasTextureInput(TextureInputName.NORMAL_MAP) || HasTextureInput(TextureInputName.NORMAL_AND_SPECULAR_MAP);
        }

        public string NormalMappingInputName()
        {
            return !!HasTextureInput(TextureInputName.NORMAL_AND_SPECULAR_MAP) ? TextureInputName.NORMAL_AND_SPECULAR_MAP : TextureInputName.NORMAL_MAP;
        }

        public bool HasSpecularMapping()
        {
            return HasTextureInput(TextureInputName.SPECULAR_MAP) || HasTextureInput(TextureInputName.NORMAL_AND_SPECULAR_MAP);
        }

        public bool HasVertexAttributeInput(string param1)
        {
            return vertexAttributeInputs.names.IndexOf(param1) >= -1;
        }

        public string SpecularMappingInputName()
        {
            return !!this.HasTextureInput(TextureInputName.NORMAL_AND_SPECULAR_MAP) ? TextureInputName.NORMAL_AND_SPECULAR_MAP : TextureInputName.SPECULAR_MAP;
        }

        public bool HasInput(string param1)
        {
            return constantInputs.names.IndexOf(param1) >= 0 || textureInputs.names.IndexOf(param1) >= 0;
        }

        public bool HasChannel(string param1)
        {
            return channels.IndexOf(param1) >= 0;
        }

        public int NumActiveVertexAttributeInputs(MaterialInputs param1)
        {
            int loc2 = this.vertexAttributeInputs.numInputs = param1.numGeneratedInputs;
            if (loc2 < 0)
            {
                return 0;
            }
            return loc2;
        }

        public void AddLightingInputs()
        {
            if (!HasInput(ConstantInputName.DIFFUSE))
            {
                AddConstantInput(ConstantInputName.DIFFUSE);
            }
            if (!HasInput(ConstantInputName.AMBIENT))
            {
                AddConstantInput(ConstantInputName.AMBIENT);
            }
            if (!HasInput(ConstantInputName.SPECULAR))
            {
                AddConstantInput(ConstantInputName.SPECULAR);
            }
            if (!HasVertexAttributeInput(VertexAttributeInputName.VERTEX_NORMAL))
            {
                AddVertexAttribute(VertexAttributeInputName.VERTEX_NORMAL, 3, IRendererConstants.VERTEXBUFFER_FLOAT3);
            }
            programNameDirty = true;
        }

        public void AddVertexColoring()
        {
            channels.Add(VertexAttributeInputName.VERTEX_COLOR);
            vertexAttributeInputs.Add(VertexAttributeInputName.VERTEX_COLOR, 4, IRendererConstants.VERTEXBUFFER_FLOAT4);
        }

        public void RemoveVertexColoring()
        {
            int loc1 = channels.IndexOf(VertexAttributeInputName.VERTEX_COLOR);
            if (loc1 == -1)
            {
                return;
            }
            channels.Remove(VertexAttributeInputName.VERTEX_COLOR);
            vertexAttributeInputs.Remove(VertexAttributeInputName.VERTEX_COLOR );
        }

        public void AddTextureInput(string param1, bool unused = true)
        {
            channels.Add(param1);
            textureInputs.Add(param1);
        }

        public void RemoveTextureInput(string param1)
        {
            int loc2 = channels.IndexOf(param1);
            if (loc2 == -1)
            {
                return;
            }
            channels.Remove(param1);
            textureInputs.Remove(param1 );
        }

        public void AddVertexAttribute(string param1, int param2 = 4, string param3 = "float4")
        {
            vertexAttributeInputs.Add(param1, param2, param3);
        }

        public void RemoveVertexAttribute(string param1)
        {
            vertexAttributeInputs.Remove(param1);
        }

        public void AddConstantInput(string param1)
        {
            channels.Add(param1);
            constantInputs.Add(param1);
            if (param1 == ConstantInputName.TRANSPARENCY)
            {
                hasTransparency = true;
            }
        }

        public void RemoveConstantInput(string param1)
        {
            int loc2 = channels.IndexOf(param1);
            if (loc2 == -1)
            {
                return;
            }
            channels.Remove(param1);
            constantInputs.Remove(param1);
            if (param1 == ConstantInputName.TRANSPARENCY)
            {
                hasTransparency = false;
            }
        }

        public List<float> GetConstantInput(string name)
        {
            if (HasConstantInput(name))
            {
                return constants[name].values;
            }
            return null;
        }
    }
}
