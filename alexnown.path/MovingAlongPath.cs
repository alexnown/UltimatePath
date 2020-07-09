using UnityEngine;

namespace alexnown.path
{
    [RequireComponent(typeof(StaticPathFollower))]
    public class MovingAlongPath : MonoBehaviour
    {
        public float MoveSpeed = 1;
        public bool YoyoMoving;

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
            if (_pathWalker.IsPathCyclic) distance = DistanceCalculator.CycleMoving(distance, MoveSpeed, dt, totalPathLength);
            else if (YoyoMoving) distance = DistanceCalculator.YoyoMoving(distance, ref MoveSpeed, dt, totalPathLength);
            else distance = DistanceCalculator.Moving(distance, MoveSpeed, dt, totalPathLength);
            if (distance != _pathWalker.DistancePassed) _pathWalker.SetDistancePassed(distance);
        }
    }
}