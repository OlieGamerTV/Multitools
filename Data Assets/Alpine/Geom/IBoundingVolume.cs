using Utilities.Flash;

namespace Alpine.Geom
{
    public interface IBoundingVolume
    {
        IBoundingVolume Clone();
        void Transform(AlpineMatrix3D param1, float param2);
        void Bound(IBoundingVolume param1);
        void BoundVertices(ByteArray param1, float param2, bool param3 = false);
    }
}