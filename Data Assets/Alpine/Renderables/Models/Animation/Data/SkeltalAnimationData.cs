using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    internal class SkeltalAnimationData : AnimationSegmentData
    {
        public bool precomputed;
        public List<JointAnimationData> joints;
        public List<AlpineTransform> transforms;
        public SkeltalAnimationData() : base()
        {

        }
    }
}
