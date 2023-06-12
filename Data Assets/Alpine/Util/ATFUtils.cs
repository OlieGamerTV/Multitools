using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Util
{
    internal class ATFUtils
    {
        public const int FORMAT_UNCOMPRESSED = 0;
        public const int FORMAT_UNCOMPRESSED_ALPHA = 1;
        public const int FORMAT_COMPRESSED = 2;
        public const int FORMAT_COMPRESSED_ALPHA = 4;

        public static bool IsATF(ByteArray arg1)
        {
            if (arg1 == null)
            {
                return false;
            }
            arg1.Position = 0;
            return (arg1.ReadByte() == 65) && (arg1.ReadByte() == 84) && (arg1.ReadByte() == 70);
        }

        public static bool IsCubeMap(ByteArray arg1)
        {
            arg1.Position = 6;
            return (arg1.ReadByte() & 0x80) > 0;
        }

        public static int GetFormat(ByteArray arg1)
        {
            if(!IsATF(arg1))
            {
                return -1;
            }
            return arg1.ReadByte() & 0x7F;
        }

        public static int GetWidth(ByteArray arg1)
        {
            arg1.Position = 7;
            return 2 << (arg1.ReadByte() - 1);
        }

        public static int GetHeight(ByteArray arg1)
        {
            arg1.Position = 8;
            return 2 << (arg1.ReadByte() - 1);
        }
    }
}
