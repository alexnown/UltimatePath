using UnityEngine;

namespace alexnown.path
{
    [System.Serializable]
    public class UniformPath : IPath
    {
        [SerializeField]
        private Vector3[] _points;
        [SerializeField]
        private float _totalLength;
        [SerializeField]
        private float _segmentLength;
        public Vector3[] Points { get => _points; set => _points = value; }
        public float TotalLength
        {
            get => _totalLength;
            set => _totalLength = value;
        }
        public float SegmentLength
        {
            get => _segmentLength;
            set => _segmentLength = value;
        }
        public int SegmentsCount => Points.Length - 1;

        public Vector3 CalculatePosition(float passedPath)
        {
            int i = 0;
            return CalculatePosition(passedPath, ref i);
        }

        public Vector3 CalculatePosition(float passedPath, ref int lastSegmentIndex)
        {
            if (passedPath <= 0)
            {
                lastSegmentIndex = 0;
                return Points[lastSegmentIndex];
            }
            else if (passedPath >= TotalLength)
            {
                lastSegmentIndex = SegmentsCount - 1;
                return Points[lastSegmentIndex + 1];
            }
            var ratio = passedPath / SegmentLength;
            var index = (int)ratio;
            return Vector3.Lerp(Points[index], Points[index + 1], ratio - index);
        }

        public int FindSegment(float passedPath)
        {
            bool isLastPoint = passedPath >= TotalLength;
            return isLastPoint ? SegmentsCount - 1 : (int)(passedPath / SegmentLength);
        }

        public int FindSegment(float passedPath, int lastSegmentIndex) => FindSegment(passedPath, 0);
    }
}