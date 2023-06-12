using Alpine.Geom;
using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class AlpSkeleton
    {
        private static Matrix4x4 jointTransform = new Matrix4x4();
        private static AlpSkeleton Empty = new AlpSkeleton(null);
        private List<Joint>? joints;
        private int animatingRefs;
        private bool modified;
        public int numJoints;

        public AlpSkeleton(ModelData? param1) : base()
        {
            List<Joint>? loc2 = null;
            Joint? loc3 = null;
            List<JointData>? loc4 = null;
            int loc5 = 0, loc6 = 0;
            animatingRefs = 0;
            modified = false;
            if (param1 == null)
            {
                joints = new List<Joint>();
            }
            else
            {
                loc5 = (loc4 = param1.skeleton).Count;
                this.joints = loc2 = new List<Joint>(loc5);
                if (loc5 > 0)
                {
                    loc6 = 0;
                    while (loc6 < loc5)
                    {
                        loc2[loc6] = new Joint();
                        loc6++;
                    }
                    loc6 = 0;
                    while(loc6 < loc5)
                    {
                        loc3 = loc2[loc6];
                        loc3.Build(loc4[loc6], loc2);
                        if (!loc3.isHardpoint)
                        {
                            numJoints++;
                        }
                        loc6++;
                    }
                    
                }
            }
        }

        public void Compute(bool param1 = false)
        {
            Joint? loc4 = null, loc5 = null;
            int loc6 = 0;
            AlpineTransform? loc9 = null, loc10 = null, loc11 = null;
            float loc12 = float.NaN, loc13 = float.NaN, loc14 = float.NaN, loc15 = float.NaN, loc16 = float.NaN, loc17 = float.NaN, loc18 = float.NaN, loc19 = float.NaN, loc20 = float.NaN, loc21 = float.NaN, loc22 = float.NaN, loc23 = float.NaN;
            bool loc2 = this.animatingRefs > 0 || param1 || this.modified;
            if (!loc2)
            {
                return;
            }
            dynamic loc3 = this.animatingRefs > 0;
            List<Joint> loc7 = joints;
            int loc8 = joints.Count;
            loc6 = 0;
            while (loc6 < loc8)
            {
                loc5 = (loc4 = loc7[loc6]).parent;
                if (loc2 || loc4.modified || loc5.modified)
                {
                    loc4.modified = true;
                    if (loc4.manualRefs > 0)
                    {
                        if (loc4.animationRefs > 0)
                        {
                            loc4.transform.CopyFrom(loc4.manualTransform);
                        }
                    }
                }
            }
        }
    }
}
