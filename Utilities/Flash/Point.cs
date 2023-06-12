using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Flash
{
    public class Point
    {
        private float length;
        public float x = 0, y = 0;

        #region Creation Functions
        public Point(float pos_x = 0, float pos_y = 0)
        {
            x = pos_x;
            y = pos_y;
        }

        public Point(System.Drawing.Point drawPoint) : this(drawPoint.X, drawPoint.Y) { }

        public Point(System.Drawing.PointF drawPoint) : this(drawPoint.X, drawPoint.Y) { }

        public Point(System.Windows.Point windowsPoint) : this(Convert.ToSingle(windowsPoint.X), Convert.ToSingle(windowsPoint.Y)) { }
        #endregion

        #region Convert To Other Points Functions
        public System.Drawing.Point ToDrawPoint()
        {
            System.Drawing.Point point = new System.Drawing.Point();
            point.X = (int)this.x;
            point.Y = (int)this.y;
            return point;
        }

        public System.Drawing.PointF ToDrawPointF()
        {
            System.Drawing.PointF point = new System.Drawing.PointF();
            point.X = this.x;
            point.Y = this.y;
            return point;
        }

        public System.Windows.Point ToWindowsPoint()
        {
            System.Windows.Point point = new System.Windows.Point();
            point.X = this.x;
            point.Y = this.y;
            return point;
        }
        #endregion

        public Point Add(Point v)
        {
            x += v.x;
            y += v.y;
            return this;
        }

        public Point Subtract(Point v)
        {
            x -= v.x;
            y -= v.y;
            return this;
        }

        public Point Clone()
        {
            return new Point(x, y);
        }

        public void CopyFrom(Point sourcePoint)
        {
            x = sourcePoint.x;
            y = sourcePoint.y;
        }

        public bool Equals(Point toCompare)
        {
            return x.Equals(toCompare.x) && y.Equals(toCompare.y);
        }

        public void SetTo(float xa, float ya)
        {
            x = xa; y = ya;
        }

        public void Offset(float dx, float dy)
        {
            x+=dx; y+=dy;
        }

        public float Length
        {
            get { return length; }
        }

        public override string ToString()
        {
            return "(x=" + x.ToString() + ", y=" + y.ToString() + ")";
        }
    }
}
