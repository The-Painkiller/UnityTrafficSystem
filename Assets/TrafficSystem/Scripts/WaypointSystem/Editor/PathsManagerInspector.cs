using UnityEditor;
namespace TrafficSystem
{
    /// <summary>
    /// A simple logic to display waypoint names within the Scene view everytime
    /// the PathsManager is selected.
    /// </summary>
    [CustomEditor(typeof(PathsManager))]
    public class PathsManagerInspector : Editor
    {
        private void OnEnable()
        {
            Waypoint.ToggleEditorOptions(true);
        }

        private void OnDisable()
        {
            Waypoint.ToggleEditorOptions(false);
        }
    }
}