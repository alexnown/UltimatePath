using UnityEngine;
using Unity.Entities;

namespace alexnown.path
{
    [ExecuteAlways]
    public class StaticPathFollowerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private PathComponent _pathContainer = null;

        [SerializeField]
        private float _passedDistance;
        private IPath _cachedPath;
        public IPath Path
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) _cachedPath = _pathContainer?.Path;
#endif
                if (_cachedPath == null) _cachedPath = _pathContainer?.Path;
                return _cachedPath;
            }
            set
            {
                _cachedPath = value;
            }
        }
        public float DistancePassed => _passedDistance;
        public float PathLength => Path == null ? 0 : Path.TotalLength;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new DistancePassed { Value = DistancePassed });
        }

        public void SetDistancePassed(float distance)
        {
            _passedDistance = distance;
            if (Path != null) transform.position = Path.CalculatePosition(DistancePassed);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (_pathContainer?.gameObject != gameObject)
                SetDistancePassed(_passedDistance);
        }
#endif
    }
}