using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    internal class BlendShapeTargetAnimationData : AnimationSegmentData
    {
        public string _base, target;
        public List<float> frames;
        public BlendShapeTargetAnimationData() : base()
        {

        }
    }
}
