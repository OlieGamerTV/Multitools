using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Flash;

namespace Alpine.Geom
{
    internal class Sphere : IBoundingVolume
    {
        public AlpineVector3D center;
        public float radius;
        public bool changed = true;

        public Sphere(AlpineVector3D param1 = null, float param2 = float.NaN) : base()
        {
            if(param1 == null)
            {
                center = new AlpineVector3D();
            }
            else
            {
                center = param1;
            }
            if (param2 == float.NaN)
            {
                radius = 0f;
            }
            else
            {
                radius = param2;
            }
            center.w = 1;
        }

        public IBoundingVolume Clone()
        {
            return new Sphere(center.Clone(), radius);
        }

        public void Transform(AlpineMatrix3D param1, float param2)
        {
            float loc3 = new float[]{ MathF.Sqrt(param1._00 * param1._00 + param1._10 * param1._10 + param1._20 * param1._20), MathF.Sqrt(param1._01 * param1._01 + param1._11 * param1._11 + param1._21 * param1._21), MathF.Sqrt(param1._02 * param1._02 + param1._12 * param1._12 + param1._22 * param1._22) }.Max();
            radius += loc3;
            center.x += param1._03;
            center.y += param1._13;
            center.z += param1._23;
        }

        public void Bound(IBoundingVolume param1)
        {
        }

        public void BoundVertices(ByteArray param1, float param2, bool param3 = false)
        {
        }
    }
}
