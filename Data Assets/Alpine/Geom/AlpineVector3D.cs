using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Imaging;

namespace Alpine.Geom
{
    public class AlpineVector3D
    {
        public static AlpineVector3D Temp = new AlpineVector3D();
        public static AlpineVector3D X_AXIS = new AlpineVector3D(1f, 0, 0, 0), Y_AXIS = new AlpineVector3D(0, 1f, 0, 0), Z_AXIS = new AlpineVector3D(0, 0, 1f, 0);
        public static AlpineVector3D NEG_X_AXIS = new AlpineVector3D(-1f, 0, 0, 0), NEG_Y_AXIS = new AlpineVector3D(0, -1f, 0, 0), NEG_Z_AXIS = new AlpineVector3D(0, 0, -1f, 0);
        public float x, y, z, w;

        public AlpineVector3D(float x_axis = 0, float y_axis = 0, float z_axis = 0, float w_axis = 0) : base()
        { 
            this.x = x_axis;
            this.y = y_axis;
            this.z = z_axis;
            this.w = w_axis;
        }

        public AlpineVector3D Clone()
        {
            return new AlpineVector3D(x, y, z, w);
        }

        //This region is for events that use all 4 axis.
        #region 4D Events
        public AlpineVector3D SetValues4D(float x_axis = 0, float y_axis = 0, float z_axis = 0, float w_axis = 0)
        {
            this.x = x_axis;
            this.y = y_axis;
            this.z = z_axis;
            this.w = w_axis;
            return this;
        }

        public AlpineVector3D Unit4D()
        {
            float loc1 = MathF.Sqrt(x * x + y * y + z * z + w * w);
            x /= loc1;
            y /= loc1;
            z /= loc1;
            w /= loc1;
            return this;
        }

        public float Normalize4D()
        {
            float loc1 = MathF.Sqrt(x * x + y * y + z * z + w * w);
            x /= loc1;
            y /= loc1;
            z /= loc1;
            w /= loc1;
            return loc1;
        }

        public float Magnitude4D()
        {
            return MathF.Sqrt(x * x + y * y + z * z + w * w);
        }

        public float MagnitudeSquared4D()
        {
            return x * x + y * y + z * z + w * w;
        }

        public AlpineVector3D CopyFrom4D(AlpineVector3D other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
            w = other.w;
            return this;
        }

        public AlpineVector3D Plus4D(AlpineVector3D other)
        {
            x += other.x;
            y += other.y;
            z += other.z;
            w += other.w;
            return this;
        }

        public AlpineVector3D Minus4D(AlpineVector3D other)
        {
            x -= other.x;
            y -= other.y;
            z -= other.z;
            w -= other.w;
            return this;
        }

        public AlpineVector3D Times4D(AlpineVector3D other)
        {
            x *= other.x;
            y *= other.y;
            z *= other.z;
            w *= other.w;
            return this;
        }

        public AlpineVector3D Divide4D(AlpineVector3D other)
        {
            x /= other.x;
            y /= other.y;
            z /= other.z;
            w /= other.w;
            return this;
        }
        #endregion

        //This region is for events that only use 3 axis.
        #region 3D Events
        public AlpineVector3D SetValues(float x_axis = 0, float y_axis = 0, float z_axis = 0)
        {
            this.x = x_axis;
            this.y = y_axis;
            this.z = z_axis;
            return this;
        }

        public AlpineVector3D Unit()
        {
            float loc1 = MathF.Sqrt(x * x + y * y + z * z);
            x /= loc1;
            y /= loc1;
            z /= loc1;
            return this;
        }

        public float Normalize()
        {
            float loc1 = MathF.Sqrt(x * x + y * y + z * z);
            x /= loc1;
            y /= loc1;
            z /= loc1;
            return loc1;
        }

        public float Magnitude()
        {
            return MathF.Sqrt(x * x + y * y + z * z);
        }

        public float MagnitudeSquared()
        {
            return x * x + y * y + z * z;
        }

        public AlpineVector3D CopyFrom(AlpineVector3D other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
            return this;
        }

        public AlpineVector3D Plus(AlpineVector3D other)
        {
            x += other.x;
            y += other.y;
            z += other.z;
            return this;
        }

        public AlpineVector3D Minus(AlpineVector3D other)
        {
            x -= other.x;
            y -= other.y;
            z -= other.z;
            return this;
        }

        public AlpineVector3D Times(float value)
        {
            x *= value;
            y *= value;
            z *= value;
            return this;
        }

        public AlpineVector3D Divide(float value)
        {
            x /= value;
            y /= value;
            z /= value;
            return this;
        }
        #endregion

