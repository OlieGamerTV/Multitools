using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class CurveData
    {
        public string name;
        public List<AlpineVector3D> points;

        public CurveData(XmlDocument xml) : base()
        {
            name = xml.DocumentElement.SelectSingleNode("@name").Value;
        }
    }
}
