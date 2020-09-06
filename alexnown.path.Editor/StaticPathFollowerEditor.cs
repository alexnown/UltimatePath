using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    [CustomEditor(typeof(StaticPathFollowerAuthoring))]
    public class StaticPathFollowerEditor : Editor
    {
        private bool _usePassedRatio;
        [SerializeField, Range(0, 1)]
        private float _passedRatio = 0;
        private SerializedProperty _pathContainer;
        private SerializedProperty _passedDistance;

        public override void OnInspectorGUI()
        {
            var pathFollower = target as StaticPathFollowerAuthoring;
            serializedObject.Update();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_pathContainer);
            if (_pathContainer.objectReferenceValue == null)
            {
                _pathContainer.objectReferenceValue = pathFollower.GetComponent<PathComponent>();
                if (_pathContainer.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Required path container", MessageType.Error);
                    return;
                }
            }
            if (pathFollower.Path != null)
            {
                GUILayout.Space(10);
                GUI.enabled = !_usePassedRatio;
                EditorGUILayout.PropertyField(_passedDistance);
                GUI.enabled = true;
                EditorGUI.BeginChangeCheck();
                _usePassedRatio = EditorGUILayout.Toggle("Set by passed ratio", _usePassedRatio);
                if (EditorGUI.EndChangeCheck())
                {
                    _passedRatio = Mathf.InverseLerp(0, pathFollower.PathLength, pathFollower.DistancePassed);
                }
                if (_usePassedRatio)
                {
                    EditorGUI.BeginChangeCheck();
                    _passedRatio = EditorGUILayout.Slider("Passed ratio", _passedRatio, 0, 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _passedDistance.floatValue = Mathf.Lerp(0, pathFollower.PathLength, _passedRatio);
                    }
                }
            }
            else EditorGUILayout.HelpBox("Path not saved! Check target PathComponent.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _pathContainer = serializedObject.FindProperty(nameof(_pathContainer));
            _passedDistance = serializedObject.FindProperty(nameof(_passedDistance));
        }
    }
}