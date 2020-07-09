using NUnit.Framework;
using UnityEngine;

namespace alexnown.path
{
    public class DistanceCalculatorTests
    {
        private static float[] _speeds = new float[] { float.MinValue, -10000, -1, 0, 1, 10000, float.MaxValue };
        private float DT = 1f;
        [Test]
        public void CheckDistanceLimits([ValueSource(nameof(_speeds))] float speed)
        {
            float totalPath = Mathf.PI;
            var cycleResult = DistanceCalculator.CycleMoving(0, speed, DT, totalPath);
            Assert.GreaterOrEqual(cycleResult, 0);
            Assert.LessOrEqual(cycleResult, totalPath);
        }

        [Test]
        public void MovingTest()
        {
            float totalPath = 10;
            Assert.AreEqual(6, DistanceCalculator.Moving(5, 1, DT, totalPath));
            Assert.AreEqual(4, DistanceCalculator.Moving(5, -1, DT, totalPath));
            Assert.AreEqual(0, DistanceCalculator.Moving(0, -1, DT, totalPath));
            Assert.AreEqual(totalPath, DistanceCalculator.Moving(totalPath, 1, DT, totalPath));
        }
        [Test]
        public void YoyoMovingTest()
        {
            float totalPath = 10;
            float speed = 1;
            Assert.AreEqual(6, DistanceCalculator.YoyoMoving(5, ref speed, DT, totalPath));
            Assert.AreEqual(1, speed);

            speed = -1;
            Assert.AreEqual(4, DistanceCalculator.YoyoMoving(5, ref speed, DT, totalPath));
            Assert.AreEqual(-1, speed);

            speed = 1;
            Assert.AreEqual(totalPath - 1, DistanceCalculator.YoyoMoving(totalPath, ref speed, DT, totalPath));
            Assert.AreEqual(-1, speed);

            speed = -1;
            Assert.AreEqual(1, DistanceCalculator.YoyoMoving(0, ref speed, DT, totalPath));
            Assert.AreEqual(1, speed);
        }

        [Test]
        public void CycleMovingTest()
        {
            float totalPath = 10;
            Assert.AreEqual(6, DistanceCalculator.CycleMoving(5, 1, DT, totalPath));
            Assert.AreEqual(4, DistanceCalculator.CycleMoving(5, -1, DT, totalPath));
            Assert.AreEqual(0, DistanceCalculator.CycleMoving(0, totalPath, DT, totalPath));
            Assert.AreEqual(0, DistanceCalculator.CycleMoving(0, -totalPath, DT, totalPath));
            Assert.AreEqual(totalPath - 1, DistanceCalculator.CycleMoving(0, -1, DT, totalPath));
        }
    }
}