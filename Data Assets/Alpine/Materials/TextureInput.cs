using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Materials
{
    internal class TextureInput
    {
        public string name, filtering, mips, repeat;
        public bool is2D;

        public TextureInput(string param1, bool param2 = true) : base()
        {
            name = param1;
            is2D = param2;
            filtering = "linear";
            mips = null;
            repeat = "clamp";
        }

        public bool IsEqual(TextureInput other)
        {
            return name == other.name && is2D == other.is2D && filtering == other.filtering && mips == other.mips && repeat == other.repeat;
        }

        public TextureInput Clone()
        {
            TextureInput loc1 = new TextureInput(this.name, this.is2D);
            loc1.filtering = this.filtering;
            loc1.mips = this.mips;
            loc1.repeat = this.repeat;
            return loc1;
        }
    }
}
