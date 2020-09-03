using UnityEditor;
using UnityEngine;
using static alexnown.path.BezierPathProvider;

namespace alexnown.path
{
    [CustomEditor(typeof(BezierPathProvider))]
    public class BezierPathProviderEditor : BasePathEditor<BezierPathProvider>
    {
        protected override void DrawInspectorGui()
        {
            base.DrawInspectorGui();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SegmentsPerUnit"));
        }
        protected override void DrawNodesConnection(int first, int second, Color color)
        {
            var pos1 = _pathProvider.GetPointPositionAndHandlers(first, out var firstHandler1, out var firstHandler2);
            var pos2 = _pathProvider.GetPointPositionAndHandlers(second, out var secondHandler1, out var secondHandler2);
            Handles.color = color;
            Handles.DrawBezier(pos1, pos2, firstHandler2, secondHandler1, color, null, 1);
        }

        protected override void InitializeAddedPoint(int index, Vector3 position)
        {
            base.InitializeAddedPoint(index, position);
            var pointData = _pathProvider.Points[index];
            pointData.Type = BezierPoint.HandleType.Solid;
            var nearHandlePosition = index == 0 ? _pathProvider.Points[1].Handle1 : _pathProvider.Points[index - 1].Handle2;
            if (index == 0)
            {
                pointData.Handle2 = (_pathProvider.Points[1].Position + nearHandlePosition - pointData.Position) / 2;
                pointData.Handle1 = -pointData.Handle2;
            }
            else
            {
                pointData.Handle1 = (_pathProvider.Points[index - 1].Position + nearHandlePosition - pointData.Position) / 2;
                pointData.Handle2 = -pointData.Handle1;
            }
        }

        protected override bool DrawPathNode(int index, Color color)
        {
            var changed = base.DrawPathNode(index, color);
            var anyModePressed = Event.current.control || Event.current.shift || Event.current.alt;
            if (anyModePressed) return changed;
            var pointData = _pathProvider.Points[index];
            if (pointData.Type == BezierPoint.HandleType.None) return changed;
            Handles.color = Color.yellow;
            var pointPos = _pathProvider.GetPointPositionAndHandlers(index, out var handlerPos1, out var handlerPos2);
            var handlerSize1 = 0.1f * HandleUtility.GetHandleSize(handlerPos1);
            var handlerSize2 = 0.1f * HandleUtility.GetHandleSize(handlerPos2);

            EditorGUI.BeginChangeCheck();
            handlerPos1 = Handles.FreeMoveHandle(handlerPos1, Quaternion.identity, handlerSize1, Vector3.zero, Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                pointData.Handle1 = _pathProvider.transform.InverseTransformPoint(handlerPos1) - pointData.Position;
                if (pointData.Type == BezierPoint.HandleType.Solid)
                    pointData.Handle2 = -pointData.Handle1;
            }

            EditorGUI.BeginChangeCheck();
            handlerPos2 = Handles.FreeMoveHandle(handlerPos2, Quaternion.identity, handlerSize2, Vector3.zero, Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
                pointData.Handle2 = _pathProvider.transform.InverseTransformPoint(handlerPos2) - pointData.Position;
                if (pointData.Type == BezierPoint.HandleType.Solid)
                    pointData.Handle1 = -pointData.Handle2;
            }

            Handles.DrawLine(pointPos, handlerPos1);
            Handles.DrawLine(pointPos, handlerPos2);
            return changed;
        }
    }
}