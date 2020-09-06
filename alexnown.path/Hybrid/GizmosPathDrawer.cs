using UnityEngine;

namespace alexnown.path
{
    public interface IDrawablePath
    {
        Vector3[] Waypoints { get; }
    }

    [RequireComponent(typeof(PathComponent))]
    public class GizmosPathDrawer : MonoBehaviour
    {
        public bool ShowAlways = true;
        public float NodeSize = 0.1f;
        public bool AdjustableSize = true;
        public Color Color = Color.cyan;
        private IDrawablePath _pathContainer;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!ShowAlways) return;
            if (!Application.isPlaying && UnityEditor.Selection.activeGameObject == gameObject) return;
            if (_pathContainer == null) _pathContainer = GetComponent<IDrawablePath>();
            var points = _pathContainer?.Waypoints;
            if (points == null) return;
            Gizmos.color = Color;
            for (int i = 0; i < points.Length; i++)
            {
                if (i < points.Length - 1) Gizmos.DrawLine(points[i], points[i + 1]);
                var handlerSize = AdjustableSize ? UnityEditor.HandleUtility.GetHandleSize(points[i]) : 1;
                Gizmos.DrawSphere(points[i], NodeSize * handlerSize);
            }
        }
#endif
    }
}