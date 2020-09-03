using UnityEngine;
using Unity.Entities;

namespace alexnown.path
{
    public class StaticPathFollower : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private GameObject _pathContainer = null;

        [SerializeField]
        private float _passedDistance;
        public bool OverrideIsCycleValue;
        public bool RelevantIsCycle;
        private StaticPath _cachedPath;
        public StaticPath Path
        {
            get
            {
                if (_cachedPath == null)
                {
                    if (_pathContainer == null) return null;
                    _cachedPath = _pathContainer.GetComponent<IStaticPathContainer>()?.Path;
                }
                return _cachedPath;
            }
            set
            {
                _cachedPath = value;
            }
        }
        public float DistancePassed => _passedDistance;
        public float PathLength => Path.GetLength(IsPathCyclic);
        public bool IsPathCyclic => OverrideIsCycleValue ? RelevantIsCycle : Path.IsCyclic;

        public void SetDistancePassed(float distance)
        {
            _passedDistance = distance;
            transform.position = Path.CalculatePosition(DistancePassed, IsPathCyclic);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new DistancePassed
            {
                Value = _passedDistance,
                OverrideIsCycleValue = OverrideIsCycleValue,
                RelevantIsCycle = RelevantIsCycle
            });
        }
    }
}