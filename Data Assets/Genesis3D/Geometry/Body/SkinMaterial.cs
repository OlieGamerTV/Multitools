using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Genesis3D.Geometry.Body
{
    public class SkinMaterial
    {
        public int dataAmount, dataOffset;
        public List<int> stringOffsets;
        public List<string> materialNames;

        public void ReadBodyMaterials(BinaryReader br, int currentPos = -1)
        {
            if(currentPos != -1)
            {
                br.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }
            dataAmount = br.ReadInt32();
            dataOffset = br.ReadInt32();

            stringOffsets = new List<int>(dataAmount);
            for(int i = 0; i < dataAmount; i++)
            {
                stringOffsets.Add(br.ReadInt32());
            }

            materialNames = new List<string>(dataAmount);
            for (int i = 0; i < dataAmount; i++)
            {
                string name = "";
                bool isCharNull = false;
                while(!isCharNull)
                {
                    char tempChar = br.ReadChar();
                    isCharNull = tempChar == '\0';

                    if (!isCharNull)
                    {
                        name += tempChar;
                    }
                }
                materialNames.Add(name);
            }
        }
    }
}
