using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class TextureInputs
    {
        public List<string> names;
        public List<TextureInput> inputs;
        public int numInputs;

        public TextureInputs(ByteArray param1 = null) : base()
        {
            uint loc2 = 0;
            List<TextureInput> loc3 = null;
            List<string> loc4 = null;
            int loc5 = 0;
            if (param1 == null)
            {
                inputs = new List<TextureInput>();
                names = new List<string>();
                numInputs = 0;
            }
            else
            {
                loc2 = param1.ReadUnsignedByte();
                if (loc2 > 8)
                {
                    throw new ArgumentOutOfRangeException("Too many (" + loc2 + ") texture inputs required. Must be no more than 8."); 
                }
                loc3 = inputs = new List<TextureInput>((int)loc2);
                loc4 = names = new List<string>((int)loc2);
                while (loc5 < loc2)
                {
                    loc3[loc5] = new TextureInput(param1.ReadUTF());
                    loc4[loc5] = loc3[loc5].name;
                    loc5++;
                }
                numInputs = (int)loc2;
            }
        }

        public void Add(string param1)
        {
            inputs.Add(new TextureInput(param1));
            names.Add(param1);
            numInputs++;
        }

        public void Remove(string param1)
        {
            int loc2 = 0;
            while(loc2 < inputs.Count)
            {
                if (inputs[loc2].name == param1)
                {
                    inputs.Remove(inputs[loc2]);
                    names.Remove(param1);
                    numInputs--;
                    return;
                }
                loc2++;
            }
        }

        public TextureInput? Find(string param1)
        {
            int loc2 = names.IndexOf(param1);
            if (loc2 == -1)
            {
                return null;
            }
            return inputs[loc2];
        }

        public bool IsEqual(TextureInputs param1)
        {
            bool loc3 = false;
            if (numInputs != param1.numInputs)
            {
                return false;
            }
            int loc5 = 0;
            dynamic loc6 = this.inputs;
            do
            {
                foreach (TextureInput loc2 in loc6)
                {
                    loc3 = false;
                    foreach (TextureInput loc4 in param1.inputs)
                    {
                        if (loc2.Equals(loc4))
                        {
                            loc3 = true;
                            break;
                        }
                    }
                }
                return true;
            }
            while (loc3);

            return false;
        }

        public TextureInputs Clone()
        {
            TextureInputs loc1 = new TextureInputs();
            foreach (TextureInput loc2 in this.inputs)
            {
                loc1.inputs.Add(loc2.Clone());
                loc1.names.Add(loc2.name);
            }
            loc1.numInputs = numInputs;
            return loc1;
        }
    }
}
