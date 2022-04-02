using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private Transform _transform = null;

    public Vector3 Position => _transform.position;

#if UNITY_EDITOR
    [SerializeField]
    private Color _GizmoColor = Color.yellow;
#endif

    private void Awake()
    {
        _transform = transform;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = _GizmoColor;
        Gizmos.DrawSphere(transform.position, 1f);
    }

#endif

}
