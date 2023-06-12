using Alpine.Geom;
using Alpine.Renderables.Models.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Manipulator
{
    internal class JointManipulator : Manipulator
    {
        protected Joint __target;
        public JointManipulator(Joint target) : base()
        {
            __target = target;
            __target.Ref(false, true);
        }

        public new void Destroy()
        {
            if(__target != null)
            {
            }
            __target = null;
        }

        public Joint Target
        {
            get { return __target; }
        }

        public AlpineTransform Transform
        {
            get { return __target.manualTransform; }
            set
            {
                __target.manualTransform.CopyFrom(value);
                __target.modified = true;
            }
        }

        public new void Update(int param1)
        {
        }
    }
}
