using Alpine.Materials;
using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class RuntimeVertexData
    {
        private static int Ids = 0;
        private int id;
        private List<MergedVertexData> datas;
        private int vertexBufferId = -1, normalBufferId = -1, tangentBufferId = -1, vcolorBufferId = -1, weightsBufferId = -1, jointsBufferId = -1;
        private Dictionary<string, TextureCoordinateData> texCoords;
        public Dictionary<string, PackedTexture> textures;
        public MaterialInputs materialInputs;
        public bool hasAnimationData, jointsUpdated;
        public ByteArray originalJoints, translatedJoints;
        public int numVertices;

        public RuntimeVertexData(ModelData param2, VertexData param3) : base()
        {
            dynamic loc5 = null;
            id = Ids++;
            datas = new List<MergedVertexData>();
            datas.Add(new MergedVertexData(param3));
            vertexBufferId = param3.verticesId;
            normalBufferId = param3.normalsId;
            tangentBufferId = param3.tangentsId;
            vcolorBufferId = param3.vcolorsId;
            weightsBufferId = param3.weightsId;
            jointsBufferId = param3.jointsId;
            if (param3.hasAnimationData)
            {
                originalJoints = param3.joints;
                translatedJoints = null;
            }
            if (param3.textures != null)
            {
                textures = new Dictionary<string, PackedTexture>();
                foreach (dynamic loc4 in param3.textures)
                {
                    textures[loc4] = param2.GetPackedTexture(param3.textures[loc4]);
                }
            }
            if (param3.texCoords != null)
            {
                texCoords = new Dictionary<string, TextureCoordinateData>();
                foreach (dynamic loc4 in param3.textures)
                {
                    texCoords[loc4] = param2.GetPackedTexture(param3.texCoords[loc4]);
                }
            }
            hasAnimationData = param3.hasAnimationData;
            materialInputs = new MaterialInputs();
            numVertices = param3.numVertices;
        }

        public int Id
        {
            get { return id; }
        }

        public VertexData GetData(int param1 = 0)
        {
            if (datas == null || param1 < 0 || param1 >= datas.Count)
            {
                return null;
            }
            return datas[param1].data;
        }

        public int GetDataJoint(int param1 = 0)
        {
            if (datas == null || param1 < 0 || param1 >= datas.Count)
            {
                return -1;
            }
            return datas[param1].joints;
        }

        public Matrix4x4 GetDataOffset(int param1 = 0)
        {
            if (datas == null || param1 < 0 || param1 >= datas.Count)
            {
                return new Matrix4x4();
            }
            return datas[param1].offset;
        }

        public bool HasData
        {
            get { return datas != null && datas.Count > 0; }
        }

        public int NumDatas
        {
            get { return datas == null ? 0 : datas.Count; }
        }

        public int GetVertexOffset(VertexData param1)
        {
            MergedVertexData loc5 = null;
            if (datas == null)
            {
                return -1;
            }
            int loc2 = 0, loc3 = datas.Count, loc4 = 0;
            while (loc4 < loc3)
            {
                if((loc5 = this.datas[loc4]).data == param1)
                {
                    break;
                }
                loc2 += loc5.data.numVertices;
                loc4++;
            }
            return loc2;
        }

        public bool IsModified
        {
            get { return vertexBufferId != datas[0].data.verticesId; }
        }

        private void UpdateMaterialInputs()
        {
            TextureCoordinateData loc3 = null;
            if(materialInputs == null)
            {
                return;
            }
            if (vcolorBufferId != -1)
            {
                materialInputs.MapVertexAttributeInput(VertexAttributeInputName.VERTEX_COLOR, vcolorBufferId);
            }
            if (normalBufferId != -1)
            {
                materialInputs.MapVertexAttributeInput(VertexAttributeInputName.VERTEX_NORMAL, normalBufferId);
            }
            if (tangentBufferId != -1)
            {
                materialInputs.MapVertexAttributeInput(VertexAttributeInputName.VERTEX_TANGENT, tangentBufferId);
            }
            if (texCoords != null)
            {
                if (this.texCoords[VertexAttributeInputName.TEXTURE_COORDINATE] != null)
                {
                    materialInputs.MapTextureCoordinate(VertexAttributeInputName.TEXTURE_COORDINATE, texCoords[VertexAttributeInputName.TEXTURE_COORDINATE].id, VertexAttributeInputName.TEXTURE_COORDINATE, 0);
                }
                else
                {
                    foreach(dynamic loc1 in textures)
                    {
                        materialInputs.MapTextureInput(loc1, textures[loc1].id);
                    }
                    foreach (dynamic loc2 in texCoords)
                    {
                        loc3 = texCoords[loc2];
                        materialInputs.MapTextureCoordinate(loc2, loc3.id, loc3.channels[0], 0);
                        if (loc3.size > 2)
                        {
                            materialInputs.MapTextureCoordinate(loc2, loc3.id, loc3.channels[1], 1);
                        }
                    }
                }
            }
        }
    }

    internal class MergedVertexData
    {
        public VertexData data;
        public int joints;
        public Matrix4x4 offset;

        public MergedVertexData(VertexData param1) : base()
        {
            data = param1;
            joints = -1;
        }
    }
}

    

