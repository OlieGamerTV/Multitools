using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Geom
{
    public class AlpineQuaternion
    {
        public static AlpineQuaternion Temp = new AlpineQuaternion();
        public float x, y, z, w;

        public AlpineQuaternion(float x_axis = 0, float y_axis = 0, float z_axis = 0, float w_axis = 1) : base()
        {
            x = x_axis;
            y = y_axis;
            z = z_axis;
            w = w_axis;
        }

        public AlpineQuaternion Clone()
        {
            return new AlpineQuaternion(x, y, z, w);
        }

        public void Identity()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 1;
        }

        public bool IsIdentity()
        {
            return x == 0 && y == 0 && z == 0 && w == 1;
        }

        public void CopyFrom(AlpineQuaternion other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
            w = other.w;
        }

        public void SetValues(float x_axis = 0, float y_axis = 0, float z_axis = 0, float w_axis = 1)
        {
            x = x_axis;
            y = y_axis;
            z = z_axis;
            w = w_axis;
        }

        public void SetAxisAngle(AlpineVector3D axis, float angle)
        {
            angle /= 2;
            w = MathF.Cos(angle);
            float loc3 = MathF.Sin(angle);
            x = axis.x * loc3;
            y = axis.y * loc3;
            z = axis.z * loc3;
        }

        public void SetEulerAngles(float param1, float param2, float param3)
        {
            float loc4 = MathF.Cos(param1 / 2);
            float loc5 = MathF.Sin(param1 / 2);
            float loc6 = MathF.Cos(param2 / 2);
            float loc7 = MathF.Sin(param2 / 2);
            float loc8 = MathF.Cos(param3 / 2);
            float loc9 = MathF.Sin(param3 / 2);
            float loc10 = loc4 * loc6;
            float loc11 = loc5 * loc7;
            w = loc10 * loc8 - loc11 * loc9;
            param1 = loc10 * loc9 + loc11 * loc8;
            param2 = loc5 * loc6 * loc8 + loc4 * loc7 * loc9;
            param3 = loc4 * loc7 * loc8 - loc5 * loc6 * loc9;
        }

        public AlpineMatrix3D ToMatrix(AlpineMatrix3D param1 = null)
        {
            float loc7 = float.NaN;
            float loc10 = float.NaN;
            if (param1 == null)
            {
                param1 = new AlpineMatrix3D();
            }
            float loc2 = x * x + y * y + z * z + w * w;
            float loc3 = loc2 > 0 ? (float)(2 / loc2) : 0f;
            float loc4 = x * loc3;
            float loc5 = y * loc3;
            float loc6 = z * loc3;
            loc7 = w * loc4;
            float loc8 = w * loc5;
            float loc9 = w * loc6;
            loc10 = x * loc4;
            float loc11 = x * loc5;
            float loc12 = x * loc6;
            float loc13 = y * loc5;
            float loc14 = y * loc6;
            float loc15 = z * loc6;
            param1.Identity();
            param1._00 = 1 - (loc13 + loc15);
            param1._01 = loc11 - loc9;
            param1._02 = loc12 - loc8;
            param1._10 = loc11 - loc9;
            param1._11 = 1 - (loc10 + loc15);
            param1._12 = loc14 - loc7;
            param1._20 = loc12 - loc8;
            param1._21 = loc14 - loc7;
            param1._22 = 1 - (loc10 + loc13);
            return param1;
        }

        public void Negate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public void Normalize()
        {
            float loc1 = x * x + y * y + z * z + w * w;
            if (loc1 != 1)
            {
                loc1 = 1 / MathF.Sqrt(loc1);
                x *= loc1;
                y *= loc1;
                z *= loc1;
                w *= loc1;
            }
        }

        public float Dot(AlpineQuaternion other)
        {
            return x * other.x + y * other.y + z * other.z + w * other.w;
        }

        public void Compose(AlpineQuaternion other)
        {
            float loc2 = x, loc3 = y, loc4 = z, loc5 = w;
            float loc6 = other.x, loc7 = other.y, loc8 = other.z, loc9 = other.w;
            w = loc5 * loc9 - loc2 * loc6 - loc3 * loc7 - loc4 * loc8;
            x = loc5 * loc6 + loc2 * loc9 + loc3 * loc8 - loc4 * loc7;
            y = loc5 * loc7 + loc3 * loc9 + loc4 * loc6 - loc2 * loc8;
            z = loc5 * loc8 + loc4 * loc9 + loc2 * loc7 - loc3 * loc6;
        }

        public void ComposeInto(AlpineQuaternion other, AlpineQuaternion target)
        {
            float loc3 = x, loc4 = y, loc5 = z, loc6 = w;
            float loc7 = other.x, loc8 = other.y, loc9 = other.z, loc10 = other.w;
            target.w = loc6 * loc10 - loc3 * loc7 - loc4 * loc8 - loc5 * loc9;
            target.x = loc6 * loc7 + loc3 * loc10 + loc4 * loc9 - loc5 * loc8;
            target.y = loc6 * loc8 + loc4 * loc10 + loc5 * loc7 - loc3 * loc9;
            target.z = loc6 * loc9 + loc5 * loc10 + loc3 * loc8 - loc4 * loc7;
        }

        public void Times(AlpineVector3D other)
        {
            float loc2 = x, loc3 = y, loc4 = z, loc5 = w;
            float loc6 = other.x, loc7 = other.y, loc8 = other.z;
            other.x = loc5 * loc5 * loc6 + 2 * loc3 * loc5 * loc8 - 2 * loc4 * loc5 * loc7 + loc2 * loc2 * loc6 + 2 * loc3 * loc2 * loc7 + 2 * loc4 * loc2 * loc8 - loc4 * loc4 * loc6 - loc3 * loc3 * loc6;
            other.y = 2 * loc2 * loc3 * loc6 + loc3 * loc3 * loc7 + 2 * loc4 * loc3 * loc8 + 2 * loc5 * loc4 * loc6 - loc4 * loc4 * loc7 + loc5 * loc5 * loc7 - 2 * loc2 * loc5 * loc8 - loc2 * loc2 * loc7;
            other.z = 2 * loc2 * loc4 * loc6 + 2 * loc3 * loc4 * loc7 + loc4 * loc4 * loc8 - 2 * loc5 * loc3 * loc6 - loc3 * loc3 * loc8 + 2 * loc5 * loc2 * loc7 - loc2 * loc2 * loc8 + loc5 * loc5 * loc8;
        }

        public void TimesInto(AlpineVector3D other, AlpineVector3D target)
        {
            float loc3 = x, loc4 = y, loc5 = z, loc6 = w;
            float loc7 = other.x, loc8 = other.y, loc9 = other.z;
            target.x = loc6 * loc6 * loc7 + 2 * loc4 * loc6 * loc9 - 2 * loc5 * loc6 * loc8 + loc3 * loc3 * loc7 + 2 * loc4 * loc3 * loc8 + 2 * loc5 * loc3 * loc9 - loc5 * loc5 * loc7 - loc4 * loc4 * loc7;
            target.y = 2 * loc3 * loc4 * loc7 + loc4 * loc4 * loc8 + 2 * loc5 * loc4 * loc9 + 2 * loc6 * loc5 * loc7 - loc5 * loc5 * loc8 + loc6 * loc6 * loc8 - 2 * loc3 * loc6 * loc9 - loc3 * loc3 * loc8;
            target.z = 2 * loc3 * loc5 * loc7 + 2 * loc4 * loc5 * loc8 + loc5 * loc5 * loc9 - 2 * loc6 * loc4 * loc7 - loc4 * loc4 * loc9 + 2 * loc6 * loc3 * loc8 - loc3 * loc3 * loc9 + loc6 * loc6 * loc9;
        }

        public override string ToString()
        {
            return "< X: " + x + ", Y: " + y + ", Z: " + z + " : W: " + w + " >";
        }
    }
}
