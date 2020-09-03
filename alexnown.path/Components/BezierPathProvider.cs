using System.Collections.Generic;
using UnityEngine;

namespace alexnown.path
{
    [RequireComponent(typeof(PathComponent))]
    [RequireComponent(typeof(GizmosPathDrawer))]
    public class BezierPathProvider : APathProvider
    {
        [SerializeField]
        private bool _isCyclic = false;
        public BezierPoint[] Points = new[] {
            new BezierPoint { Position = Vector3.zero, Handle1 = 0.5f * Vector3.left, Handle2 = 0.5f * Vector3.right },
            new BezierPoint { Position = Vector3.up, Handle1 = 0.5f * Vector3.left, Handle2 = 0.5f * Vector3.right }
        };
        [Header("Segmenting")]
        public int SegmentsPerUnit = 20;
        private PathComponent _path;
        private List<Vector3> _cachedPositions;
        private List<Vector3> _uniformPoinsts;
        public bool IsCyclic => _isCyclic;
        public override Vector3 GetPointPosition(int pointIndex) => transform.TransformPoint(Points[pointIndex].Position);
        public Vector3 GetPointPositionAndHandlers(int pointIndex, out Vector3 handler1, out Vector3 handler2)
        {
            var data = Points[pointIndex];
            handler1 = transform.TransformPoint(data.Handle1 + data.Position);
            handler2 = transform.TransformPoint(data.Handle2 + data.Position);
            return transform.TransformPoint(data.Position);
        }
        public override void SetPointPosition(int pointIndex, Vector3 worldPos)
            => Points[pointIndex].Position = transform.InverseTransformPoint(worldPos);

        public override void CachePath()
        {
            if (LockedAxis != Axis.None) LockAxis(LockedAxis);
            if (_path == null) _path = GetComponent<PathComponent>();
            if (_cachedPositions == null) _cachedPositions = new List<Vector3>();
            else _cachedPositions.Clear();
            if (_uniformPoinsts == null) _uniformPoinsts = new List<Vector3>();
            else _uniformPoinsts.Clear();
            _cachedPositions.Add(GetPointPosition(0));
            for (int i = 0; i < Points.Length - 1; i++)
            {
                SegmentingSection(i, i + 1, SegmentsPerUnit, _cachedPositions);
            }
            if (IsCyclic) SegmentingSection(Points.Length - 1, 0, SegmentsPerUnit, _cachedPositions);
            var distances = DistanceCalculator.CalculateDistances(_cachedPositions);
            float totalDistance = 0;
            foreach (var distance in distances) totalDistance += distance;
            var pointsCount = Mathf.CeilToInt(totalDistance * SegmentsPerUnit);
            var segmentLength = totalDistance / pointsCount;
            DistanceCalculator.ToUniformPath(_cachedPositions, distances, segmentLength, _uniformPoinsts);
            var uniform = new UniformPath
            {
                TotalLength = totalDistance,
                SegmentLength = segmentLength,
                Points = _uniformPoinsts.ToArray()
            };
            _path.StorePath(uniform);
        }

        public override Vector3 GetPositionBetweenPoints(int first, int second, float ratio)
            => GetPositionBetweenPoints(first, second, ratio, out var firstPos, out var secondPos);
        public Vector3 GetPositionBetweenPoints(int first, int second, float ratio, out Vector3 firstPosition, out Vector3 secondPosition)
        {
            var data1 = Points[first];
            var data2 = Points[second];
            firstPosition = transform.TransformPoint(data1.Position);
            secondPosition = transform.TransformPoint(data2.Position);
            var handle1 = transform.TransformPoint(data1.Handle2 + data1.Position);
            var handle2 = transform.TransformPoint(data2.Handle1 + data2.Position);
            return DistanceCalculator.GetCubicCurvePoint(firstPosition, handle1, handle2, secondPosition, ratio);
        }
        public override void LockAxis(Axis axis)
        {
            LockedAxis = axis;
            if (axis == Axis.None) return;
            foreach (var pointData in Points)
            {
                pointData.Position = LockPositionAxis(pointData.Position, axis);
                pointData.Handle1 = LockPositionAxis(pointData.Handle1, axis);
                pointData.Handle2 = LockPositionAxis(pointData.Handle2, axis);
            }
        }
        public float ApproximateSegmentLength(int first, int second, int segmentsCount)
        {
            var data1 = Points[first];
            var data2 = Points[second];
            var firstPosition = transform.TransformPoint(data1.Position);
            var secondPosition = transform.TransformPoint(data2.Position);
            var handle1 = transform.TransformPoint(data1.Handle2 + data1.Position);
            var handle2 = transform.TransformPoint(data2.Handle1 + data2.Position);
            var segmentSize = 1f / segmentsCount;
            float ratio = 0, totalDistance = 0;
            var previousPosition = firstPosition;
            for (int i = 0; i < segmentsCount - 1; i++)
            {
                ratio += segmentSize;
                var newPosition = DistanceCalculator.GetCubicCurvePoint(firstPosition, handle1, handle2, secondPosition, ratio);
                totalDistance += Vector3.Distance(previousPosition, newPosition);
                previousPosition = newPosition;
            }
            totalDistance += Vector3.Distance(previousPosition, secondPosition);
            return totalDistance;
        }

        private int SegmentingSection(int first, int second, int segmentsPerUnit, List<Vector3> positions)
        {
            var approximatedLength = ApproximateSegmentLength(first, second, 4);

            var segmentsCount = Mathf.CeilToInt(approximatedLength * segmentsPerUnit);
            if (segmentsCount > 1000)
            {
                UnityEngine.Debug.LogWarning($"Bezier section ({first}->{second}) segmented at {segmentsCount} parts, recomended decrease {nameof(segmentsPerUnit)} value");
                segmentsCount = 1000;
            }
            DistanceCalculator.SegmentingPathSection(this, first, second, segmentsCount, positions);
            return segmentsCount;
        }

        [System.Serializable]
        public class BezierPoint
        {
            public HandleType Type;
            public Vector3 Position;
            [HideInInspector]
            public Vector3 Handle1;
            [HideInInspector]
            public Vector3 Handle2;

            public enum HandleType
            {
                Solid,
                Separated,
                None,
            }
        }
    }
}