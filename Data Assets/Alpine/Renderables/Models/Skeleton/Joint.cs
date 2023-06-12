using Alpine.Geom;
using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class Joint
    {
        private static Joint? dummyJoint;
        public bool dummy, isHardpoint, modified, hasLocalTransform;
        public JointData? data;
        public string name;
        public Joint? parent;
        public List<Joint>? children;
        public AlpineTransform? baseTransform, invBaseTransform, animTransform, manualTransform, transform, finalTransform, skinningTransform;
        public int animationsModified, animationRefs, manualRefs;

        public Joint() : base()
        {
            this.finalTransform = new AlpineTransform();
            this.skinningTransform = new AlpineTransform();
            modified = false;
            animationRefs = 0;
            manualRefs = 0;
            hasLocalTransform = true;
        }

        public void Build(JointData param1, List<Joint> param2)
        {
            int loc3 = 0, loc4 = 0;
            data = param1;
            name = param1.name;
            isHardpoint = param1.isHardpoint;
            if (param1.parentId == JointData.INVALID_JOINTID)
            {
                if (dummyJoint == null)
                {
                    dummyJoint = new Joint();
                    dummyJoint.dummy = true;
                }
                parent = dummyJoint;
            }
            else
            {
                parent = param2[(int)param1.parentId];
            }
            if (param1.childrenIds != null)
            {
                this.children = new List<Joint>(param1.childrenIds.Count);
                loc4 = children.Count;
                loc3 = 0;
                while (loc3 < loc4)
                {
                    children[loc3] = param2[(int)param1.childrenIds[loc3]];
                    loc3++;
                }
            }
            baseTransform = param1.transform;
            invBaseTransform = param1.invTransform;
            transform = baseTransform;
            modified = true;
        }

        public void Destroy()
        {
            this.parent = null;
            this.children = null;
            this.baseTransform = null;
            this.invBaseTransform = null;
            this.animTransform = null;
            this.manualTransform = null;
            this.transform = null;
            this.finalTransform = null;
        }

        public void Ref(bool animRef, bool manRef)
        {
            if (animRef)
            {
                animationRefs++;
                if (animationRefs == 1)
                {
                    animTransform = new AlpineTransform();
                }
            }
            if (manRef)
            {
                manualRefs++;
                if (manualRefs == 1)
                {
                    manualTransform = new AlpineTransform();
                }
            }
            if (animationRefs > 0 && manualRefs == 0)
            {
                this.transform = animTransform;
            }
            else
            {
                transform = new AlpineTransform();
            }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
