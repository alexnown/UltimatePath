using UnityEngine;

namespace alexnown.path
{
    [RequireComponent(typeof(PathComponent))]
    [RequireComponent(typeof(GizmosPathDrawer))]
    public class WaypointsPathProvider : MonoBehaviour
    {
        public Vector3[] Points = new[] { Vector3.zero, Vector3.right };
        [SerializeField]
        private bool _isCyclic;
        private PathComponent _path;

        public Vector3 GetPointPosition(int pointIndex) => transform.TransformPoint(Points[pointIndex]);
        public void SetPointPosition(int pointIndex, Vector3 worldPos)
            => Points[pointIndex] = transform.InverseTransformPoint(worldPos);

        public void CachePath()
        {
            if (_path == null) _path = GetComponent<PathComponent>();
            var nonUniform = _path.Path as NonUniformPath;
            if (nonUniform == null) nonUniform = new NonUniformPath();
            var currPathLength = nonUniform.Points == null ? 0 : nonUniform.Points.Length;
            var requiredPoints = Points.Length + (_isCyclic ? 1 : 0);
            if (currPathLength != requiredPoints)
                nonUniform.Points = new Vector3[requiredPoints];
            for (int i = 0; i < requiredPoints; i++)
                nonUniform.Points[i] = GetPointPosition(i % Points.Length);
            nonUniform.RecalculateDistances();
            _path.StorePath(nonUniform);
        }
    }
}