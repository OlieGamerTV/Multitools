using Alpine.Geom;
using Alpine.Renderables.Models.Skeleton;
using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models
{
    internal class AnimatedModelRenderable : ModelRenderable
    {
        private const string VERTEX_SHADER_BLENDSHAPE_HEADER = "mov v_Out, a_Base \n";
        private const string VERTEX_SHADER_BLENDSHAPE_SEG = ("mul t_Target, a_Target, c_BST \n" + "add v_Out.xyz, v_Out.xyz, t_Target.xyz \n");
        private const string VERTEX_SHADER_MESH_ANIM_INIT_POSITION = ((("sub t_Pos1, a_Position, a_Position \n" + "mov t_Pos2, t_Pos1 \n") + "mov v_PositionAnim, a_Position \n") + "mov v_PositionAnim.xyz, t_Pos1.xyz \n");
        private const string VERTEX_SHADER_MESH_ANIM_INIT_NORMAL = "mov t_NormalAnim, v_PositionAnim \n";
        private const string VERTEX_SHADER_MESH_ANIM_INIT_TANGENT = "mov t_TangentAnim, v_PositionAnim \n";
        private const string VERTEX_SHADER_MESH_ANIM_POS_SEG = ((((((((((("mov t_Translation, vc[ a_Index ] \n" + "mov t_Rotation, vc[ a_Index + 1 ] \n") + "mul t_Pos1, v_PositionSrc, t_Rotation.w \n") + "crs t_Pos2.xyz, t_Rotation, v_PositionSrc \n") + "add t_Pos1, t_Pos1, t_Pos2 \n") + "crs t_Pos1.xyz, t_Rotation, t_Pos1 \n") + "add t_Pos1, t_Pos1, t_Pos1 \n") + "add t_Pos1, t_Pos1, v_PositionSrc \n") + "mul t_Pos1, t_Pos1, t_Translation.w \n") + "add t_Pos1.xyz, t_Pos1.xyz, t_Translation.xyz \n") + "mul t_Pos1, t_Pos1, a_Weight \n") + "add v_PositionAnim.xyz, v_PositionAnim.xyz, t_Pos1.xyz \n");
        private const string VERTEX_SHADER_MESH_ANIM_NORM_SEG = ((((((("mul t_Pos1, v_NormalSrc, t_Rotation.w \n" + "crs t_Pos2.xyz, t_Rotation, v_NormalSrc \n") + "add t_Pos1, t_Pos1, t_Pos2 \n") + "crs t_Pos1.xyz, t_Rotation, t_Pos1 \n") + "add t_Pos1, t_Pos1, t_Pos1 \n") + "add t_Pos1, t_Pos1, v_NormalSrc \n") + "mul t_Pos1, t_Pos1, a_Weight \n") + "add t_NormalAnim.xyz, t_NormalAnim.xyz, t_Pos1.xyz \n");
        private const string VERTEX_SHADER_MESH_ANIM_TANG_SEG = ((((((("mul t_Pos1, v_TangentSrc, t_Rotation.w \n" + "crs t_Pos2.xyz, t_Rotation, v_TangentSrc \n") + "add t_Pos1, t_Pos1, t_Pos2 \n") + "crs t_Pos1.xyz, t_Rotation, t_Pos1 \n") + "add t_Pos1, t_Pos1, t_Pos1 \n") + "add t_Pos1, t_Pos1, v_TangentSrc \n") + "mul t_Pos1, t_Pos1, a_Weight \n") + "add t_TangentAnim.xyz, t_TangentAnim.xyz, t_Pos1.xyz \n");
        private const string VERTEX_SHADER_VARY_NORM_SEG = "m33 t_Normal.xyz, t_Normal, u_NormalMatrix\t\n";
        private const string VERTEX_SHADER_VARY_TANG_SEG = ((("nrm t_Normal.xyz, t_Normal.xyz\t\t\t\t\t\t\n" + "m33 t_Tangent.xyz, t_Tangent, u_NormalMatrix\t\t\n") + "nrm t_Tangent.xyz, t_Tangent.xyz\t\t\t\t\t\n") + "crs t_BiTangent.xyz, t_Normal.xyz, t_Tangent.xyz\t\n");
        private const string VERTEX_SHADER_FOOTER = ("m44 v_Position, v_Position, u_ModelMatrix\t\t\t\t\n" + "m44 t_PositionT, v_Position, u_ViewProjectionMatrix\t\n");
        private const int MAX_JOINTS = 50, JOINT_CONST_START = 28, MAX_BLENDSHAPE_TARGETS = 8;
        private static List<float> __sharedJointConstants = new List<float>(MAX_JOINTS * (2 * 4));
        private static List<float> __blendShapeConstants = new List<float>(MAX_BLENDSHAPE_TARGETS);
        protected static AlpineMatrix3D __tempMat = new AlpineMatrix3D();
        private AlpSkeleton skeleton;
        private List<float> jointConstants;
        private bool underJointLimit, externalSkeleton;
        private AnimatedModelRenderable skeletonOwner;

        public AnimatedModelRenderable(ModelData param2) : base(param2)
        {
        }
    }
}
