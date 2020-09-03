using UnityEngine;

namespace alexnown.path
{
    public interface IPath
    {
        Vector3[] Points { get; set; }
        float TotalLength { get; }
        int SegmentsCount { get; }
        int FindSegment(float passedPath);
        int FindSegment(float passedPath, int lastSegmentIndex);
        Vector3 CalculatePosition(float passedPath);
        Vector3 CalculatePosition(float passedPath, ref int lastSegmentIndex);
    }
}