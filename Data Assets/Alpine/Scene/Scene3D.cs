using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Scene
{
    internal class Scene3D
    {
        #region Static Parameters
        private static List<float> __shadowFragmentConstants = new List<float> { 1, 255, 65025, 16581375 };
        private static Vector4 __tempVector = new Vector4();
        private static List<float> __tempMatrixComponents = new List<float>(16), __constants = new List<float>(4);
        public const int RENDER_MODE_NORMAL = 0, RENDER_MODE_SHADOW = 1, RENDER_MODE_TEST = 2;
        public const int RENDER_MODE_DEPTH_BUFFER = 3, RENDER_MODE_NORMALS = 4, RENDER_MODE_LINEAR_DEPTH = 5;
        public const int RENDER_MODE_MOTION_BLUR_OFFSET = 6, RENDER_MODE_MOTION_BLUR_FINAL = 7, RENDER_MODE_IMAGE_SPACE_SHADOW = 8;
        public const int RENDER_MODE_OUTLINE = 9;
        const int MAX_CONSTANTS_VERTEX = 128, MAX_CONSTANTS_FRAGMENT = 28;
        private static int POSTFILTER_TEXTURE = -1;
        private static string BASELINE = "baseline", BASELINE_CONSTRAINED = "baselineConstrained";
        #endregion
        #region Protected Parameters
        protected Rectangle viewPort, scissorsRect;
        protected AlpineMatrix3D viewportMatrix;
        #endregion
    }
}
