using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    [CustomEditor(typeof(StaticPathFollower))]
    public class StaticPathFollowerEditor : Editor
    {
        private bool _usePassedRatio;
        [SerializeField, Range(0, 1)]
        private float _passedRatio = 0;
        private SerializedProperty _pathContainer;
        private SerializedProperty _passedDistance;
        private SerializedProperty OverrideIsCycleValue;
        private SerializedProperty RelevantIsCycle;

        public override void OnInspectorGUI()
        {
            var pathFollower = target as StaticPathFollower;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_pathContainer);
            if (EditorGUI.EndChangeCheck()) pathFollower.Path = null;
            bool hasPathContainer;
            if (_pathContainer.objectReferenceValue != null)
            {
                hasPathContainer = (_pathContainer.objectReferenceValue as GameObject).GetComponent<IStaticPathContainer>() != null;
                if (!hasPathContainer) EditorGUILayout.HelpBox("Target does not have path container component!", MessageType.Error);
            }
            else
            {
                hasPathContainer = pathFollower.GetComponent<IStaticPathContainer>() != null;
                if (hasPathContainer) _pathContainer.objectReferenceValue = pathFollower.gameObject;
                else EditorGUILayout.HelpBox("Required path container", MessageType.Error);
            }
            if (hasPathContainer)
            {
                GUI.enabled = !_usePassedRatio;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_passedDistance);
                bool changedPassedDistance = EditorGUI.EndChangeCheck();
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
                        changedPassedDistance = true;
                    }
                }
                if (changedPassedDistance)
                {
                    pathFollower.SetDistancePassed(_passedDistance.floatValue);
                    EditorUtility.SetDirty(pathFollower.transform);
                }
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(OverrideIsCycleValue);
                if (OverrideIsCycleValue.boolValue)
                    EditorGUILayout.PropertyField(RelevantIsCycle);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            _pathContainer = serializedObject.FindProperty(nameof(_pathContainer));
            _passedDistance = serializedObject.FindProperty(nameof(_passedDistance));
            OverrideIsCycleValue = serializedObject.FindProperty(nameof(OverrideIsCycleValue));
            RelevantIsCycle = serializedObject.FindProperty(nameof(RelevantIsCycle));
        }
    }
}