using UnityEngine;

namespace alexnown.path
{
    public class WaypointsPathCreator : MonoBehaviour, IDrawablePath, IStaticPathContainer
    {
        public StaticPath Path => _path;
        public Vector3[] Waypoints => _path?.Points;
        public bool IsCyclic { get { return _isCyclic; } set { _isCyclic = value; } }

        public Vector3[] Points = new[] { Vector3.zero, Vector3.right };
        [SerializeField]
        private StaticPath _path;
        [SerializeField]
        private bool _isCyclic;

        public Vector3 GetPointPosition(int pointIndex) => transform.TransformPoint(Points[pointIndex]);
        public void SetPointPosition(int pointIndex, Vector3 worldPos)
            => Points[pointIndex] = transform.InverseTransformPoint(worldPos);

        public void CachePath()
        {
            if (_path == null) _path = new StaticPath();
            var currPathLength = Path.Points == null ? 0 : Path.Points.Length;
            var requiredPoints = Points.Length + (IsCyclic ? 1 : 0);
            if (currPathLength != requiredPoints)
                Path.Points = new Vector3[requiredPoints];
            for (int i = 0; i < requiredPoints; i++)
                Path.Points[i] = GetPointPosition(i % Points.Length);
            Path.RecalculateDistances();
        }
    }
}