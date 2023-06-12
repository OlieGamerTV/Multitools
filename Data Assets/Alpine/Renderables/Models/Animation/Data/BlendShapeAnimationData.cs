using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    internal class BlendShapeAnimationData : AnimationSegmentData
    {
        public string name;
        public List<float> frames;
        public List<string> targetNames;
        public List<List<float>> targetFrames;
        public BlendShapeAnimationData() : base()
        {
        }
    }
}
