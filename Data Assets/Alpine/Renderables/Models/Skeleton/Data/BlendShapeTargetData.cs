using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Multi_Tool.Tools.BKV;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class BlendShapeTargetData
    {
        public string name;
        public float weight;

        public BlendShapeTargetData(dynamic param1, int param2 = -1) : base()
        {
            if (param1 is BKVTable)
            {
                FromBKV(param1 as BKVTable, param2);
            }
        } 

        private void FromBKV(BKVTable param1, int param2)
        {
            name = param1.GetKey((uint)param2);
            weight = param1.GetValue(param2).AsFloat();
        }
    }
}
