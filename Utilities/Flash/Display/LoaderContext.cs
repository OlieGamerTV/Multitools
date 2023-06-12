using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Utilities.Flash.Display
{
    internal class LoaderContext
    {
        bool allowCodeImport, allowLoadBytesCodeExecution, checkPolicyFile;
        string imageDecodingPolicy;

        public LoaderContext(bool checkPolicy = false) : base()
        {
            checkPolicyFile = checkPolicy;
        }
    }
}
