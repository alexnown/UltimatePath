using UnityEngine;

namespace alexnown.path
{
    public class PathComponent : MonoBehaviour, IDrawablePath
    {
        [SerializeField]
        private bool _isNonUniformPath;
        [SerializeField]
        private NonUniformPath _nonUniformPath;

        public IPath Path
        {
            get
            {
                if (_isNonUniformPath) return _nonUniformPath;
                return null;
            }
        }

        public Vector3[] Waypoints => Path?.Points;

        public void StorePath(IPath path)
        {
            _isNonUniformPath = path is NonUniformPath;
            _nonUniformPath = path as NonUniformPath;
        }
    }
}