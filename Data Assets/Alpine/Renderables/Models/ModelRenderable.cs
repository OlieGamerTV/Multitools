global using Alpine;
using Alpine.Geom;
using Alpine.Materials;
using Alpine.Renderables.Models.Skeleton;
using Alpine.Renderables.Models.Skeleton.Data;
using Alpine.Scene;
using Alpine.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models
{
    internal class ModelRenderable : Renderable
    {
        protected static string VERTEX_SHADER_MESH_STATIC = "m44 v_Position, v_Position, u_ModelMatrix\t\t\t\t\n" + "m44 t_PositionT, v_Position, u_ViewProjectionMatrix\t\n";
        protected static string FRAGMENT_SHADER_SOURCE = "";
        protected static string VERTEX_SHADER_VARY_NORM_STATIC = "m33 t_Normal.xyz, a_Normal, u_NormalMatrix\t\n";
        protected static string SHADER_SHADOW_VERT = "mov objPosition, v_Position\t\t\t\n" + "mov op, t_PositionT\t\t\t\t\t\n";
        protected static string SHADER_SHADOW_FRAG = "sub vToCamera.xyz, objPosition.xyz, fc1.xyz\t\t\n" + "dp3 vToCamera.x, vToCamera.xyz, vToCamera.xyz\t\t\n" + "sqt vToCamera.x, vToCamera.x\t\t\t\t\t\t\n" + "div vToCamera.x, vToCamera.x, fc1.w\t\t\t\t\n" + "min vToCamera.x, vToCamera.x, fc0.x\t\t\t\t\n" + "mul vFraction1.xyzw, vToCamera.xxxx, fc0.xyzw\t\t\n" + "frc vFraction1.xyzw, vFraction1.xyzw\t\t\t\t\t\n" + "div vRemainder.xyzw, vFraction1.yzww, fc0.yyyy\t\t\t\n" + "sub vFraction1.xyzw, vFraction1.xyzw, vRemainder.xyzw\t\n" + "mov oc, vFraction1\t\t\t\t\t\t\t\t\n";
        protected static string SHADER_TEST_VERT = "m44 testPos, v_Position, vc4\t\t\n" + "mov objPosition, v_Position\t\t\n" + "mov op, t_PositionT\t\t\t\t\n";
        protected static string SHADER_TEST_FRAG = "div ft0, shadowCoord, v0.wwww\t\n" + "mul ft0.xy, ft0.xy, fc1.xx\t\t\n" + "add ft0.xy, ft0.xy, fc1.xx\t\t\n" + "sub ft0.y, fc0.x, ft0.y\t\t\t\n" + "tex ft1, ft0, fs0 <2d,clamp,linear>\t\n" + "dp3 ft2.x, ft1, fc0\t\t\t\t\n" + "sub ft3, objPosition, fc2\t\t\n" + "dp3 ft3.x, ft3.xyz, ft3.xyz\t\t\n" + "sqt ft3.x, ft3.x\t\t\t\t\t\n" + "div ft3.x, ft3.x, fc2.w\t\t\t\n" + "sub ft3.x, ft3.x, fc3.x\t\t\t\n" + "sge ft3, ft2.x, ft3.x\t\t\t\n" + "sge ft4.x, ft0.x, fc1.w\t\t\n" + "sge ft4.y, ft0.y, fc1.w\t\t\n" + "mul ft4.x, ft4.x, ft4.y\t\t\n" + "sub ft4.w, fc0.x, ft0.x\t\t\n" + "sge ft4.y, ft4.w, fc1.w\t\t\n" + "mul ft4.x, ft4.x, ft4.y\t\t\n" + "sub ft4.w, fc0.x, ft0.y\t\t\n" + "sge ft4.y, ft4.w, fc1.w\t\t\n" + "mul ft4.x, ft4.x, ft4.y\t\t\n" + "mul ft3, ft3, ft4.xxxx\t\n" + "mov oc, ft3\t\t\t\n";
        protected static string SHADER_NORMALS_VERT = "mov op, t_PositionT\t\t\t\t\t\t\n" + "m33 v0.xyz, vaNormal, matObjectToWorld\t\n" + "mov v0.w, matObjectToWorld.x\t\t\t\t\n";
        protected static string SHADER_NORMALS_FRAG = "nrm tNormal.xyz, v0.xyz\t\t\t\t\n" + "add tNormal, tNormal.xyz, cConst.xxx\t\n" + "mul tNormal, tNormal, cConst.yyy\t\t\n" + "mov oc, tNormal\t\t\t\t\t\t\n";
        protected static string SHADER_LINEAR_DEPTH_VERT = "mov varyPosition, v_Position\t\t\n" + "mov op, t_PositionT\t\t\t\t\n";
        protected static string SHADER_LINEAR_DEPTH_FRAG = "sub vToCamera.xyz, varyPosition.xyz, vCameraPos.xyz\t\t\n" + "dp3 vToCamera.x, vToCamera.xyz, vToCamera.xyz\t\t\t\t\n" + "sqt vToCamera.x, vToCamera.x\t\t\t\t\t\t\t\t\n" + "div vToCamera.x, vToCamera.x, vCameraPos.w\t\t\t\t\n" + "min vToCamera.x, vToCamera.x, cFractionMult.x\t\t\t\t\n" + "mul vFraction1.xyzw, vToCamera.xxxx, cFractionMult.xyzw\t\n" + "frc vFraction1.xyzw, vFraction1.xyzw\t\t\t\t\t\t\n" + "div vRemainder.xyz, vFraction1.yzw, cFractionMult.yyy\t\t\n" + "sub vFraction1.xyzw, vFraction1.xyz, vRemainder.xyz\t\t\n" + "mov oc, vFraction1\t\t\t\t\t\t\t\t\t\t\n";
        protected static string SHADER_MOTION_BLUR_OFFSET_VERT = "m44 posPrev, a_Position, matPrev\t\t\t\n" + "m33 normal.xyz, vaNormal, u_ModelMatrix\t\n" + "sub motion, v_Position, posPrev\t\t\t\n" + "nrm motion.xyz, motion.xyz\t\t\t\t\n" + "nrm normal.xyz, normal.xyz\t\t\t\t\n" + "dp3 motion.w, motion.xyz, normal.xyz\t\t\n" + "sge normal.w, ZeroX1, motion.w\t\t\t\n" + "mov vPosCur, t_PositionT\t\t\t\t\t\n" + "m44 posPrev, a_Position, mvpPrev\t\t\t\n" + "mov vPosPrev, posPrev\t\t\t\t\t\t\n" + "mul posPrev, posPrev, normal.w\t\t\t\n" + "sub normal.w, OneX1, normal.w\t\t\t\t\n" + "mul posCur, t_PositionT, normal.w\t\t\t\n" + "add posPrev, posCur, posPrev\t\t\t\t\n" + "mov op, posPrev\t\t\t\t\t\t\t\n" + "mov posPrev.z, motion.w\t\t\t\t\t\n" + "mov vStretch, posPrev\t\t\t\t\t\t\n";
        protected static string SHADER_MOTION_BLUR_OFFSET_FRAG = "div posCur, vPosCur, vPosCur.w\t\t\t\n" + "add coord.xy, posCur.xy, OneX1\t\t\n" + "mul coord.xy, coord.xy, HalfX1\t\t\n" + "sub coord.y, OneX1, coord.y\t\t\t\n" + "tex color, coord.xy, fs0 <2d,clamp,linear>\t\n" + "div posPrev.xy, vPosPrev.xy, vPosPrev.w\t\n" + "add posPrev.xy, posPrev.xy, OneX1\t\t\t\n" + "mul posPrev.xy, posPrev.xy, HalfX1\t\t\n" + "sub posPrev.y, OneX1, posPrev.y\t\t\t\n" + "add posPrev.xy, coord.xy, posPrev.xy\t\t\n" + "mul posPrev.xy, posPrev.xy, HalfX1\t\t\n" + "tex posPrev, posPrev.xy, fs0 <2d,clamp,linear>\t\n" + "sge posCur.z, ZeroX1, color.w\t\t\t\n" + "add posCur.y, posCur.z, posPrev.w\t\t\n" + "add posCur.z, posCur.y, color.w\t\t\n" + "div posCur.x, color.w, posCur.z\t\t\n" + "div posCur.y, posCur.y, posCur.z\t\t\n" + "mul color.xyz, color.xyz, posCur.x\t\n" + "mul posPrev.xyz, posPrev.xyz, posCur.y\t\n" + "add color.xyz, posPrev.xyz, color.xyz\t\n" + "max color.w, color.w, posPrev.w\t\t\n" + "add posPrev.w, vStretch.z, OneX1\t\t\n" + "mul posPrev.w, posPrev.w, sample7\t\t\n" + "min posPrev.w, posPrev.w, OneX1\t\t\n" + "mul color.w, color.w, posPrev.w\t\t\t\n" + "mov oc, color\t\t\t\t\t\t\t\n";
        protected static string SHADER_MOTION_BLUR_FINAL_VERT = "m44 posPrev, a_Position, matPrev\t\t\t\n" + "m33 normal.xyz, vaNormal, u_ModelMatrix\t\n" + "sub motion, v_Position, posPrev\t\t\t\n" + "nrm motion.xyz, motion.xyz\t\t\t\t\n" + "nrm normal.xyz, normal.xyz\t\t\t\t\n" + "dp3 motion.w, motion.xyz, normal.xyz\t\t\n" + "sge normal.w, ZeroX1, motion.w\t\t\t\n" + "m44 posPrev, a_Position, mvpPrev\t\t\t\n" + "mul posPrev, posPrev, normal.w\t\t\t\n" + "sub normal.w, OneX1, normal.w\t\t\t\t\n" + "mul posCur, t_PositionT, normal.w\t\t\t\n" + "add posPrev, posCur, posPrev\t\t\t\t\n" + "mov op, posPrev\t\t\t\t\t\t\t\n" + "mov vStretch, posPrev\t\t\t\t\t\t\n";
        protected static string SHADER_MOTION_BLUR_FINAL_FRAG = "div coord.xy, vStretch.xy, vStretch.w\t\t\n" + "add coord.xy, coord.xy, OneX1\t\t\t\t\n" + "mul coord.xy, coord.xy, HalfX1\t\t\t\n" + "sub coord.y, OneX1, coord.y\t\t\t\t\n" + "tex color, coord.xy, fs0 <2d,clamp,linear>\t\n" + "mov oc, color\t\t\t\t\t\t\t\n";
        protected static List<float> shadowConstants = new List<float>(4);
        protected static List<float> constants = new List<float>{0, 1, 2, 255};
        //protected static const __fragBuilder:ShaderBuilder = new ShaderBuilder(ShaderBuilder.TYPE_FRAG,true);
        //protected static const __vertBuilder:ShaderBuilder = new ShaderBuilder(ShaderBuilder.TYPE_VERT,true);
        protected static AlpineVector3D tempVec = new AlpineVector3D();
        //private static const __meshRay:Ray = new Ray();
        //private static const __meshRayHit:RayHit = new RayHit();
        protected string programNamePrefix;
        protected RuntimeModelData data;
        protected List<MergedMesh> mergedMeshes;
        protected List<Mesh> activeMeshes;
        //protected var __lights:Lights;
        List<Lod> lods;
        protected bool complexTransparency, meshesUpdated, programsUpdated, programNameHasChanged = true, renderAfterUpdate = false, trackDraws;
        public bool affectedByLights;
        //protected var __environmentMapping:EnvironmentMapping;
        //protected var __projectiveTexture:ProjectiveTexture;
        private List<PackedTexture> modifiedTextures;
        protected IBoundingVolume originalBoundingVolume;
        protected AlpineMatrix3D lastTransform, drawMatrix;
        //protected var __dualParaboloidReflection:DualParaboloidReflection;
        protected List<IBoundingVolume> boundingVolumes;
        protected List<string> draws;

        public ModelRenderable(ModelData param2)
        {
            int loc3 = 0, loc4 = 0;
            data = new RuntimeModelData(param2);
            programNamePrefix = "model" + id;
            drawMatrix = new AlpineMatrix3D();
            List<Material> loc5 = this.data.Materials;
            loc4 = loc5.Count;
            loc3 = 0;
            while(loc3 < loc4)
            {
                if (((!(loc5[loc3] == null)) && ((loc5[loc3]).HasChannel(ConstantInputName.TRANSPARENCY))))
                {
                    base.transparency = true;
                    break;
                }
                loc3++;
            }
            List<Mesh> loc6 = data.Meshes;
            loc4 = loc6.Count;
            loc3 = 0;
            while (loc3 < loc4)
            {
                loc6[loc3].BuildProgramNames(programNamePrefix);
                loc3++;
            }
            List<LodData> loc7 = param2.lods;
            if ((!(loc7 == null)) && (loc7.Count > 0))
            {
                loc4 = loc7.Count;
                lods = new List<Lod>(loc4);
                loc3 = 0;
                while (loc3 < loc4)
                {
                    lods.Insert(loc3, new Lod(loc7[loc3], this));
                    loc3++;
                }
            }
            List<IBoundingVolume> loc8 = param2.boundingVolumes;
            if (((!(loc8 == null)) && (loc8.Count > 0)))
            {
                loc4 = loc8.Count;
                boundingVolumes = new List<IBoundingVolume>(loc4);
                loc3 = 0;
                while (loc3 < loc4)
                {
                    boundingVolumes.Insert(loc3, loc8[loc3].Clone());
                    loc3++;
                }
            }
            mergedMeshes = new List<MergedMesh>();
            programsUpdated = true;
            meshesUpdated = true;
            complexTransparency = false;
        }

        public RuntimeModelData Data
        {
            get { return data; }
        }

        public List<Mesh> Meshes
        {
            get { return data.Meshes; }
        }

        public void TrackDraws(bool arg1)
        {
            if (arg1)
            {
                trackDraws = true;
                draws = new List<string>();
            }
            else
            {
                trackDraws = false;
                draws = null;
            }
        }

        public List<string> Draws
        {
            get { return draws; }
        }

        protected string GetProgramNameForMesh(Mesh arg1, int arg2)
        {
            switch (arg2)
            {
                case Scene3D.RENDER_MODE_NORMAL:
                    return (arg1.programName);
                case Scene3D.RENDER_MODE_SHADOW:
                    return arg1.programShadowName;
                case Scene3D.RENDER_MODE_TEST:
                    return (arg1.programTestName);
                case Scene3D.RENDER_MODE_DEPTH_BUFFER:
                    return (arg1.programDepthName);
                case Scene3D.RENDER_MODE_NORMALS:
                    return (arg1.programNormalsName);
                case Scene3D.RENDER_MODE_LINEAR_DEPTH:
                    return (arg1.programLinearDepthName);
                case Scene3D.RENDER_MODE_MOTION_BLUR_OFFSET:
                    return (arg1.programMotionBlurOffsetName);
                case Scene3D.RENDER_MODE_MOTION_BLUR_FINAL:
                    return (arg1.programMotionBlurFinalName);
                case Scene3D.RENDER_MODE_IMAGE_SPACE_SHADOW:
                    return (arg1.programImageSpaceShadowName);
                case Scene3D.RENDER_MODE_OUTLINE:
                    return (arg1.programOutlineName);
                default:
                    return "NOPROGRAMFORYOU";
            }
        }

        protected string GenerateProgramNameForMesh()
        {
            return "mesh";
        }

        public Mesh GetMesh(string arg1)
        {
            if (data.Meshes == null || data.Meshes.Count < 1)
            {
                return null;
            }
            foreach (Mesh loc2 in data.Meshes)
            {
                if (loc2.name == arg1)
                {
                    return loc2;
                }
            }
            return null;
        }

        public void SetMesh(string arg1, bool arg2, bool arg3 = true)
        {
            Mesh loc4 = GetMesh(arg1);
            if(loc4 == null)
            {
                return;
            }
            if (loc4.visible == arg2)
            {
                return;
            }
            loc4.visible = arg2;
            if (!arg3)
            {
                meshesUpdated = true;
            }
            if (!meshesUpdated)
            {
                if (arg2)
                {

                }
            }
        }

        public void HideAllMeshes()
        {
            foreach (Mesh loc1 in data.Meshes)
            {
                loc1.visible = false;
            }
            meshesUpdated = true;
        }

        public void SetMeshTransparency(string arg1, float arg2)
        {
            Mesh loc3 = GetMesh(arg1);
            if (loc3 == null)
            {
                Debug.Fail("Couldn't find mesh - " + arg1);
                return;
            }
            base.transparency = true;
        }
    }
}
