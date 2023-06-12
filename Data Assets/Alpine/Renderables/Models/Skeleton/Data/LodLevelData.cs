using Multi_Tool.Tools.BKV;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class LodLevelData
    {
        public int level;
        public float threshold, thresholdSqr;
        public List<string> targets;

        public LodLevelData(dynamic param1) : base()
        {
            if (param1 is BKVTable && param1 != null)
            {
                FromBKV(param1 as  BKVTable);
            }
        }

        private void FromBKV(BKVTable param1)
        {
            level = param1.GetValue("id").AsInt();
            threshold = param1.GetValue("threshold").AsFloat();
            thresholdSqr = threshold * threshold;
            BKVTable loc2 = param1.GetValue("targets").AsTable();
            int loc3 = (int)loc2.GetNumValues();
            targets = new List<string>(loc3);
            int loc4 = 0;
            while (loc4 < loc3)
            {
                targets[loc4] = loc2.GetValue(loc4).AsString();
                loc4++;
            }
        }
    }
}