        //This Region is for more general events.
        #region General Events
        public AlpineVector3D PlusInto(AlpineVector3D other, AlpineVector3D target)
        {
            target.x = x + other.x;
            target.y = y + other.y;
            target.z = z + other.z;
            target.w = w + other.w;
            return target;
        }

        public AlpineVector3D MinusInto(AlpineVector3D other, AlpineVector3D target)
        {
            target.x = x - other.x;
            target.y = y - other.y;
            target.z = z - other.z;
            target.w = w - other.w;
            return target;
        }

        public AlpineVector3D TimesVectorInto(AlpineVector3D other, AlpineVector3D target)
        {
            target.x = x * other.x;
            target.y = y * other.y;
            target.z = z * other.z;
            target.w = w * other.w;
            return target;
        }

        public AlpineVector3D TimesInto(float amount, AlpineVector3D target)
        {
            target.x = x * amount;
            target.y = y * amount;
            target.z = z * amount;
            target.w = w * amount;
            return target;
        }

        public AlpineVector3D DivideInto(float amount, AlpineVector3D target)
        {
            target.x = x * amount;
            target.y = y * amount;
            target.z = z * amount;
            target.w = w * amount;
            return target;
        }

        public AlpineVector3D TimesVector(AlpineVector3D other)
        {
            x *= other.x;
            y *= other.y;
            z *= other.z;
            w *= other.w;
            return this;
        }

        public AlpineVector3D NegateInto(AlpineVector3D other)
        {
            other.x = -x;
            other.y = -y;
            other.z = -z;
            other.w = -w;
            return other;
        }

        public AlpineVector3D Cross(AlpineVector3D other)
        {
            return new AlpineVector3D(y * other.z - z * other.y, z * other.z - x * other.z, x * other.y - y * other.x, 0);
        }

        public AlpineVector3D CrossInto(AlpineVector3D other, AlpineVector3D target)
        {
            float loc3 = x, loc4 = y, loc5 = z;
            float loc6 = other.x, loc7 = other.y, loc8 = other.z;
            target.x = loc4 * loc8 - loc5 * loc7;
            target.y = loc5 * loc6 - loc3 * loc8;
            target.z = loc3 * loc7 - loc4 * loc6;
            target.w = 0;
            return target;
        }

        public AlpineVector3D Lerp(AlpineVector3D other, float amount)
        {
            x += (other.x - x) * amount;
            y += (other.y - y) * amount;
            z += (other.z - z) * amount;
            w += (other.w - w) * amount;
            return this;
        }

        public AlpineVector3D LerpInto(AlpineVector3D other, float amount, AlpineVector3D target)
        {
            target.x = x + (other.x - x) * amount;
            target.y = y + (other.y - y) * amount;
            target.z = z + (other.z - z) * amount;
            target.w = w + (other.w - w) * amount;
            return this;
        }

        public float Dot(AlpineVector3D other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public float DotXZ(AlpineVector3D other)
        {
            return x * other.x + z * other.z;
        }

        public AlpineVector3D Negate()
        {
            x = -x;
            y = -y;
            z = -z;
            w = -w;
            return this;
        }

        public Matrix4x4 Tensor(AlpineVector3D other)
        {
            return new Matrix4x4(this.x * other.x, this.x * other.y, this.x * other.z, this.x * other.w, this.y * other.x, this.y * other.y, this.y * other.z, this.y * other.w, this.z * other.x, this.z * other.y, this.z * other.z, this.z * other.w, this.w * other.x, this.w * other.y, this.w * other.z, this.w * other.w);
        }

        public void RoundToZero(float range)
        {
            if (x < range && x > -range)
            {
                x = 0;
            }
            if(y < range && y > -range)
            {
                y = 0;
            }
            if (z < range && z > -range)
            {
                z = 0;
            }
        }
        #endregion

        //This Region is for some bool / string events.
        #region Bool / String Events
        public bool IsZero()
        {
            return x == 0 && y == 0 && z == 0 && w == 0;
        }

        public bool Valid()
        {
            return x == x && y == y && z == z & w == w;
        }

        public override string ToString()
        {
            return "[ X: " + x + ", Y: " + y + ", Z: " + z + ", W: " + w + " ]";
        }

        public string XYZToString()
        {
            return string.Join(" ", x, y, z);
        }

        public string XYZWToString()
        {
            return string.Join(" ", x, y, z, w);
        }

        public string ToStringFixed(uint fractionDigits)
        {
            return "[ " + x * fractionDigits + ", " + y * fractionDigits + ", " + z * fractionDigits + ", " + w * fractionDigits + " ]";
        }

        public string ToStringFixed(int fractionDigits)
        {
            return "[ X: " + x * fractionDigits + ", Y: " + y * fractionDigits + ", Z: " + z * fractionDigits + ", W: " + w * fractionDigits + " ]";
        }
        #endregion
    }
}
