using Genesis3D.Geometry.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Genesis3D.Geometry.Pose
{
    public class PoseJoint
    {
        public int parentJoint;
        public Matrix4x4 transform;
        public Quaternion rotation;

        public Vector3 unscaledAttachmentTransform;
        public Quaternion attachmentRotation;
        public Matrix4x4 attachmentTransform;

        public Vector3 localTransform;
        public Quaternion localRotation;

        public bool touched, noAttachmentRotation;
        public int covered;
    }
}
