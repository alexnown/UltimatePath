using UnityEngine;

namespace alexnown.path
{
    public class StaticPathWalker : MonoBehaviour
    {
        public GameObject PathContainer;
        [Header("Move values")]
        public float PassedPath;
        public float MoveSpeed;
        public bool YoyoMoving;
        public bool OverridePathCycleValue;
        public bool IsCycle;
        private StaticPath _path;

        public void SetPath(StaticPath path) => _path = path;

        private void Start()
        {
            if (_path == null) _path = PathContainer?.GetComponent<IStaticPathContainer>()?.Path;
        }

        private void Update()
        {
            if (_path == null || MoveSpeed == 0) return;
            bool isCycleMoving = OverridePathCycleValue ? IsCycle : _path.Cyclic;
            var newLength = PassedPath + Time.deltaTime * MoveSpeed;
            var totalPathLength = _path.GetLength(isCycleMoving);
            if (isCycleMoving)
            {
                newLength = Mathf.Repeat(newLength, totalPathLength);
            }
            else
            {
                if (YoyoMoving)
                {
                    if (newLength < 0) MoveSpeed = Mathf.Abs(MoveSpeed);
                    else if (newLength > totalPathLength) MoveSpeed = -Mathf.Abs(MoveSpeed);
                }
                newLength = Mathf.Clamp(newLength, 0, totalPathLength);
            }
            if (newLength == PassedPath) return;
            PassedPath = newLength;
            transform.position = _path.CalculatePosition(PassedPath, isCycleMoving);
        }
    }
}