using Unity.Entities;

namespace alexnown.path
{
    public struct StaticPathReference : IComponentData
    {
        public BlobAssetReference<StaticPathData> Value;
    }
}