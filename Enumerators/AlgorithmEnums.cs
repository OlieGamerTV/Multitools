using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_Tool.Enumerators
{
    class AlgorithmEnums
    {
        public enum AlgorithmTypes
        {
            zlib,
            deflate,
            gzip,
            lzma,
            serf
        }

        public const string AUTOMATIC = "AUTOMATIC";
        public const string ZLIB = "ZLIB";
        public const string DEFLATE = "DEFLATE";
        public const string GZIP = "GZIP";
        public const string LZMA = "LZMA";
        public const string SERF = "SERF";


        public AlgorithmTypes algorithmTypes;
    }
}
