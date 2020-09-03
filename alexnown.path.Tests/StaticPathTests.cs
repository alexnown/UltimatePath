using NUnit.Framework;
using UnityEngine;

namespace alexnown.path
{
    public class StaticPathTests
    {
        private static float[] _passedLengths = new float[] { -100, 0, 1, 1.0001f, 2.5f, 3, 3.5f, 4, 100 };
        private static int[] _startSegments = new int[] { -100, 0, 1, 2, 3, 4, 100 };
        public NonUniformPath CreateSquarePath() => new NonUniformPath { Points = new[] { Vector3.zero, Vector3.up, new Vector3(1, 1, 0), Vector3.right } };

        [Test]
        public void CheckInitialization()
        {
            var path = new NonUniformPath();
            Assert.IsNull(path.Points);
            Assert.IsNull(path.Distances);
            path.Points = new Vector3[0];
            path.RecalculateDistances();
            Assert.IsNull(path.Distances);
            Assert.AreEqual(0, path.TotalLength);
            path.Points = new[] { Vector3.zero, Vector3.up };
            path.RecalculateDistances();
            Assert.AreEqual(1, path.SegmentsCount);
            Assert.AreEqual(1, path.TotalLength);
        }

        [Test]
        public void CheckRecalculateDistances()
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(3, path.TotalLength);
            Assert.AreEqual(1f, path.Distances[0]);
            Assert.AreEqual(2f, path.Distances[1]);
            Assert.AreEqual(3f, path.Distances[2]);
        }

        [Test]
        public void FindSegmentUnsafe_TestPassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var expectedSegment = Mathf.Max(0, Mathf.CeilToInt(passedPath) - 1);
            if (passedPath < 0 || expectedSegment > path.SegmentsCount - 1)
                Assert.Throws(typeof(System.IndexOutOfRangeException), () => path.FindSegmentRecursively(passedPath, 0));
            else Assert.AreEqual(expectedSegment, path.FindSegmentRecursively(passedPath, 0));
        }

        [Test]
        public void FindSegmentUnsafe_TestStartIndexArgument([ValueSource(nameof(_startSegments))] int startIndex)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var passedPath = 1.5f;
            if (startIndex < 0 || startIndex >= path.SegmentsCount)
                Assert.Throws(typeof(System.IndexOutOfRangeException), () => path.FindSegmentRecursively(passedPath, startIndex));
            else Assert.DoesNotThrow(() => path.FindSegmentRecursively(passedPath, startIndex));
        }

        [Test]
        public void FindSegmentSafe_TestPassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var expectedSegment = Mathf.Clamp(Mathf.CeilToInt(passedPath) - 1, 0, path.SegmentsCount - 1);
            Assert.AreEqual(expectedSegment, path.FindSegment(passedPath));
        }

        [Test]
        public void FindSegmentSafe_TestStartIndexArgument([ValueSource(nameof(_startSegments))] int startIndex)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(1, path.FindSegment(1.5f, startIndex));
        }

        [Test]
        public void CalculatePosition_PassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Vector3 expectedPosition;
            if (passedPath <= 0) expectedPosition = path.Points[0];
            else if (passedPath >= path.TotalLength) expectedPosition = path.Points[path.Points.Length - 1];
            else
            {
                int pointIndex = Mathf.FloorToInt(passedPath);
                float t = Mathf.Repeat(passedPath, 1);
                expectedPosition = Vector3.Lerp(path.Points[pointIndex], path.Points[pointIndex + 1], t);
            }
            Assert.AreEqual(expectedPosition, path.CalculatePosition(passedPath));
        }

        [Test]
        public void CalculatePosition_StartIndexArgument([ValueSource(nameof(_startSegments))] int startIndex)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var passedPath = 1.5f;
            var expectedPosition = path.CalculatePosition(passedPath);
            Assert.AreEqual(expectedPosition, path.CalculatePosition(passedPath, ref startIndex));
        }

        [Test]
        public void CalculatePosition_SavingSegmentIndex([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            int refSegment = 0;
            path.CalculatePosition(passedPath, ref refSegment);
            Assert.AreEqual(path.FindSegment(passedPath), refSegment);
        }
    }
}