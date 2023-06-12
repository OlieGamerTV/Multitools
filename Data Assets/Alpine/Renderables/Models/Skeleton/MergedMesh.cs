using Alpine.Scene.Renderer;
using Alpine.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class MergedMesh : Mesh
    {
        public bool active, modified, mergeJoints;
        public List<Mesh> mergedMeshes;

        public MergedMesh(int param2) : base()
        {
            name = "merged" + param2;
            programId = -1;
            renderable = true;
            culling = IRendererConstants.CULLING_BACK;
            visible = true;
            indices = null;
            joints = null;
            this.active = false;
            this.modified = false;
            this.mergeJoints = false;
            this.mergedMeshes = null;
        }

        public static int SortJoints(int param1, int param2)
        {
            return param1 - param2;
        }

        public new void Destroy()
        {
            base.Destroy();
            mergedMeshes = null;
        }

        public bool Merge(Mesh param1)
        {
            int loc2 = 0, loc4 = 0;
            bool loc3 = false;
            if (mergedMeshes == null)
            {
                mergedMeshes = new List<Mesh>();
            }
            else if(mergedMeshes.IndexOf(param1) != -1)
            {
                return true;
            }
            this.active = true;
            culling = param1.culling;
            material = param1.material;
            materialInputs = param1.materialInputs;
            vertexData = param1.vertexData;
            uvOffset = param1.uvOffset;
            if (maxInfluences < param1.maxInfluences)
            {
                maxInfluences = param1.maxInfluences;
            }
            if (mergeJoints && param1.joints != null)
            {
                if (joints == null)
                {
                    joints = param1.joints;
                }
                else
                {
                    loc2 = param1.joints.Count;
                    loc3 = false;
                    loc4 = 0;
                    while (loc4 < loc2)
                    {
                        if (joints.IndexOf(param1.joints[loc4]) == -1)
                        {
                            joints.Add(param1.joints[loc4]);
                            loc3 = true;
                        }
                        loc4++;
                    }
                    if (loc3)
                    {
                        joints.Sort(SortJoints);
                    }
                }
            }
            if (indices == null)
            {
                indices = AlpineUtils.CreateByteArray(param1.indices);
            }
            else
            {
                if (param1.indices != null)
                {
                    indices.Position = indices.Length;
                    indices.Length += param1.indices.Length;
                    indices.WriteBytes(param1.indices);
                }
            }
            mergedMeshes.Add(param1);
            modified = true;
            return true;
        }
    }
}
