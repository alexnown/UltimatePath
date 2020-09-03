using Unity.Entities;

namespace alexnown.path
{
    [GenerateAuthoringComponent]
    public struct BindPathReference : IComponentData
    {
        public Entity Path;
    }
}