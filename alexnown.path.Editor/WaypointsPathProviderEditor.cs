using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    [CustomEditor(typeof(WaypointsPathProvider))]
    public class WaypointsPathProviderEditor : BasePathEditor<APathProvider>
    {
        protected override void DrawNodesConnection(int first, int second, Color color)
        {
            var firstPosition = _pathProvider.GetPointPosition(first);
            var secondPosition = _pathProvider.GetPointPosition(second);
            Handles.color = color;
            Handles.DrawLine(firstPosition, secondPosition);
        }
    }
}