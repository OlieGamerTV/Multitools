using Alpine.Renderables.Models.Skeleton;
using Utilities.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Animation.Manipulator
{
    internal class UVManipulator : Manipulator
    {
        private Mesh mesh;
        private float rateX, rateY, minX, minY, maxX, maxY;
        public UVManipulator(Mesh arg1, Point arg2, Point arg3 = null, Point arg4 = null)
        {
            mesh = arg1;
            rateX = arg2.x;
            rateY = arg2.y;
            if (arg3 == null)
            {
                minX = minY = 0;
            }
            else
            {
                minX = arg3.x;
                minY = arg3.y;
            }
            if (arg4 == null)
            {
                maxX = maxY = 0;
            }
            else
            {
                maxX = arg4.x;
                maxY = arg4.y;
            }
            if (mesh.uvOffset == null)
            {
                mesh.uvOffset = new Point();
            }
        }

        public void Destroy()
        {
            mesh.uvOffset = null;
            mesh = null;
        }

        public Mesh Target
        {
            get { return mesh; }
        }

        public void Rate(Point arg1)
        {
            rateX = arg1.x;
            rateY = arg1.y;
        }

        public void Update(int arg1)
        {
            if (mesh.uvOffset == null)
            {
                return;
            }
            float loc2 = (arg1 / 1000);
            mesh.uvOffset.x = (mesh.uvOffset.x + (rateX * loc2));
            mesh.uvOffset.y = (mesh.uvOffset.y + (rateY * loc2));
        }
    }
}
