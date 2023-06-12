using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Alpine.Geom;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class JointData
    {
        public static uint INVALID_JOINTID = 255, INVALID_TRANSFORMID = 65535;
        public int id;
        public string name;
        public bool isHardpoint;
        public uint parentId, transformId, invTransformId;
        public List<uint> childrenIds;
        public AlpineTransform transform, invTransform;

        public JointData() : base() {}
    }
}
