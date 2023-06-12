using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Geom
{
    public class AlpineMatrix3D
    {
        public static AlpineMatrix3D Temp = new AlpineMatrix3D();
        private static AlpineVector3D tempVector = new AlpineVector3D();
        private static List<float> tempNumbers = new List<float>(16);

        public float _00, _01, _02, _03, _10, _11, _12, _13, _20, _21, _22, _23, _30, _31, _32, _33;

        public AlpineMatrix3D(float m00 = 1, float m01 = 0, float m02 = 0, float m03 = 0, float m10 = 0, float m11 = 1, float m12 = 0, float m13 = 0, float m20 = 0, float m21 = 0, float m22 = 1, float m23 = 0, float m30 = 0, float m31 = 0, float m32 = 0, float m33 = 1) : base()
        {
            _00 = m00;
            _01 = m01;
            _02 = m02;
            _03 = m03;
            _10 = m10;
            _11 = m11;
            _12 = m12;
            _13 = m13;
            _20 = m20;
            _21 = m21;
            _22 = m22;
            _23 = m23;
            _30 = m30;
            _31 = m31;
            _32 = m32;
            _33 = m33;
        }

        public AlpineMatrix3D Identity()
        {
            this._00 = 1;
            this._01 = 0;
            this._02 = 0;
            this._03 = 0;
            this._10 = 0;
            this._11 = 1;
            this._12 = 0;
            this._13 = 0;
            this._20 = 0;
            this._21 = 0;
            this._22 = 1;
            this._23 = 0;
            this._30 = 0;
            this._31 = 0;
            this._32 = 0;
            this._33 = 1;
            return this;
        }

        public AlpineMatrix3D SetValues(float m00 = 0, float m01 = 0, float m02 = 0, float m03 = 0, float m10 = 0, float m11 = 0, float m12 = 0, float m13 = 0, float m20 = 0, float m21 = 0, float m22 = 0, float m23 = 0, float m30 = 0, float m31 = 0, float m32 = 0, float m33 = 0)
        {
            _00 = m00;
            _01 = m01;
            _02 = m02;
            _03 = m03;
            _10 = m10;
            _11 = m11;
            _12 = m12;
            _13 = m13;
            _20 = m20;
            _21 = m21;
            _22 = m22;
            _23 = m23;
            _30 = m30;
            _31 = m31;
            _32 = m32;
            _33 = m33;
            return this;
        }

        public AlpineMatrix3D CopyFrom(AlpineMatrix3D other)
        {
            _00 = other._00;
            _01 = other._01;
            _02 = other._02;
            _03 = other._03;
            _10 = other._10;
            _11 = other._11;
            _12 = other._12;
            _13 = other._13;
            _20 = other._20;
            _21 = other._21;
            _22 = other._22;
            _23 = other._23;
            _30 = other._30;
            _31 = other._31;
            _32 = other._32;
            _33 = other._33;
            return this;
        }

        public AlpineMatrix3D CopyFrom3D(AlpineMatrix3D other)
        {
            _00 = other._00;
            _01 = other._01;
            _02 = other._02;
            _10 = other._10;
            _11 = other._11;
            _12 = other._12;
            _20 = other._20;
            _21 = other._21;
            _22 = other._22;
            return this;
        }

        public List<float> ToList()
        {
            List<float> list = new List<float>();
            for (int i = 0; i < 15; i++)
            {
                switch (i)
                {
                    case 0:
                        list.Add(_00);
                        break;
                    case 1:
                        list.Add(_01);
                        break;
                    case 2:
                        list.Add(_02);
                        break;
                    case 3:
                        list.Add(_03);
                        break;
                    case 4:
                        list.Add(_10);
                        break;
                    case 5:
                        list.Add(_11);
                        break;
                    case 6:
                        list.Add(_12);
                        break;
                    case 7:
                        list.Add(_13);
                        break;
                    case 8:
                        list.Add(_20);
                        break;
                    case 9:
                        list.Add(_21);
                        break;
                    case 10:
                        list.Add(_22);
                        break;
                    case 11:
                        list.Add(_23);
                        break;
                    case 12:
                        list.Add(_30);
                        break;
                    case 13:
                        list.Add(_31);
                        break;
                    case 14:
                        list.Add(_32);
                        break;
                    case 15:
                        list.Add(_33);
                        break;
                }
            }
            return list;
        }
    }
}
