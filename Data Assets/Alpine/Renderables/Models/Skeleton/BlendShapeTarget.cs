using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class BlendShapeTarget
    {
        public Mesh target;
        public float weight;
        public int activeRefs;

        public BlendShapeTarget(Mesh param1, float param2) : base()
        {
            target = param1;
            weight = param2;
        }

        public void Ref()
        {
            activeRefs++;
        }

        public void Deref()
        {
            activeRefs--;
        }
    }
}
