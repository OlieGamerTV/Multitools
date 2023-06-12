using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    internal class JointAnimationData
    {
        public string joint;
        public List<AlpineTransform> frames;
        public List<uint> frameIds;
        public JointAnimationData() : base()
        {

        }
    }
}
