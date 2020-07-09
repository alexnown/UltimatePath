using UnityEngine;

namespace alexnown.path
{
    public class WaypointsPathCreator : MonoBehaviour, IDrawablePath, IStaticPathContainer
    {
        public StaticPath Path => _path;
        public bool StorePointsInLocalSpace => _storePointsInLocalSpace;
        public Vector3[] Waypoints => _path?.Points;
        public bool IsCyclic => _path.IsCyclic;

        public Vector3[] Points = new[] { Vector3.zero, Vector3.right };
        [SerializeField]
        private bool _storePointsInLocalSpace = true;
        [SerializeField]
        private StaticPath _path;

        public void ChangePointsSpace(bool isLocal)
        {
            if (_storePointsInLocalSpace == isLocal) return;
            _storePointsInLocalSpace = isLocal;
            if (Points == null) return;
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = isLocal ? transform.InverseTransformPoint(Points[i])
                                    : transform.TransformPoint(Points[i]);
            }
        }
        public Vector3 GetPointPosition(int pointIndex)
        {
            var pos = Points[pointIndex];
            return StorePointsInLocalSpace ? transform.TransformPoint(pos) : pos;
        }
        public void SetPointPosition(int pointIndex, Vector3 worldPos)
        {
            if (StorePointsInLocalSpace) worldPos = transform.InverseTransformPoint(worldPos);
            Points[pointIndex] = worldPos;
        }
        public void CachePath()
        {
            if (_path == null) _path = new StaticPath();
            var currPathLength = Path.Points == null ? 0 : Path.Points.Length;
            if (Points.Length != currPathLength)
                Path.Points = new Vector3[Points.Length];
            for (int i = 0; i < Points.Length; i++)
                Path.Points[i] = GetPointPosition(i);
            Path.RecalculateDistances();
        }
    }
}