using UnityEngine;

namespace alexnown.path
{
    public enum Axis
    {
        None,
        X,
        Y,
        Z
    }
    public abstract class APathProvider : MonoBehaviour
    {
        public Axis LockedAxis;
        public abstract Vector3 GetPointPosition(int index);
        public abstract void SetPointPosition(int pointIndex, Vector3 worldPos);
        public abstract Vector3 GetPositionBetweenPoints(int first, int second, float ratio);

        public abstract void CachePath();
        public abstract void LockAxis(Axis axis);
        protected Vector3 LockPositionAxis(Vector3 pos, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    pos.x = 0;
                    break;
                case Axis.Y:
                    pos.y = 0;
                    break;
                case Axis.Z:
                    pos.z = 0;
                    break;
            }
            return pos;
        }
    }
}