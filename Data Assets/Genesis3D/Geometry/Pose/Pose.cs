using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Genesis3D.Geometry.Pose
{
    public class Pose
    {
        int jointCount;
        int nameChecksum;
        bool touched;
        List<string> jointNames;
        Vector3 scale;

        bool slave;
        int slaveJointIndex;
        Pose parent;
        PoseJoint rootJoint;
        Matrix4x4 parentsLastTransform, rootTransform;
        List<Matrix4x4> transformArrays;
        List<PoseJoint> jointArray;
        int onlyThisJoint;
    }
}
