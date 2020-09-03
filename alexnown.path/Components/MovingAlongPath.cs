using Unity.Entities;

namespace alexnown.path
{
    [GenerateAuthoringComponent]
    public struct MovingAlongPath : IComponentData
    {
        public float MoveSpeed;
        public bool YoyoMoving;
        //private void Update()
        //{
        //    if (MoveSpeed == 0) return;
        //    var dt = Time.deltaTime;
        //    var totalPathLength = _pathWalker.PathLength;
        //    float distance = _pathWalker.DistancePassed;
        //    if (_pathWalker.IsPathCyclic) distance = DistanceCalculator.CycleMoving(distance, MoveSpeed, dt, totalPathLength);
        //    else if (YoyoMoving) distance = DistanceCalculator.YoyoMoving(distance, ref MoveSpeed, dt, totalPathLength);
        //    else distance = DistanceCalculator.Moving(distance, MoveSpeed, dt, totalPathLength);
        //    if (distance != _pathWalker.DistancePassed) _pathWalker.SetDistancePassed(distance);
        //}
    }
}