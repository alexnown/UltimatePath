using UnityEngine;

namespace alexnown.path
{
    public interface IDrawablePath
    {
        Vector3[] Waypoints { get; }
        bool IsCyclic { get; }
    }
}