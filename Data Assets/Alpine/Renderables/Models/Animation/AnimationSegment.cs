using Alpine.Renderables.Models.Animation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation
{
    internal class AnimationSegment
    {
        protected AnimationSegmentData segmentData;
        private List<AnimationSegmentDataFrame> frameData;
        protected bool playing, isBlenderTarget;
        protected float time, blend, weight;
        protected int direction, currentFrame, nextFrame;

        public AnimationSegment()
        {
            segmentData = null;
            frameData = null;
            playing = false;
            time = 0;
            direction = 1;
            currentFrame = 0;
            nextFrame = 0;
            blend = 0;
            weight = 1;
            isBlenderTarget = false;
        }
    }
}
