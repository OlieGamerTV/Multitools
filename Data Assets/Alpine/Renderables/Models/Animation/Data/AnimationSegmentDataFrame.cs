using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class AnimationSegmentDataFrame
    {
        public uint frame;
        public float time;
        public bool interpolate;
        public AnimationSegmentDataFrame() : base()
        {

        }
    }
}
