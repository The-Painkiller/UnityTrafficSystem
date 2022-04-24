using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class WaypointPathCreator : EditorWindow
{
    private PathsManager _pathsManager = null;

    private GameObject _currentSelectedGameObject = null;
    private Waypoint _currentSelectedWaypoint = null;
    private Waypoint[] _waypointsInCurrentScene = null;
    private string[] _waypointsInCurrentSceneNames = null;

    private List<int> _chosenWaypointsIndexes = new List<int>();

    private const float WINDOW_WIDTH = 500f;
    private const float WINDOW_HEIGHT = 300f;

    public const float FIELD_SIZE_SMALL = 50;
    public const float FIELD_SIZE_MEDIUM = 100f;
    public const float FIELD_SIZE_LARGE = 150f;
    public const float FIELD_SIZE_XLARGE = 200f;

    public const float SPACE_SIZE_SMALL = 10f;
    public const float SPACE_SIZE_MEDIUM = 20f;
    public const float SPACE_SIZE_LARGE = 30f;
    public const float SPACE_SIZE_XLARGE = 40f;

    [MenuItem("Traffic System/Waypoint Path Creator")]
    public static void Initialize()
    {
        WaypointPathCreator window = (WaypointPathCreator)GetWindow(typeof(WaypointPathCreator));
        window.position = new Rect(Screen.width/2f, Screen.height/2f, WINDOW_WIDTH, WINDOW_HEIGHT);
        window.Show();

        Waypoint.ToggleEditorOptions(true);
    }

    private void OnEnable()
    {
        _waypointsInCurrentScene = FindObjectsOfType<Waypoint>();
        _waypointsInCurrentScene = _waypointsInCurrentScene.OrderBy(x => x.name).ToArray();

        if (_waypointsInCurrentScene == null || _waypointsInCurrentScene.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No Waypoints found in the current scene. Please add at least 2 and restart the tool.", "Ok");
        }

        _pathsManager = FindObjectOfType<PathsManager>();
        if (_pathsManager == null)
        {
            EditorUtility.DisplayDialog("Error", "No Paths Manager found in the current scene. Please add one and restart the tool.", "Ok");
        }

        PopulateWaypointsDropDownContent();
        _chosenWaypointsIndexes.Clear();
    }

    private void Update()
    {
        _currentSelectedGameObject = Selection.activeGameObject;
        if (_currentSelectedGameObject == null)
            return;

        _currentSelectedWaypoint = _currentSelectedGameObject.GetComponent<Waypoint>();
    }

    private void OnGUI()
    {
        if (_chosenWaypointsIndexes == null)
            return;
              
        GUILayout.Space(SPACE_SIZE_SMALL);

        if (_chosenWaypointsIndexes.Count == 0)
        {
            AddSelection();
            if (AddWaypointButton())
            {
                _chosenWaypointsIndexes.Add(-1);
            }
            return;
        }

        for (int i = 0; i < _chosenWaypointsIndexes.Count; i++)
        {
            _chosenWaypointsIndexes[i] = EditorGUILayout.Popup(_chosenWaypointsIndexes[i], _waypointsInCurrentSceneNames, GUILayout.MaxWidth(FIELD_SIZE_LARGE));
        }

        EditorGUILayout.BeginHorizontal();

        if (AddWaypointButton())
        {
            _chosenWaypointsIndexes.Add(-1);
        }

        if (RemoveWaypointButton())
        {
            _chosenWaypointsIndexes.RemoveAt(_chosenWaypointsIndexes.Count - 1);
        }

        EditorGUILayout.EndHorizontal();

        AddSelection();

        GUILayout.Space(SPACE_SIZE_LARGE);

        if (GUILayout.Button("Create Path", GUILayout.MaxWidth(FIELD_SIZE_MEDIUM)))
        {
            Waypoint[] waypoints = new Waypoint[_chosenWaypointsIndexes.Count];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = _waypointsInCurrentScene[_chosenWaypointsIndexes[i]];
            }

            _pathsManager.CreatePathFromSelectedWaypoints(waypoints);
        }
    }

    private void AddSelection()
    {
        if (_currentSelectedWaypoint != null)
        {
            GUILayout.Space(SPACE_SIZE_SMALL);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected", _currentSelectedWaypoint.name);
            if (GUILayout.Button("Add Selection"))
            {
                _chosenWaypointsIndexes.Add(Array.IndexOf(_waypointsInCurrentScene, _currentSelectedWaypoint));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void OnDestroy()
    {
        Waypoint.ToggleEditorOptions(false);
    }

    private void PopulateWaypointsDropDownContent()
    {
        _waypointsInCurrentSceneNames = new string[_waypointsInCurrentScene.Length];

        for (int i = 0; i < _waypointsInCurrentSceneNames.Length; i++)
        {
            _waypointsInCurrentSceneNames[i] = _waypointsInCurrentScene[i].name;
        }
    }

    private bool AddWaypointButton()
    {
        return GUILayout.Button("+", GUILayout.MaxWidth(FIELD_SIZE_SMALL));
    }

    private bool RemoveWaypointButton()
    {
        return GUILayout.Button("-", GUILayout.MaxWidth(FIELD_SIZE_SMALL));
    }
}
