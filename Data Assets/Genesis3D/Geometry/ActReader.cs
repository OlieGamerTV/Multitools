using Genesis3D.Geometry.Body;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis3D.Geometry
{
    public class ActReader
    {
        public static void ReadFile(string filePath)
        {
            if (filePath == null) return;
            BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
            SkinGeometry geometry = new SkinGeometry();
            reader.BaseStream.Seek(0x60, SeekOrigin.Begin);
            geometry.ReadBodyGeo(reader);
            SkinMaterial material = new SkinMaterial();
            material.ReadBodyMaterials(reader, 0x40E);
            geometry.ReadBodyIndices(reader, 0x426);
        }
    }
}
