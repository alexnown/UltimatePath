using UnityEditor;
using UnityEngine;

namespace alexnown.path
{
    [CustomEditor(typeof(PathComponent))]
    public class PathComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var component = target as Component;
            var path = (target as PathComponent).Path;
            GUILayout.Space(10);
            if (path != null)
            {
                EditorGUILayout.LabelField(path.GetType().Name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Segments: {path.SegmentsCount}");
                EditorGUILayout.LabelField($"Length {path.TotalLength}");
            }
            else EditorGUILayout.HelpBox("Path not saved, use one of PathProvider!", MessageType.Error);

            GUILayout.Space(10);
            var drawer = component.GetComponent<GizmosPathDrawer>();
            var hasPathDrawer = drawer != null;
            if (GUILayout.Button(hasPathDrawer ? "Remove gizmos drawer" : "Add gizmos drawer"))
            {
                if (hasPathDrawer) GameObject.DestroyImmediate(drawer);
                else component.gameObject.AddComponent<GizmosPathDrawer>();
            }
        }
    }
}