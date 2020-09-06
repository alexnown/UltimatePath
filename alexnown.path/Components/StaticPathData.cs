using Unity.Entities;
using Unity.Mathematics;

namespace alexnown.path
{
    public struct StaticPathData
    {
        public BlobArray<float3> Points;
        public BlobArray<float> Distances;
    }
}