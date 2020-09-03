using Unity.Entities;

namespace alexnown.path
{
    public struct DistancePassed : IComponentData
    {
        public float Value;
        public bool OverrideIsCycleValue;
        public bool RelevantIsCycle;
    }
}