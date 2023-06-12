using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multi_Tool.Tools.BKV;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class BlendShapeData
    {
        public string name, _base;
        public List<BlendShapeTargetData> targets;
        public bool mergedTargets;

        public BlendShapeData(dynamic param1) : base()
        {
            if (param1 is BKVTable)
            {
                FromBKV(param1 as BKVTable);
            }
        }

        private void FromBKV(BKVTable param1)
        {
            name = param1.GetValue("name").AsString();
            _base = param1.GetValue("base").AsString();
            mergedTargets = param1.GetValue("merged").AsBool();
            BKVTable loc2 = param1.GetValue("targets").AsTable();
            int loc3 = (int)loc2.GetNumValues();
            targets = new List<BlendShapeTargetData>(loc3);
            int loc4 = 0;
            while (loc4 < loc3)
            {
                targets[loc4] = new BlendShapeTargetData(loc2, loc4);
                loc4++;
            }
        }
    }
}
