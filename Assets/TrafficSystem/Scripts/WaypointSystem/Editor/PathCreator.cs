using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Path creation tool using waypoints.
    /// This tool lets you put together a sequence of waypoints
    /// and assign it as a path to the PathManager's list of paths.
    /// </summary>
    public class PathCreator : EditorWindow
    {
        private PathsManager _pathsManager = null;
        private Waypoint[] _waypointsInCurrentScene = null;
        private List<Waypoint> _chosenWaypoints = new List<Waypoint>();

        private ReorderableList _waypointsReorderable = null;

        private Vector2 _waypointsScrollPosition = Vector2.zero;

        private const float WINDOW_WIDTH = 500f;
        private const float WINDOW_HEIGHT = 500;

        [MenuItem("Traffic System/Path Creator")]
        public static void Init()
        {
            PathCreator window = GetWindow<PathCreator>();
            window.Show();
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, WINDOW_WIDTH, WINDOW_HEIGHT);
        }

        private void OnEnable()
        {
            _waypointsInCurrentScene = FindObjectsOfType<Waypoint>();
            if (_waypointsInCurrentScene == null || _waypointsInCurrentScene.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No Waypoints found in the current scene. Please add at least 2 and restart the tool.", "Ok");
            }

            _pathsManager = FindObjectOfType<PathsManager>();
            if (_pathsManager == null)
            {
                EditorUtility.DisplayDialog("Error", "No Paths Manager found in the current scene. Please add one and restart the tool.", "Ok");
            }

            _waypointsReorderable = new ReorderableList(_chosenWaypoints, typeof(Waypoint), true, true, true, true);

            _waypointsReorderable.drawElementCallback = DrawReorderableItems;
            _waypointsReorderable.drawHeaderCallback = DrawReorderableHeader;
        }

        private void DrawReorderableHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Waypoints Path");
        }

        private void DrawReorderableItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            _chosenWaypoints[index] = EditorGUI.ObjectField(rect, _chosenWaypoints[index], typeof(Waypoint), true) as Waypoint;
        }

        private void OnGUI()
        {
            if (_chosenWaypoints == null || _waypointsInCurrentScene == null || _waypointsInCurrentScene.Length == 0)
                return;
            GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
            if (_chosenWaypoints.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Start adding waypoints sequence.", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM));
                if (GUILayout.Button("+", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_SMALL)))
                {
                    _chosenWaypoints.Add(null);
                }

                EditorGUILayout.EndHorizontal();
                return;
            }

            if (GUILayout.Button("New", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_SMALL)))
            {
                _chosenWaypoints.Clear();
                _chosenWaypoints.Add(null);
            }

            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

            EditorGUILayout.LabelField("Add the sequence of waypoints to create a path.", EditorStyles.boldLabel);

            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);

            _waypointsScrollPosition = GUILayout.BeginScrollView(_waypointsScrollPosition);
            _waypointsReorderable.DoLayoutList();

            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);

            if (GUILayout.Button("CreatePath", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM)))
            {
                _pathsManager.CreatePathFromSelectedWaypoints(_chosenWaypoints.ToArray());

                EditorUtility.DisplayDialog("Done", "Path added to PathsManager in the main scene", "OK");

                EditorUtility.SetDirty(_pathsManager);
            }

            GUILayout.EndScrollView();
        }
    }
}