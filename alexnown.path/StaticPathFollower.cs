using UnityEngine;

namespace alexnown.path
{
    public class StaticPathFollower : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathContainer = null;

        [SerializeField]
        private float _passedDistance;
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
        public float PathLength => Path.TotalLength;

        public void SetDistancePassed(float distance)
        {
            _passedDistance = distance;
            transform.position = Path.CalculatePosition(DistancePassed);
        }
    }
}