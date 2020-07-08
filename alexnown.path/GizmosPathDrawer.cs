using UnityEngine;
namespace alexnown.path
{
    public class PathGizmosDrawer : MonoBehaviour
    {
        public float NodeSize = 0.1f;
        public bool AdjustableSize = true;
        public Color Color = Color.cyan;
        private IDrawablePath _pathContainer;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_pathContainer == null) _pathContainer = GetComponent<IDrawablePath>();
            var points = _pathContainer?.Waypoints;
            if (points == null || UnityEditor.Selection.activeGameObject == gameObject) return;
            Gizmos.color = Color;
            bool cyclicPath = _pathContainer.IsCyclic;
            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                if (i < points.Length - 1 || cyclicPath)
                {
                    Gizmos.DrawLine(points[i], points[nextIndex]);
                }
                var handlerSize = AdjustableSize ? UnityEditor.HandleUtility.GetHandleSize(points[i]) : 1;
                Gizmos.DrawSphere(points[i], NodeSize * handlerSize);
            }
        }
#endif
    }
}