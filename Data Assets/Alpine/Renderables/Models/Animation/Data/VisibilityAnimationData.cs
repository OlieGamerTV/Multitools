using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class VisibilityAnimationData : AnimationSegmentData
    {
        public List<string> targets;
        public List<bool> frames;
        public VisibilityAnimationData() : base()
        {
        }
    }
}
