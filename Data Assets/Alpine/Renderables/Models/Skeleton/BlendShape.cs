using Alpine.Renderables.Models.Skeleton.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class BlendShape
    {
        public string name;
        public float weight;
        public List<BlendShapeTarget> targets;
        public int numTargets;
        public bool mergedTargets;

        public BlendShape(ModelRenderable param1, BlendShapeData param2)
        {
            name = param2.name;
            weight = 1;
            mergedTargets = param2.mergedTargets;
            targets = new List<BlendShapeTarget>();
            foreach (BlendShapeTargetData loc3 in param2.targets)
            {
            }
        }

        public int NumActiveTargets()
        {
            int loc1 = 0, loc2 = 0, loc3 = numTargets;
            loc2 = 0;
            while (loc2 < loc3)
            {
                if (targets[loc2].activeRefs > 0)
                {
                    loc1++;
                }
                loc2++;
            }
            return loc1;
        }
    }
}
