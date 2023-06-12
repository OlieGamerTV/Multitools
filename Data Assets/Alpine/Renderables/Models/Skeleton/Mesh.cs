using Alpine.Geom;
using Alpine.Materials;
using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Scene;
using Alpine.Scene.Renderer;
using Alpine.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class Mesh
    {
        private static AlpineVector3D tempVecV1 = new AlpineVector3D();
        private static AlpineVector3D tempVecV2 = new AlpineVector3D();
        private static AlpineVector3D tempVecV3 = new AlpineVector3D();
        private static AlpineVector3D tempVecU = new AlpineVector3D();
        private static AlpineVector3D tempVecV = new AlpineVector3D();
        private static AlpineVector3D tempVecN = new AlpineVector3D();
        private static AlpineVector3D tempVecW = new AlpineVector3D();
        private static AlpineVector3D tempVecIT = new AlpineVector3D();
        private static int ids = 0;
        protected int id;
        public string name, culling;
        public MeshData data;
        public Material material;
        public MaterialInputs materialInputs;
        public RuntimeVertexData vertexData;
        public int vertexDataOffset, indicesId = -1, programId, programIndex = -1, maxInfluences;
        public ByteArray indices;
        public bool renderable, externallyGeneratedNormals = false, visible;
        public string programName, programDepthName, programTestName, programShadowName, programNormalsName, programLinearDepthName, programMotionBlurOffsetName, programMotionBlurFinalName, programImageSpaceShadowName, programOutlineName;
        public List<int> joints;
        public List<float> jointTable;
        public Point uvOffset;

        public Mesh(MeshData param2 = null) : base()
        {
            id = ids++;
            vertexDataOffset = -1;
            visible = true;
            programId = -1;
            if (param2 != null)
            {
                Debug.WriteLine("Mesh: ID - [" + id + "], Vertex Data Offset - [" + vertexDataOffset + "], Visible? - [" + visible.ToString() + "], Program ID - [" + programId + "], Data has been found. Building...");
                Build(param2);
            }
            else
            {
                Debug.WriteLine("Mesh: ID - [" + id + "], Vertex Data Offset - [" + vertexDataOffset + "], Visible? - [" + visible.ToString() + "], Program ID - [" + programId + "], No data was found.");
            }
        }

        public void Build(MeshData param2)
        {
            data = param2;
            name = param2.name;
            indicesId = param2.indicesId;
            indices = param2.indices;
            renderable = param2.renderable;
            culling = !!param2.backfaceCulling ? IRendererConstants.CULLING_BACK : IRendererConstants.CULLING_NONE;
            joints = param2.joints;
            maxInfluences = param2.maxInfluences;
            Debug.WriteLine("Mesh.Build: Model built - [" + name + "], Indices ID - [" + indicesId + "], Renderable? - [" + renderable.ToString() + "], Culling - [" + culling + "], Max Influences - [" + maxInfluences + "].");
        }

        public void BuildProgramNames(string param1)
        {
            this.programDepthName = param1 + "-depth";
            this.programTestName = param1 + "-test";
            this.programShadowName = param1 + "-shadow";
            this.programNormalsName = param1 + "-normals";
            this.programLinearDepthName = param1 + "-linear.depth";
            this.programMotionBlurOffsetName = param1 + "-motion.blur.offset";
            this.programMotionBlurFinalName = param1 + "-motion.blur.final";
            this.programImageSpaceShadowName = param1 + "-image.space.shadow";
            this.programOutlineName = param1 + "-outline";
        }

        public void Destroy()
        {
            joints = null;
            indices = null;
            jointTable = null;
            material = null;
            materialInputs = null;
        }

        public void TranslateJoints(bool param1)
        {
            int loc4 = 0, loc5 = 0;
            int loc6 = 0;
            List<float> loc7 = null;
            if (!vertexData.hasAnimationData)
            {
                return;
            }
            vertexData.jointsUpdated = true;
            if (vertexData.translatedJoints == null)
            {
                vertexData.translatedJoints = AlpineUtils.CreateByteArray(vertexData.originalJoints);
            }
            ByteArray loc2 = vertexData.originalJoints, loc3 = vertexData.translatedJoints;
            if (param1)
            {
                indices.Position = 0;
                loc5 = (int)(indices.Length / 2);
                loc4 = 0;
                while (loc4 < loc5)
                {
                    loc6 = (indices.ReadShort() * 4 * 4);
                    loc2.Position = loc6;
                    loc3.Position = loc6;
                    loc3.WriteFloat(loc2.ReadFloat() * 2 + 28);
                    loc3.WriteFloat(loc2.ReadFloat() * 2 + 28);
                    loc3.WriteFloat(loc2.ReadFloat() * 2 + 28);
                    loc3.WriteFloat(loc2.ReadFloat() * 2 + 28);
                    loc4++;
                }
            }
            else
            {
                loc5 = joints[joints.Count - 1] + 1;
                loc7 = jointTable = new List<float>(loc5);
                loc4 = 0;
                while (loc4 < loc5)
                {
                    loc7.Insert(loc4, -1);
                    loc4++;
                }
                loc5 = joints.Count;
                loc4 = 0;
                while (loc4 < loc5)
                {
                    loc7[joints[loc4]] = loc4 * 2 + 28;
                    loc4++;
                }
                if (loc7[0] == -1)
                {
                    loc7[0] = 0;
                }
                indices.Position = 0;
                loc5 = (int)(indices.Length / 2);
                loc4 = 0;
                while (loc4 < loc5)
                {
                    loc6 = (indices.ReadShort() * 4);
                    loc3.Position = loc6 * 4;
                    loc2.Position = loc6 * 4;
                    loc3.WriteFloat(loc7[Convert.ToInt32(loc2.ReadFloat())]);
                }
            }
        }
    }
}
