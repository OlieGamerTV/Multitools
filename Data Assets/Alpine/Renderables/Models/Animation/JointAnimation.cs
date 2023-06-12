using Alpine.Geom;
using Alpine.Renderables.Models.Animation.Data;
using Alpine.Renderables.Models.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation
{
    internal class JointAnimation
    {
        private static AlpineTransform blendingTemp = new AlpineTransform();
        private static AlpineTransform temp1 = new AlpineTransform();
        private static AlpineTransform temp2 = new AlpineTransform();
        private Joint target;
        private JointAnimationData data;
        private JointAnimation parent;
        private int numFrames;
        private AlpineTransform frame1, frame2, blendedFrame;
        private int frame1Id, frame2Id;
        private bool blendFrames;
        private AlpineTransform inverse1, inverse2;
        private int inverseFrame1, inverseFrame2;

        public JointAnimation(JointAnimationData arg1, Joint arg2, JointAnimation arg3)
        {
            data = arg1;
            parent = arg3;
            target = arg2;
            target.Ref(true, false);
            blendedFrame = target.animTransform;
            SetFrames(0, 0);
        }

        public void SetFrames(int arg1, int arg2)
        {
            frame1Id = arg1;
            frame1 = data.frames[frame1Id];
            frame2Id = arg2;
            frame2 = data.frames[frame2Id];
            blendFrames = !(data.frameIds[frame1Id] == data.frameIds[frame2Id]);
        }
    }
}
