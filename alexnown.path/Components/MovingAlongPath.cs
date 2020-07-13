using UnityEngine;

namespace alexnown.path
{
    public enum MovingType : byte
    {
        Clamp,
        Yoyo,
        Incremental
    }

    [RequireComponent(typeof(StaticPathFollower))]
    public class MovingAlongPath : MonoBehaviour
    {
        public float MoveSpeed = 1;
        public MovingType Type;

        private StaticPathFollower _pathWalker;

        private void Start()
        {
            _pathWalker = GetComponent<StaticPathFollower>();
        }

        private void Update()
        {
            if (MoveSpeed == 0) return;
            var dt = Time.deltaTime;
            var totalPathLength = _pathWalker.PathLength;
            float distance = _pathWalker.DistancePassed;
            switch (Type)
            {
                case MovingType.Incremental:
                    distance = DistanceCalculator.CycleMoving(distance, MoveSpeed, dt, totalPathLength);
                    break;
                case MovingType.Yoyo:
                    distance = DistanceCalculator.YoyoMoving(distance, ref MoveSpeed, dt, totalPathLength);
                    break;
                default:
                    distance = DistanceCalculator.Moving(distance, MoveSpeed, dt, totalPathLength);
                    break;
            }
            if (distance != _pathWalker.DistancePassed) _pathWalker.SetDistancePassed(distance);
        }
    }
}