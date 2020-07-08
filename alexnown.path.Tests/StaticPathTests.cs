using NUnit.Framework;
using UnityEngine;

namespace alexnown.path
{
    public class StaticPathTests
    {
        private static float[] _passedLengths = new float[] { -100, 0, 1, 1.0001f, 2.5f, 3, 3.5f, 4, 100 };
        private static int[] _startSegments = new int[] { -100, 0, 1, 2, 3, 4, 100 };
        public StaticPath CreateSquarePath() => new StaticPath { Points = new[] { Vector3.zero, Vector3.up, new Vector3(1, 1, 0), Vector3.right } };

        [Test]
        public void CheckInitialization()
        {
            var path = new StaticPath();
            Assert.IsNull(path.Points);
            Assert.IsNull(path.Distances);
            Assert.IsFalse(path.Cyclic);
            path.Points = new Vector3[0];
            path.RecalculateDistances();
            Assert.IsNull(path.Distances);
            Assert.AreEqual(0, path.GetLength());
            path.Points = new[] { Vector3.zero, Vector3.up };
            path.RecalculateDistances();
            Assert.AreEqual(2, path.Distances.Length);
            Assert.AreEqual(1, path.GetLength());
            path.Cyclic = true;
            Assert.AreEqual(2, path.GetLength());
        }

        [Test]
        public void GetPathTotalLength()
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(3, path.GetLength(false));
            Assert.AreEqual(4, path.GetLength(true));
            path.Cyclic = false;
            Assert.AreEqual(path.GetLength(false), path.GetLength());
            path.Cyclic = true;
            Assert.AreEqual(path.GetLength(true), path.GetLength());
        }

        [Test]
        public void CheckRecalculateDistances()
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(1f, path.Distances[0]);
            Assert.AreEqual(2f, path.Distances[1]);
            Assert.AreEqual(3f, path.Distances[2]);
            Assert.AreEqual(4f, path.Distances[3]);
        }

        [Test]
        public void FindSegmentUnsafe_TestPassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var expectedSegment = Mathf.Max(0, Mathf.CeilToInt(passedPath) - 1);
            if (passedPath < 0 || expectedSegment > path.SegmentsCount())
                Assert.Throws(typeof(System.IndexOutOfRangeException), () => path.FindSegmentWithoutChecks(passedPath, 0));
            else Assert.AreEqual(expectedSegment, path.FindSegmentWithoutChecks(passedPath, 0));
        }

        [Test]
        public void FindSegmentUnsafe_TestStartIndexArgument([ValueSource(nameof(_startSegments))] int startIndex)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var passedPath = 1.5f;
            if (startIndex < 0 || startIndex >= path.Points.Length)
                Assert.Throws(typeof(System.IndexOutOfRangeException), () => path.FindSegmentWithoutChecks(passedPath, startIndex));
            else Assert.DoesNotThrow(() => path.FindSegmentWithoutChecks(passedPath, startIndex));
        }

        [Test]
        public void FindSegmentSafe_TestCyclicOption()
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(2, path.FindSegmentSafe(3.5f, false));
            Assert.AreEqual(3, path.FindSegmentSafe(3.5f, true));
        }

        [Test]
        public void FindSegmentSafe_TestPassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            var expectedSegment = Mathf.Clamp(Mathf.CeilToInt(passedPath) - 1, 0, path.SegmentsCount() - 1);
            Assert.AreEqual(expectedSegment, path.FindSegmentSafe(passedPath));
        }

        [Test]
        public void FindSegmentSafe_TestStartIndexArgument([ValueSource(nameof(_startSegments))] int startIndex)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Assert.AreEqual(1, path.FindSegmentSafe(1.5f, startIndex));
        }

        [Test]
        public void CalculatePosition_TestCyclicOption()
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            float passedPath = 3.4f;
            Assert.AreEqual(path.Points[3], path.CalculatePosition(passedPath, false));
            var expectedPosition = Vector3.Lerp(path.Points[3], path.Points[0], Mathf.Repeat(passedPath, 1));
            Assert.AreEqual(expectedPosition, path.CalculatePosition(passedPath, true));
        }

        [Test]
        public void CalculatePosition_PassedPathArgument([ValueSource(nameof(_passedLengths))] float passedPath)
        {
            var path = CreateSquarePath();
            path.RecalculateDistances();
            Vector3 expectedPosition;
            if (passedPath <= 0) expectedPosition = path.Points[0];
            else if (passedPath >= path.GetLength()) expectedPosition = path.Points[path.Points.Length - 1];
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
            Assert.AreEqual(path.FindSegmentSafe(passedPath), refSegment);
        }
    }
}