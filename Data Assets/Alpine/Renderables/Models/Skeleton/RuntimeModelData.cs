using Alpine.Materials;
using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class RuntimeModelData
    {
        private List<ModelData> datas;
        private List<RuntimeVertexData> vertexDatas;
        private List<PackedTexture> textures, extTextures, allTextures;
        private List<Mesh> meshes, extMeshes, allMeshes;
        private List<Material> materials, extMaterials, allMaterials;

        public RuntimeModelData(ModelData param2) : base()
        {
            int loc3 = 0, loc4 = 0, loc5 = 0;
            Mesh loc6 = null;
            MeshData loc7 = null;
            datas = new List<ModelData>();
            datas.Add(param2);
            textures = param2.textures;
            allTextures = param2.textures;
            materials = param2.materials;
            loc5 = this.materials.Count;
            loc3 = 0;
            while (loc3 < loc5)
            {
                materials[loc3] = this.materials[loc3].Clone();
                loc3++;
            }
            allMaterials = materials;
            vertexDatas = new List<RuntimeVertexData>();
            loc5 = param2.vertexDatas.Count;
            loc3 = 0;
            while (loc3 < loc5)
            {
                vertexDatas.Add(new RuntimeVertexData(param2, param2.vertexDatas[loc3]));
                loc3++;
            }
            meshes = new List<Mesh>();
            loc5 = param2.meshes.Count;
            loc3 = 0;
            while (loc3 < loc5)
            {
                loc7 = param2.meshes[loc3];
                if (param2.IsBlendShapeBase(loc7.name))
                {
                }
            }
        }

        public List<ModelData> Datas
        {
            get { return datas; }
        }

        public List<Material> Materials
        {
            get { return materials; }
        }

        public List<Mesh> Meshes
        {
            get { return meshes; }
        }

        public List<RuntimeVertexData> VertexDatas
        {
            get { return vertexDatas; }
        }

        public List<PackedTexture> Textures
        {
            get { return allTextures; }
        }
    }
}
