using UnityEditor;
using UnityEngine;

public class TrafficSignalCreator : EditorWindow
{
    private const float WINDOW_WIDTH = 600f;
    private const float WINDOW_HEIGHT = 600f;

    private static SignalManager _currentSignalManager = null;

    private GUIStyle _verticalLine = new GUIStyle();
    private Vector2 _globalScrollPosition = Vector2.zero;

    private SerializedObject _serializedSignalManagerObject = null;
    private SerializedObject _serializedSignalCreator = null;
    private SerializedProperty _serializedSignalObjects = null;
    private SerializedProperty _serializedTimeBoxes = null;
    private SerializedProperty _serializedTimeBoxSignalsArray = null;
    private SerializedProperty _serializedCurrentDirectionsArray = null;

    private int _numberOfTimeBoxes = 0;


    [MenuItem("Traffic System/Traffic Signal Creator")]
    public static void Initialize()
    {
        TrafficSignalCreator window = (TrafficSignalCreator)GetWindow(typeof(TrafficSignalCreator));

        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, WINDOW_WIDTH, WINDOW_HEIGHT);
        window.Show();
    }

    private void OnEnable()
    {
        _verticalLine.normal.background = EditorGUIUtility.whiteTexture;
        _verticalLine.margin = new RectOffset(0, 0, 4, 4);
        _verticalLine.fixedWidth = 1;

        _serializedSignalCreator = new SerializedObject(this);

        _numberOfTimeBoxes = -1;
    }

    private void OnGUI()
    {
        _currentSignalManager = EditorGUILayout.ObjectField("Drag Signal Manager", _currentSignalManager, typeof(SignalManager), true, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_XLARGE)) as SignalManager;

        if (_currentSignalManager == null)
        {
            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);
            EditorGUILayout.LabelField("OR", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_SMALL));
            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);
            if (GUILayout.Button("Create New Manager", GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM)))
            {
                GameObject sceneManagerObject = new GameObject("SignalManager");
                _currentSignalManager = sceneManagerObject.AddComponent<SignalManager>();
            }
        }

        if (_currentSignalManager == null)
        {
            return;
        }

        if (_serializedSignalManagerObject == null)
        {
            _serializedSignalManagerObject = new SerializedObject(_currentSignalManager);
            _serializedSignalObjects = _serializedSignalManagerObject.FindProperty("Signals");
            _serializedTimeBoxes = _serializedSignalManagerObject.FindProperty("TimeBoxedTrafficSignals");
        }

        if(_numberOfTimeBoxes == -1)
            _numberOfTimeBoxes = _currentSignalManager.TimeBoxedTrafficSignals.Count;

        GUILayout.Space(EditorUtils.SPACE_SIZE_LARGE);
        EditorGUI.indentLevel += 2;
        
        DisplayIntervalInSeconds();

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
        EditorUtils.DrawHorizontalLine(Color.black);
        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        _globalScrollPosition = EditorGUILayout.BeginScrollView(_globalScrollPosition);

        DisplaySignalsList();

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
        EditorUtils.DrawHorizontalLine(Color.black);
        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
        
        DisplayTimeBoxes();


        _serializedSignalManagerObject.ApplyModifiedProperties();
        _serializedSignalManagerObject.Update();
        
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel -= 2;
    }

    private static void DisplayIntervalInSeconds()
    {
        _currentSignalManager.IntervalPerSignalInSeconds = EditorGUILayout.IntField("Signal Interval(Secs)", _currentSignalManager.IntervalPerSignalInSeconds, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_XLARGE));
    }

    private void DisplaySignalsList()
    {
        EditorGUILayout.PropertyField(_serializedSignalObjects);

        _serializedSignalManagerObject.ApplyModifiedProperties();
        _serializedSignalManagerObject.Update();
    }

    private void DisplayTimeBoxes()
    {
        _numberOfTimeBoxes = EditorGUILayout.IntField("No. of Time boxes", _numberOfTimeBoxes, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_XLARGE));
        ConsolidateNumberOfTimeBoxes();
        EditorGUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        for (int i = 0; i < _serializedTimeBoxes.arraySize; i++)
        {
            EditorUtils.DrawHorizontalLine(Color.cyan);
            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
            EditorGUILayout.LabelField("Timebox " + (i + 1), EditorStyles.boldLabel);
            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);

            _serializedTimeBoxSignalsArray = _serializedTimeBoxes.GetArrayElementAtIndex(i).FindPropertyRelative("Signals");

            _serializedTimeBoxSignalsArray.arraySize = _serializedSignalObjects.arraySize;

            for (int j = 0; j < _serializedTimeBoxSignalsArray.arraySize; j++)
            {
                EditorGUILayout.LabelField("Signal " + j);
                _serializedCurrentDirectionsArray = _serializedTimeBoxSignalsArray.GetArrayElementAtIndex(j).FindPropertyRelative("CurrentDirections");

                EditorGUILayout.PropertyField(_serializedCurrentDirectionsArray);

                EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);
                EditorUtils.DrawHorizontalLine(Color.gray);
                EditorGUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);
            }

            EditorGUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);
        }

        _serializedSignalCreator.ApplyModifiedProperties();
        _serializedSignalCreator.Update();

    }

    private void ConsolidateNumberOfTimeBoxes()
    {
        if (_currentSignalManager.TimeBoxedTrafficSignals.Count != _numberOfTimeBoxes)
        {
            if (_currentSignalManager.TimeBoxedTrafficSignals.Count > _numberOfTimeBoxes)
            {
                for (int i = _currentSignalManager.TimeBoxedTrafficSignals.Count - 1; i >= _numberOfTimeBoxes; i--)
                {
                    _currentSignalManager.TimeBoxedTrafficSignals.RemoveAt(i);
                }
            }
            else
            {
                for (int i = _currentSignalManager.TimeBoxedTrafficSignals.Count; i <= _numberOfTimeBoxes; i++)
                {
                    _currentSignalManager.TimeBoxedTrafficSignals.Add(new TrafficSignalsCollective());
                }
            }
        }
    }
}
