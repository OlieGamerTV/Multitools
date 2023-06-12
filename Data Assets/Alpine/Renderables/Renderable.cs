using Alpine.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables
{
    internal class Renderable
    {
        private static int nextID = 1;
        protected IBoundingVolume boundingVolume;
        public Renderable boundingSphereRenderable;
        protected Renderable boundingVolumeRenderable;
        protected int id;
        public bool transparency, updated;
        public bool castsShadow, receivesShadow, raysCanHit = true;
        public int motionBlueTexture = -1;
        protected int motionBlurIterations = 1;
        public float outlineThickness;
        public AlpineVector3D outlineColor;
        public string culling = "back";
        public float distance;
        private bool visible = true;
        private string blendSrc = "", blendDest = "";
        private int drawPriority = 2147483647;
        protected string compareMode = "lessEqual";
        protected bool writeDepth = true;

        public Renderable() : base()
        {
            outlineColor = new AlpineVector3D(0f, 0f, 0f, 1f);
            id = nextID++;
            transparency = false;
        }
    }
}
