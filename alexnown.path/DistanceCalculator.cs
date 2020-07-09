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
    }
}