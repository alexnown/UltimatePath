using UnityEngine;

namespace alexnown.path
{
    [System.Serializable]
    public class StaticPath
    {
        public bool IsCyclic;
        public Vector3[] Points;
        public float[] Distances;

        public void RecalculateDistances()
        {
            if (Points == null) return;
            var distancesLength = Distances == null ? 0 : Distances.Length;
            if (distancesLength != Points.Length) Distances = new float[Points.Length];
            if (Points.Length == 0) return;
            float distanceSum = 0;
            for (int i = 0; i < Points.Length; i++)
            {
                int nextIndex = (i + 1) % Points.Length;
                distanceSum += Vector3.Distance(Points[i], Points[nextIndex]);
                Distances[i] = distanceSum;
            }
        }
        public int SegmentsCount() => SegmentsCount(IsCyclic);
        public int SegmentsCount(bool isCyclic)
        {
            if (Distances == null || Distances.Length < 2) return 0;
            return Distances.Length - (isCyclic ? 0 : 1);
        }
        public float GetLength() => GetLength(IsCyclic);
        public float GetLength(bool isCyclic)
        {
            if (Distances == null || Distances.Length < 2) return 0;
            var lastIndex = Distances.Length - (isCyclic ? 1 : 2);
            return Distances[lastIndex];
        }
        public int FindSegmentSafe(float passedPath) => FindSegmentSafe(passedPath, IsCyclic, 0);
        public int FindSegmentSafe(float passedPath, bool isCyclic) => FindSegmentSafe(passedPath, isCyclic, 0);
        public int FindSegmentSafe(float passedPath, int startIndex) => FindSegmentSafe(passedPath, IsCyclic, startIndex);
        public int FindSegmentSafe(float passedPath, bool isCyclic, int startIndex)
        {
            if (passedPath <= 0) return 0;
            if (passedPath >= GetLength(isCyclic)) return Distances.Length - (isCyclic ? 1 : 2);
            return FindSegmentWithoutChecks(passedPath, Mathf.Clamp(startIndex, 0, Distances.Length - 1));
        }
        public int FindSegmentWithoutChecks(float passedPath, int startIndex)
        {
            var segmentPath = Distances[startIndex];
            if (segmentPath < passedPath) return FindSegmentWithoutChecks(passedPath, startIndex + 1);
            var segmentStartPath = startIndex > 0 ? Distances[startIndex - 1] : 0;
            if (segmentStartPath > passedPath) return FindSegmentWithoutChecks(passedPath, startIndex - 1);
            return startIndex;
        }
        public Vector3 CalculatePosition(float passedPath) => CalculatePosition(passedPath, IsCyclic);
        public Vector3 CalculatePosition(float passedPath, bool isCyclic)
        {
            int lastIndex = 0;
            return CalculatePosition(passedPath, isCyclic, ref lastIndex);
        }
        public Vector3 CalculatePosition(float passedPath, ref int lastIntervalIndex) => CalculatePosition(passedPath, IsCyclic, ref lastIntervalIndex);
        public Vector3 CalculatePosition(float passedPath, bool isCyclic, ref int lastIntervalIndex)
        {
            if (passedPath <= 0)
            {
                lastIntervalIndex = 0;
                return Points[lastIntervalIndex];
            }
            else if (passedPath >= GetLength(isCyclic))
            {
                lastIntervalIndex = isCyclic ? 0 : Distances.Length - 2;
                return isCyclic ? Points[0] : Points[lastIntervalIndex + 1];
            }
            lastIntervalIndex = FindSegmentSafe(passedPath, isCyclic, lastIntervalIndex);
            var firstDistance = lastIntervalIndex > 0 ? Distances[lastIntervalIndex - 1] : 0;
            var lerpValue = Mathf.InverseLerp(firstDistance, Distances[lastIntervalIndex], passedPath);
            var nextPointIndex = (lastIntervalIndex + 1) % Points.Length;
            return Vector3.Lerp(Points[lastIntervalIndex], Points[nextPointIndex], lerpValue);
        }
    }
}