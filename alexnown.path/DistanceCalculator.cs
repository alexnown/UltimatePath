using System.Collections.Generic;
using UnityEngine;

namespace alexnown.path
{
    public static class DistanceCalculator
    {
        public static float CycleMoving(float startDistance, float moveSpeed, float dt, float totalPathLength)
            => Mathf.Repeat(startDistance + moveSpeed * dt, totalPathLength);

        public static float YoyoMoving(float startDistance, ref float moveSpeed, float dt, float totalPathLength)
        {
            float newDistance = startDistance + moveSpeed * dt;
            if (newDistance < 0)
            {
                moveSpeed = Mathf.Abs(moveSpeed);
                newDistance = Mathf.Abs(newDistance);
            }
            else if (newDistance > totalPathLength)
            {
                moveSpeed = -Mathf.Abs(moveSpeed);
                newDistance = 2 * totalPathLength - newDistance;
            }
            return newDistance;
        }

        public static float Moving(float startDistance, float moveSpeed, float dt, float totalPathLength)
            => Mathf.Clamp(startDistance + moveSpeed * dt, 0, totalPathLength);

        public static void SegmentingPathSection(APathProvider path, int firstIndex, int secondIndex, int segmentsCount, List<Vector3> positions)
        {
            float segmentSize = 1f / segmentsCount;
            float ratio = 0;
            for (int i = 0; i < segmentsCount - 1; i++)
            {
                ratio += segmentSize;
                var nextPosition = path.GetPositionBetweenPoints(firstIndex, secondIndex, ratio);
                positions.Add(nextPosition);
            }
            var lastPosition = path.GetPointPosition(secondIndex);
            positions.Add(lastPosition);
        }

        public static List<Vector3> ToUniformPath(List<Vector3> positions, float[] distances, float segmentLength)
        {
            var uniform = new List<Vector3>();
            ToUniformPath(positions, distances, segmentLength, uniform);
            return uniform;
        }
        public static void ToUniformPath(List<Vector3> positions, float segmentLength, List<Vector3> uniformPositionsStore)
            => ToUniformPath(positions, CalculateDistances(positions), segmentLength, uniformPositionsStore);
        public static void ToUniformPath(List<Vector3> positions, float[] distances, float segmentLength, List<Vector3> uniformPositionsStore)
        {
            uniformPositionsStore.Add(positions[0]);
            float previousDistance = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                var segmentDistance = distances[i];
                while (previousDistance + segmentDistance >= segmentLength)
                {
                    var ratio = (segmentLength - previousDistance) / segmentDistance;
                    var pointPos = Vector3.Lerp(positions[i], positions[i + 1], ratio);
                    uniformPositionsStore.Add(pointPos);
                    previousDistance -= segmentLength;
                }
                previousDistance += segmentDistance;
            }
            var lastPosition = positions[positions.Count - 1];
            if (previousDistance < 0.33f * segmentLength) uniformPositionsStore[uniformPositionsStore.Count - 1] = lastPosition;
            else uniformPositionsStore.Add(lastPosition);
        }

        public static float[] CalculateDistances(List<Vector3> positions)
        {
            var distances = new float[positions.Count - 1];
            for (int i = 0; i < positions.Count - 1; i++)
            {
                distances[i] = Vector3.Distance(positions[i], positions[i + 1]);
            }
            return distances;
        }

        public static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 part1 = Mathf.Pow(1 - t, 3) * p1;
            Vector3 part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
            Vector3 part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
            Vector3 part4 = Mathf.Pow(t, 3) * p4;

            return part1 + part2 + part3 + part4;
        }

        public static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);

            Vector3 part1 = Mathf.Pow(1 - t, 2) * p1;
            Vector3 part2 = 2 * (1 - t) * t * p2;
            Vector3 part3 = Mathf.Pow(t, 2) * p3;

            return part1 + part2 + part3;
        }

        public static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t)
        {
            return p1 + ((p2 - p1) * t);
        }
    }
}