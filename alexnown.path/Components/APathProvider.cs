using UnityEngine;

namespace alexnown.path
{
    public abstract class APathProvider : MonoBehaviour
    {
        public abstract Vector3 GetPointPosition(int index);
        public abstract void SetPointPosition(int pointIndex, Vector3 worldPos);
        public abstract Vector3 GetPositionBetweenPoints(int first, int second, float ratio = 0.5f);

        public abstract void CachePath();
    }
}