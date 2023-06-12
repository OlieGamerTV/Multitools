using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Multi_Tool.Tools
{
    internal class DPackItem
    {
        private string _name;
        private ByteArray _bytes;

        public DPackItem(string param1, ByteArray param2) : base()
        {
            if (param1 == null || param1.Length == 0)
            {
                Debug.WriteLine("DPackItem Constructor: Invalid name.");
                _name = "null";
            }
            if (param2 == null)
            {
                Debug.WriteLine("DPackItem constructor: Invalid bytes.");
            }
            _name = param1;
            _bytes = param2;
        }

        public string GetName()
        {
            return _name;
        }

        public ByteArray GetBytes()
        {
            return _bytes;
        }
    }
}
