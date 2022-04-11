using UnityEngine;
using UnityEditor;
public class Waypoint : MonoBehaviour
{
    private Transform _transform = null;

    public Vector3 Position => _transform.position;

#if UNITY_EDITOR
    [SerializeField]
    private Color _gizmoColor = Color.yellow;

    private static bool _displayObjectName = false;
#endif

    private void Awake()
    {
        _transform = transform;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(transform.position, 1f);

        if(_displayObjectName)
            UnityEditor.Handles.Label(transform.position, gameObject.name, UnityEditor.EditorStyles.boldLabel);
    }

    public static void ToggleEditorOptions(bool toggle)
    {
        _displayObjectName = toggle;
    }

#endif

}
