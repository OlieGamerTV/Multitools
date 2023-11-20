using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class AlphaAnimationData : AnimationSegmentData
    {
        public List<string> targets;
        public List<float> frames;
        public AlphaAnimationData() : base()
        {
        }
    }
}
