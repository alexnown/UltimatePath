using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    [CustomEditor(typeof(WaypointsPathProvider))]
    public class WaypointsPathCreatorEditor : Editor
    {
        private int _pointIndexForPositionHandle = -1;
        private SerializedProperty _serializedPointsArray;
        private SerializedProperty _serializedIsCyclic;
        private GUIStyle _textStyle;
        private Matrix4x4 _ltwMatrix;

        public override void OnInspectorGUI()
        {
            var pointPath = target as WaypointsPathProvider;
            var newLtw = pointPath.transform.localToWorldMatrix;
            bool requireUpdatePath = newLtw != _ltwMatrix;
            _ltwMatrix = newLtw;
            serializedObject.Update();
            GUI.enabled = !Application.isPlaying;
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_serializedIsCyclic);
            EditorGUILayout.PropertyField(_serializedPointsArray, true);
            requireUpdatePath |= EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            if (requireUpdatePath && !Application.isPlaying)
            {
                pointPath.CachePath();
                EditorUtility.SetDirty(pointPath);
            }
        }

        private void ConvertPointsArray(bool toLocalSpace)
        {
            var pointPath = target as WaypointsPathProvider;
            var transform = pointPath.transform;
            for (int i = 0; i < _serializedPointsArray.arraySize; i++)
            {
                var pointProp = _serializedPointsArray.GetArrayElementAtIndex(i);
                pointProp.vector3Value = toLocalSpace
                    ? transform.InverseTransformPoint(pointProp.vector3Value)
                    : transform.TransformPoint(pointProp.vector3Value);
            }
        }

        private void OnEnable()
        {
            var pointPath = target as WaypointsPathProvider;
            _ltwMatrix = pointPath.transform.localToWorldMatrix;
            if (!Application.isPlaying) pointPath.CachePath();
            _textStyle = new GUIStyle();
            _textStyle.normal.textColor = Color.blue;
            _textStyle.fontSize = 20;
            _serializedPointsArray = serializedObject.FindProperty("Points");
            _serializedIsCyclic = serializedObject.FindProperty("_isCyclic");
        }

        private bool DrawClickableNode(Vector3 worldPos, float handlerSize, Color color)
        {
            Handles.color = color;
            return Handles.Button(worldPos, Quaternion.identity, handlerSize, handlerSize, Handles.SphereHandleCap);
        }

        private void OnSceneGUI()
        {
            if (Application.isPlaying) return;
            serializedObject.Update();
            var pointPath = target as WaypointsPathProvider;
            var drawer = pointPath.GetComponent<GizmosPathDrawer>();
            var nodeColor = drawer != null ? drawer.Color : Color.cyan;
            var currentEvent = Event.current;
            bool addPointMode = currentEvent.shift;
            bool removePointMode = currentEvent.control;
            var canDragPoint = !addPointMode && !removePointMode;
            if (!canDragPoint) _pointIndexForPositionHandle = -1;

            bool pathChanged = false;
            for (int i = 0; i < _serializedPointsArray.arraySize; i++)
            {
                bool firstOrLast = i == 0 || i == _serializedPointsArray.arraySize - 1;
                var pointWorldPosition = pointPath.GetPointPosition(i);
                var handlerSize = HandleUtility.GetHandleSize(pointWorldPosition);

                if (currentEvent.control)
                {
                    bool deleted = DrawClickableNode(pointWorldPosition, 0.2f * handlerSize, Color.red);
                    if (deleted)
                    {
                        pathChanged = true;
                        _serializedPointsArray.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        i--;
                        continue;
                    }
                }
                else if (currentEvent.shift)
                {
                    DrawClickableNode(pointWorldPosition, 0.1f * handlerSize, Color.gray);
                }
                else if (i != _pointIndexForPositionHandle)
                {
                    if (DrawClickableNode(pointWorldPosition, 0.2f * handlerSize, nodeColor))
                        _pointIndexForPositionHandle = i;
                }

                if (firstOrLast || i == _pointIndexForPositionHandle)
                    Handles.Label(pointWorldPosition + new Vector3(0, 0.5f * handlerSize, 0), i.ToString(), _textStyle);
                if (i == _pointIndexForPositionHandle && HandlePointDragging(ref pointWorldPosition))
                {
                    pathChanged = true;
                    pointPath.SetPointPosition(i, pointWorldPosition);
                }

                bool connectedWithNextPoint = i < _serializedPointsArray.arraySize - 1 || _serializedIsCyclic.boolValue;
                if (connectedWithNextPoint)
                {
                    int nextPointIndex = (i + 1) % _serializedPointsArray.arraySize;
                    var nextNodeWorldPos = pointPath.GetPointPosition(nextPointIndex);
                    Handles.color = canDragPoint ? nodeColor : Color.gray;
                    Handles.DrawLine(pointWorldPosition, nextNodeWorldPos);

                    if (addPointMode)
                    {
                        var newPointPosition = (pointWorldPosition + nextNodeWorldPos) / 2;
                        if (DrawClickableNode(newPointPosition, 0.2f * HandleUtility.GetHandleSize(newPointPosition), Color.green))
                        {
                            pathChanged = true;
                            _serializedPointsArray.InsertArrayElementAtIndex(i + 1);
                            serializedObject.ApplyModifiedProperties();
                            pointPath.SetPointPosition(i + 1, newPointPosition);
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
            if (pathChanged)
            {
                pointPath.CachePath();
                EditorUtility.SetDirty(pointPath);
            }
        }
        private bool HandlePointDragging(ref Vector3 pointWorldPosition)
        {
            EditorGUI.BeginChangeCheck();
            pointWorldPosition = Handles.DoPositionHandle(pointWorldPosition, Quaternion.identity);
            return EditorGUI.EndChangeCheck();
        }
    }
}