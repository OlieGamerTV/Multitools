using Multi_Tool.Tools.BKV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class LodData
    {
        public string name;
        public List<LodLevelData> levels;
        public LodData(dynamic param1) : base()
        {
            if (param1 is BKVTable)
            {
                FromBKV(param1 as  BKVTable);
            }
        }

        private void FromBKV(BKVTable param1)
        {
            name = param1.GetValue("name").AsString();
            BKVTable loc2 = param1.GetValue("levels").AsTable();
            int loc3 = (int)loc2.GetNumValues();
            levels = new List<LodLevelData>(loc3);
            int loc4 = 0;
            while (loc4 < loc3)
            {
                levels[loc4] = new LodLevelData(loc2.GetValue(loc4).AsTable());
                loc4++;
            }
        }
    }
}
