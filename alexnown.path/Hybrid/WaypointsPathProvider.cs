using UnityEngine;

namespace alexnown.path
{
    [RequireComponent(typeof(PathComponent))]
    [RequireComponent(typeof(GizmosPathDrawer))]
    public class WaypointsPathProvider : APathProvider
    {
        public Vector3[] Points = new[] { Vector3.zero, Vector3.right };
        [SerializeField]
        private bool _isCyclic = false;
        private PathComponent _path;

        public override Vector3 GetPointPosition(int pointIndex) => transform.TransformPoint(Points[pointIndex]);
        public override void SetPointPosition(int pointIndex, Vector3 worldPos)
            => Points[pointIndex] = transform.InverseTransformPoint(worldPos);

        public override void CachePath()
        {
            if (LockedAxis != Axis.None) LockAxis(LockedAxis);
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

        public override Vector3 GetPositionBetweenPoints(int first, int second, float ratio = 0.5f)
        {
            return Vector3.Lerp(GetPointPosition(first), GetPointPosition(second), ratio);
        }

        public override void LockAxis(Axis axis)
        {
            LockedAxis = axis;
            if (axis == Axis.None) return;
            for (var i = 0; i < Points.Length; i++)
            {
                Points[i] = LockPositionAxis(Points[i], axis);
            }
        }
    }
}