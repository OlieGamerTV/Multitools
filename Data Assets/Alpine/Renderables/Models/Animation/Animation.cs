using Alpine.Renderables.Models.Animation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation
{
    internal class Animation
    {
        public const int INTERPOLATE = 3, ADVANCE = 2, COMPLETE = 1, NOT_PLAYING = 0, STOP = 0, LOOP = 1, HOLD = 2;
        protected AnimatedModelRenderable model;
        protected AnimationData data;
        private bool playing, paused, hasBlendShapeAnimations, isBlendTarget;
        private int onEnd, loopCount, tDirection, numAnimations;
        private float time, weight, frozenWeight;
        private List<AnimationSegment> animations;

        public Animation(AnimationData data, AnimatedModelRenderable arg2)
        {
            BlendShapeAnimationData loc4;
            List<BlendShapeTargetAnimationData> loc5;
            BlendShapeTargetAnimationData loc6;
            List<UVAnimationData> loc7;
            UVAnimationData loc8;
            List<VisibilityAnimationData> loc9;
            VisibilityAnimationData loc10;
            List<AlphaAnimationData> loc11;
            AlphaAnimationData loc12;
        }
    }
}
