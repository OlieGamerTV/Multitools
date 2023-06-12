using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Alpine.Renderables.Models.Skeleton.Data
{
    public class PointData : AlpineVector3D
    {
        public string name;

        public PointData(XmlDocument xml)
        {
            try
            {
                x = Convert.ToSingle(xml.DocumentElement.SelectSingleNode("@x").Value);
                y = Convert.ToSingle(xml.DocumentElement.SelectSingleNode("@y").Value);
                z = Convert.ToSingle(xml.DocumentElement.SelectSingleNode("@z").Value);
                name = xml.DocumentElement.SelectSingleNode("@name").Value;
            }
            catch(Exception ex)
            {
                throw new ArgumentOutOfRangeException("File reached end of stream with no found parameter. " + ex);
            }
        }
    }
}
