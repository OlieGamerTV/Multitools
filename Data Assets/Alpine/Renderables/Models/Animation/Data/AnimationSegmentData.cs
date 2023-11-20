using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Data
{
    public class AnimationSegmentData
    {
        public List<AnimationSegmentDataFrame> frameData;
        public AnimationSegmentData() : base()
        {
        }

        public void ReadFrames(BKVTable param1)
        {
            BKVTable loc2 = null;
            AnimationSegmentDataFrame loc3 = null;
            int loc4 = (int)param1.GetNumValues();
            frameData = new List<AnimationSegmentDataFrame>(loc4);
            int loc5 = 0;
            while (loc5 < loc4)
            {
                loc2 = param1.GetValue(loc5).AsTable();
                loc3 = new AnimationSegmentDataFrame();
                loc3.frame = (uint)loc2.GetValue("frame").AsInt();
                loc3.time = loc2.GetValue("time").AsFloat();
                loc3.interpolate = !!loc2.HasValue("interpolate") ? loc2.GetValue("interpolate").AsBool() : true;
                frameData.Add(loc3);
                loc5++;
            }
        }

        public void BuildFrames(uint param1, float param2)
        {
            AnimationSegmentDataFrame loc3 = null;
            frameData = new List<AnimationSegmentDataFrame>((int)param1);
            float loc4 = param2 / (param1 - 1);
            int loc5 = 0;
            while (loc5 < param1)
            {
                loc3 = new AnimationSegmentDataFrame();
                loc3.frame = (uint)loc5;
                loc3.time = loc5 * loc4;
                loc3.interpolate = true;
                frameData.Add(loc3);
                loc5++;
            }
        }
    }
}
