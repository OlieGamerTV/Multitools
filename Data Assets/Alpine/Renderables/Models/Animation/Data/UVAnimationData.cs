using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class UVAnimationData : AnimationSegmentData
    {
        public string texture;
        public List<float> framesU, framesV;
        public bool isOffsets;
        public UVAnimationData() : base()
        {
        }
    }
}
