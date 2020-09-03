using UnityEngine;

namespace alexnown.path
{
    [System.Serializable]
    public class NonUniformPath : IPath
    {
        [SerializeField]
        private Vector3[] _points;
        [SerializeField]
        private float[] _distances;
        public Vector3[] Points { get => _points; set => _points = value; }
        public float[] Distances => _distances; 

        public float TotalLength => _distances[SegmentsCount - 1];

        public int SegmentsCount => _distances.Length;

        public Vector3 CalculatePosition(float passedPath)
        {
            int index = 0;
            return CalculatePosition(passedPath, ref index);
        }

        public Vector3 CalculatePosition(float passedPath, ref int lastSegmentIndex)
        {
            if (passedPath <= 0)
            {
                lastSegmentIndex = 0;
                return Points[lastSegmentIndex];
            }
            else if (passedPath >= TotalLength)
            {
                lastSegmentIndex = SegmentsCount - 1;
                return Points[lastSegmentIndex + 1];
            }
            lastSegmentIndex = FindSegment(passedPath, lastSegmentIndex);
            var firstDistance = lastSegmentIndex > 0 ? Distances[lastSegmentIndex - 1] : 0;
            var lerpValue = Mathf.InverseLerp(firstDistance, Distances[lastSegmentIndex], passedPath);
            var nextPointIndex = (lastSegmentIndex + 1) % Points.Length;
            return Vector3.Lerp(Points[lastSegmentIndex], Points[nextPointIndex], lerpValue);
        }

        public int FindSegment(float passedPath) => FindSegment(passedPath, 0);

        public int FindSegment(float passedPath, int lastSegmentIndex)
        {
            if (passedPath <= 0) return 0;
            if (passedPath >= TotalLength) return SegmentsCount - 1;
            return FindSegmentRecursively(passedPath, Mathf.Clamp(lastSegmentIndex, 0, SegmentsCount - 1));
        }

        public int FindSegmentRecursively(float passedPath, int startIndex)
        {
            var segmentPath = _distances[startIndex];
            if (segmentPath < passedPath) return FindSegmentRecursively(passedPath, startIndex + 1);
            var segmentStartPath = startIndex > 0 ? _distances[startIndex - 1] : 0;
            if (segmentStartPath > passedPath) return FindSegmentRecursively(passedPath, startIndex - 1);
            return startIndex;
        }

        public void RecalculateDistances()
        {
            if (Points == null || Points.Length < 2) throw new System.InvalidOperationException("Path must consist of at least two points");
            var distancesLength = Distances == null ? 0 : Distances.Length;
            if (distancesLength != Points.Length - 1) _distances = new float[Points.Length - 1];
            float distanceSum = 0;
            for (int i = 0; i < Distances.Length; i++)
            {
                distanceSum += Vector3.Distance(Points[i], Points[i + 1]);
                Distances[i] = distanceSum;
            }
        }
    }
}