using Alpine.Materials;
using Multi_Tool.Tools.BKV;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class MeshData
    {
        public string? name;
        public int id, indicesId, materialId, vertexDataId, maxInfluences;
        public ByteArray? indices;
        public int indicesAmount;
        public Material? material;
        public VertexData? vertexData;
        public bool renderable, backfaceCulling, hasLOD;
        public List<int>? joints;

        public MeshData(dynamic? param1 = null) : base()
        {
            renderable = true;
            if (param1 == null)
            {
                return;
            }
            if (param1 is BKVTable)
            {
                FromBKV(param1);
            }
        }

        private void FromBKV(BKVTable param1)
        {
            this.id = param1.GetValue("id").AsInt();
            this.name = param1.GetValue("name").AsString();
            this.vertexDataId = param1.GetValue("vert").AsInt();
            this.renderable = !param1.GetValue("nonrendered").AsBool();
            if(this.renderable)
            {
                this.materialId = param1.GetValue("material").AsInt();
                this.backfaceCulling = param1.GetValue("bfculling").AsBool();
                this.maxInfluences = param1.GetValue("influences").AsInt();
            }
        }

        public void Destroy()
        {
            indices = null;
            material = null;
            vertexData = null;
            joints = null;
        }

        public void Build(ByteArray param1)
        {
            indices = param1;
        }
    }
}
