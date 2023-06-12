using Alpine.Scene.Renderer;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class VertexAttributeInputs
    {
        public List<string> names, formats;
        public List<uint> numComponents;
        public int numInputs;

        public VertexAttributeInputs(ByteArray? param1 = null) : base()
        {
            uint loc2 = 0;
            List<string> loc3 = null;
            List<uint> loc4 = null;
            List<string> loc5 = null;
            int loc6 = 0;
            string loc7 = null;
            uint loc8 = 0;
            if (param1 == null)
            {
                names = new List<string>();
                numComponents = new List<uint>();
                formats = new List<string>();
                numInputs = 0;
            }
            else
            {
                loc2 = param1.ReadUnsignedByte();
                if (loc2 > 8)
                {
                    throw new ArgumentOutOfRangeException("Too many (" + loc2 + ") texture inputs required. Must be no more than 8.");
                }
                loc3 = names = new List<string>((int)loc2);
                loc4 = numComponents = new List<uint>((int)loc2);
                loc5 = formats = new List<string>((int)loc2);
                while (loc6 < loc2)
                {
                    loc7 = loc3[loc6] = param1.ReadUTF();
                    if (VertexAttributeInputName.RESERVED.IndexOf(loc7) < 0 && loc7.Substring(0, 1) == "_")
                    {
                        throw new ArgumentException("Name (" + loc7 + ") cannot begin with an underscore.");
                    }
                    if ((loc8 = loc4[loc6] = param1.ReadUnsignedByte()) > 4)
                    {
                        throw new ArgumentException("Too many components (" + loc8 + ") per vertex. Must be no more than 4.");
                    }
                    switch (loc8)
                    {
                        case 1:
                            loc5[loc6] = IRendererConstants.VERTEXBUFFER_FLOAT1;
                            break;
                        case 2:
                            loc5[loc6] = IRendererConstants.VERTEXBUFFER_FLOAT2;
                            break;
                        case 3:
                            loc5[loc6] = IRendererConstants.VERTEXBUFFER_FLOAT3;
                            break;
                        case 4:
                            loc5[loc6] = IRendererConstants.VERTEXBUFFER_FLOAT4;
                            break;
                    }
                    loc6++;
                }
                numInputs = (int)loc2;
            }
        }

        public void Add(string param1, int param2, string param3)
        {
            names.Add(param1);
            numComponents.Add((uint)param2);
            formats.Add(param3);
            numInputs++;
        }

        public void Remove(string param1)
        {
            int loc2 = names.IndexOf(param1);
            if (loc2 == -1)
            {
                return;
            }
            names.RemoveAt(loc2);
            numComponents.RemoveAt(loc2);
            formats.RemoveAt(loc2);
            numInputs--;
        }

        public bool IsEqual(VertexAttributeInputs param1)
        {
            string? loc3 = null;
            int loc4 = 0;
            if (numInputs != param1.numInputs)
            {
                return false;
            }
            int loc2 = 0;
            if(loc2 <  numInputs)
            {
                loc3 = names[loc2] as string;
                if((loc4 = param1.names.IndexOf(loc3)) == -1)
                {
                    return false;
                }
                if (numComponents[loc2] != param1.numComponents[loc4])
                {
                    return false;
                }
                if (formats[(int)loc2] != param1.formats[loc4])
                {
                    return false;
                }
                loc2++;
            }
            return true;
        }

        public VertexAttributeInputs Clone()
        {
            var loc1 = new VertexAttributeInputs();
            loc1.names = names;
            loc1.numComponents = numComponents;
            loc1.formats = formats;
            loc1.numInputs = numInputs;
            return loc1;
        }
    }
}
