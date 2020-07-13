using UnityEngine;

namespace alexnown.path
{
    [System.Serializable]
    public class StaticPath
    {
        public Vector3[] Points;
        public float[] Distances;

        public void RecalculateDistances()
        {
            if (Points == null || Points.Length < 2) return;
            var distancesLength = Distances == null ? 0 : Distances.Length;
            if (distancesLength != Points.Length - 1) Distances = new float[Points.Length - 1];
            float distanceSum = 0;
            for (int i = 0; i < Distances.Length; i++)
            {
                distanceSum += Vector3.Distance(Points[i], Points[i + 1]);
                Distances[i] = distanceSum;
            }
        }
        public int SegmentsCount => Distances.Length;
        public float TotalLength => Distances == null ? 0 : Distances[Distances.Length - 1];

        public int FindSegmentSafe(float passedPath) => FindSegmentSafe(passedPath, 0);
        public int FindSegmentSafe(float passedPath, int startIndex)
        {
            if (passedPath <= 0) return 0;
            if (passedPath >= TotalLength) return SegmentsCount - 1;
            return FindSegmentWithoutChecks(passedPath, Mathf.Clamp(startIndex, 0, SegmentsCount - 1));
        }
        public int FindSegmentWithoutChecks(float passedPath, int startIndex)
        {
            var segmentPath = Distances[startIndex];
            if (segmentPath < passedPath) return FindSegmentWithoutChecks(passedPath, startIndex + 1);
            var segmentStartPath = startIndex > 0 ? Distances[startIndex - 1] : 0;
            if (segmentStartPath > passedPath) return FindSegmentWithoutChecks(passedPath, startIndex - 1);
            return startIndex;
        }
        public Vector3 CalculatePosition(float passedPath)
        {
            int index = 0;
            return CalculatePosition(passedPath, ref index);
        }

        public Vector3 CalculatePosition(float passedPath, ref int lastIntervalIndex)
        {
            if (passedPath <= 0)
            {
                lastIntervalIndex = 0;
                return Points[lastIntervalIndex];
            }
            else if (passedPath >= TotalLength)
            {
                lastIntervalIndex = SegmentsCount - 1;
                return Points[lastIntervalIndex + 1];
            }
            lastIntervalIndex = FindSegmentSafe(passedPath, lastIntervalIndex);
            var firstDistance = lastIntervalIndex > 0 ? Distances[lastIntervalIndex - 1] : 0;
            var lerpValue = Mathf.InverseLerp(firstDistance, Distances[lastIntervalIndex], passedPath);
            var nextPointIndex = (lastIntervalIndex + 1) % Points.Length;
            return Vector3.Lerp(Points[lastIntervalIndex], Points[nextPointIndex], lerpValue);
        }
    }
}