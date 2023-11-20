using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Geom
{
    public class AlpineTransform
    {
        public static AlpineTransform temp = new AlpineTransform();
        private static float SPHERICAL_LIMIT = 0.9928f;
        public AlpineVector3D translation;
        public AlpineQuaternion rotation;
        public float scale;

        public AlpineTransform(float transform_X = 0, float transform_Y = 0, float transform_Z = 0, float quat_X = 0, float quat_Y = 0, float quat_Z = 0, float quat_W = 1, float defScale = 1) : base()
        {
            translation = new AlpineVector3D(transform_X, transform_Y, transform_Z);
            rotation = new AlpineQuaternion(quat_X, quat_Y, quat_Z, quat_W);
            scale = defScale;
        }

        public AlpineTransform Clone()
        {
            return new AlpineTransform(this.translation.x, this.translation.y, this.translation.z, this.rotation.x, this.rotation.y, this.rotation.z, this.rotation.w, this.scale);
        }

        public void Identity()
        {
            this.translation.x = 0;
            this.translation.y = 0;
            this.translation.z = 0;
            this.rotation.x = 0;
            this.rotation.y = 0;
            this.rotation.z = 0;
            this.rotation.w = 1;
            scale = 1.0f;
        }

        public void Zero()
        {
            this.translation.x = 0;
            this.translation.y = 0;
            this.translation.z = 0;
            this.rotation.x = 0;
            this.rotation.y = 0;
            this.rotation.z = 0;
            this.rotation.w = 0;
            scale = 0f;
        }

        public void CopyFrom(AlpineTransform other)
        {
            this.translation = other.translation;
            this.rotation = other.rotation;
            this.scale = other.scale;
        }

        public void Compose(AlpineTransform param1)
        {
            AlpineVector3D.Temp.Times(scale);
            translation.Plus(AlpineVector3D.Temp);
            scale *= param1.scale;
        }

        public override string ToString()
        {
            return "{ T: " + translation.ToString() + " R: " + rotation.ToString() + " S: " + scale + " }";
        }

        public string SimpleToString()
        {
            return string.Join(" ", 
                translation.x, 
                translation.y, 
                translation.z, 
                translation.w, 
                rotation.x, 
                rotation.y, 
                rotation.z, 
                rotation.w, 
                scale);
        }
    }
}
