using Unity.Entities;

namespace alexnown.path
{
    public enum MovingType : byte
    {
        Clamp,
        Yoyo,
        Incremental
    }
    [GenerateAuthoringComponent]
    public struct MovingAlongPath : IComponentData
    {
        public float MoveSpeed;
        public MovingType Type;
    }
}