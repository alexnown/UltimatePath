using UnityEngine;

namespace alexnown.path
{
    public class PathComponent : MonoBehaviour, IDrawablePath
    {
        [SerializeField]
        private bool _isNonUniformPath;
        [SerializeField]
        private NonUniformPath _nonUniformPath;
        [SerializeField]
        private bool _isUniformPath;
        [SerializeField]
        private UniformPath _uniformPath;

        public IPath Path
        {
            get
            {
                if (_isNonUniformPath) return _nonUniformPath;
                else if (_isUniformPath) return _uniformPath;
                return null;
            }
        }

        public Vector3[] Waypoints => Path?.Points;

        public void StorePath(IPath path)
        {
            _isNonUniformPath = path is NonUniformPath;
            _nonUniformPath = path as NonUniformPath;
            _isUniformPath = path is UniformPath;
            _uniformPath = path as UniformPath;
        }
    }
}