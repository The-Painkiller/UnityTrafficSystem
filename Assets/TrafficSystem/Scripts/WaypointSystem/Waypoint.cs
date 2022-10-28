using UnityEngine;
using UnityEditor;

namespace TrafficSystem
{
    /// <summary>
    /// A single point of location on the map.
    /// A group of waypoints make a path on which a vehicle travels.
    /// </summary>
    public class Waypoint : MonoBehaviour
    {
        private Transform _transform = null;

        public Vector3 Position => _transform.position;

        private void Awake()
        {
            _transform = transform;
        }

#if UNITY_EDITOR

        [SerializeField]
        private Color _gizmoColor = Color.yellow;

        private static bool _displayObjectName = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, 1f);

            if (_displayObjectName)
                Handles.Label(transform.position, gameObject.name, EditorStyles.boldLabel);
        }

        public static void ToggleEditorOptions(bool toggle)
        {
            _displayObjectName = toggle;
        }

#endif

    }
}