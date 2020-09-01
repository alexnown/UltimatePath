using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    public abstract class BasePathEditor<T> : Editor where T : APathProvider
    {
        private GUIStyle _textStyle;
        private Matrix4x4 _ltwMatrix;
        protected SerializedProperty _serializedPointsArray;
        protected SerializedProperty _serializedIsCyclic;
        protected T _pathProvider;

        protected abstract void DrawNodesConnection(int first, int second, Color color);
        public override void OnInspectorGUI()
        {
            var newLtw = (target as Component).transform.localToWorldMatrix;
            bool requireUpdatePath = newLtw != _ltwMatrix;
            _ltwMatrix = newLtw;
            serializedObject.Update();
            GUI.enabled = !Application.isPlaying;
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(10);
            DrawInspectorGui();
            requireUpdatePath |= EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();
            if (requireUpdatePath && !Application.isPlaying) CachePath();
        }
        protected virtual void DrawInspectorGui()
        {
            EditorGUILayout.PropertyField(_serializedIsCyclic);
            EditorGUILayout.PropertyField(_serializedPointsArray, true);
        }
        private void OnEnable()
        {
            _pathProvider = target as T;
            _ltwMatrix = _pathProvider.transform.localToWorldMatrix;
            if (!Application.isPlaying) CachePath();
            _textStyle = new GUIStyle();
            _textStyle.normal.textColor = Color.blue;
            _textStyle.fontSize = 20;
            _serializedPointsArray = serializedObject.FindProperty("Points");
            _serializedIsCyclic = serializedObject.FindProperty("_isCyclic");
        }

        protected void CachePath()
        {
            _pathProvider.CachePath();
            EditorUtility.SetDirty(_pathProvider);
        }

        protected virtual void OnSceneGUI()
        {
            if (Application.isPlaying) return;
            serializedObject.Update();
            bool pathChanged = false;
            var drawer = _pathProvider.GetComponent<GizmosPathDrawer>();
            var nodeColor = drawer != null ? drawer.Color : Color.cyan;
            var connectionColor = Event.current.control || Event.current.shift || Event.current.alt ? Color.gray : nodeColor;
            for (int i = 0; i < _serializedPointsArray.arraySize; i++)
            {
                pathChanged |= DrawPathNode(i, nodeColor);
            }
            for (int i = 1; i < _serializedPointsArray.arraySize; i++)
            {
                DrawNodesConnection(i - 1, i, connectionColor);
            }
            if (_serializedIsCyclic.boolValue) DrawNodesConnection(_serializedPointsArray.arraySize - 1, 0, connectionColor);
            if (Event.current.shift) pathChanged |= DrawAddNodeButtons();
            if (pathChanged)
            {
                serializedObject.ApplyModifiedProperties();
                CachePath();
            }
        }

        protected virtual bool DrawAddNodeButtons()
        {
            var added = false;
            var nodesCount = _serializedPointsArray.arraySize;
            for (int i = 1; i < nodesCount; i++)
            {
                var pos = _pathProvider.GetPositionBetweenPoints(i - 1, i, 0.5f);
                added |= DrawAddNodeButton(i, pos);
            }
            if (_serializedIsCyclic.boolValue)
            {
                var pos = _pathProvider.GetPositionBetweenPoints(nodesCount - 1, 0, 0.5f);
                added |= DrawAddNodeButton(_serializedPointsArray.arraySize, pos);
            }
            else
            {
                var firstPointPos = _pathProvider.GetPointPosition(0);
                var atStartNodePosition = 2 * firstPointPos - _pathProvider.GetPositionBetweenPoints(0, 1, 0.33f);
                added |= DrawAddNodeButton(0, atStartNodePosition);

                var lastNodePos = _pathProvider.GetPointPosition(_serializedPointsArray.arraySize - 1);
                var atEndNodePosition = 2 * lastNodePos - _pathProvider.GetPositionBetweenPoints(nodesCount - 2, nodesCount - 1, 0.66f);
                added |= DrawAddNodeButton(nodesCount, atEndNodePosition);
            }
            return added;
        }

        private bool DrawAddNodeButton(int index, Vector3 position)
        {
            var clicked = DrawClickableNode(position, 0.2f * HandleUtility.GetHandleSize(position), Color.green);
            if (clicked)
            {
                _serializedPointsArray.InsertArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                InitializeAddedPoint(index, position);
            }
            return clicked;
        }

        protected virtual void InitializeAddedPoint(int index, Vector3 position)
        {
            _pathProvider.SetPointPosition(index, position);
        }

        protected virtual void OnPointMoved(int index, Vector3 worldPos)
        {
            _pathProvider.SetPointPosition(index, worldPos);
        }

        protected virtual bool DrawPathNode(int index, Color color)
        {
            var pointWorldPosition = _pathProvider.GetPointPosition(index);
            var handlerSize = HandleUtility.GetHandleSize(pointWorldPosition);

            Handles.Label(pointWorldPosition + new Vector3(0, 0.5f * handlerSize, 0), index.ToString(), _textStyle);
            bool addNodeMode = Event.current.shift;
            if (Event.current.alt || addNodeMode) DrawClickableNode(pointWorldPosition, 0.1f * handlerSize, Color.gray);
            if (Event.current.control)
            {
                bool deleted = DrawClickableNode(pointWorldPosition, 0.2f * handlerSize, Color.red) && _serializedPointsArray.arraySize > 2;
                if (deleted) _serializedPointsArray.DeleteArrayElementAtIndex(index);
                return deleted;
            }
            else if (!addNodeMode)
            {
                Handles.color = color;
                EditorGUI.BeginChangeCheck();
                pointWorldPosition = Event.current.alt
                    ? Handles.DoPositionHandle(pointWorldPosition, Quaternion.identity)
                    : Handles.FreeMoveHandle(pointWorldPosition, Quaternion.identity, 0.2f * handlerSize, 0.2f * Vector3.one, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    OnPointMoved(index, pointWorldPosition);
                    return true;
                }
            }
            return false;
        }

        protected bool DrawClickableNode(Vector3 worldPos, float handlerSize, Color color)
        {
            Handles.color = color;
            return Handles.Button(worldPos, Quaternion.identity, handlerSize, handlerSize, Handles.SphereHandleCap);
        }

    }
}