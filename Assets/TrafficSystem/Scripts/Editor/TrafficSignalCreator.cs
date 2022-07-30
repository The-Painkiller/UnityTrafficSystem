using UnityEngine;
using UnityEditor;

public class TrafficSignalCreator : EditorWindow
{
    private const float WINDOW_WIDTH = 600f;
    private const float WINDOW_HEIGHT = 600f;

    private static SignalManager _currentSignalManager = null;

    private SerializedObject _serializedSignalManager;
    private SerializedProperty _serializedSignalsArray;
    private SerializedProperty _serializedCollidersArray;
    private SerializedProperty _serializedTimeBoxedSignalsList;
    private SerializedProperty _serializedSignalDirectionsArray;

    private Vector2 _globalScrollPosition = Vector2.zero;
    private Vector2 _timeboxScrollPosition = Vector2.zero;

    private GUIStyle _verticalLine = new GUIStyle();

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
            return;

        if (_serializedSignalManager == null || _serializedSignalsArray == null)
        {
            _serializedSignalManager = new SerializedObject(_currentSignalManager);
            _serializedSignalsArray = _serializedSignalManager.FindProperty("Signals");
        }

        if (_serializedTimeBoxedSignalsList == null)
        {
            _serializedTimeBoxedSignalsList = _serializedSignalManager.FindProperty("TimeBoxedTrafficSignals");
        }

        if (_serializedCollidersArray == null)
        {
            _serializedCollidersArray = _serializedSignalManager.FindProperty("SignalIndicators");
        }

        if (_currentSignalManager.SignalIndicators == null || _currentSignalManager.SignalIndicators.Length != _currentSignalManager.Signals.Length)
        {
            _currentSignalManager.SignalIndicators = new  SignalIndicator[_currentSignalManager.Signals.Length];
        }

        GUILayout.Space(EditorUtils.SPACE_SIZE_LARGE);

        EditorGUI.indentLevel += 2;
        _globalScrollPosition = EditorGUILayout.BeginScrollView(_globalScrollPosition);
        DisplaySignalManager();
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel -= 2;
    }

    private void DisplaySignalManager()
    {
        _currentSignalManager.IntervalPerSignalInSeconds = EditorGUILayout.IntField("Time Per Signal", _currentSignalManager.IntervalPerSignalInSeconds, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM));

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(_serializedSignalsArray, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_LARGE));

        EditorGUILayout.PropertyField(_serializedCollidersArray, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_XLARGE));

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);

        EditorUtils.DrawHorizontalLine(Color.black);

        GUILayout.Space(EditorUtils.SPACE_SIZE_SMALL);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.IntField("Time Boxes", _currentSignalManager.TimeBoxedTrafficSignals.Count, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM));

        if (GUILayout.Button("+", GUILayout.MaxWidth(EditorUtils.SPACE_SIZE_LARGE)))
        {
            _currentSignalManager.TimeBoxedTrafficSignals.Add(new TrafficSignalsCollective());
        }
        if (GUILayout.Button("-", GUILayout.MaxWidth(EditorUtils.SPACE_SIZE_LARGE)))
        {
            if (_currentSignalManager.TimeBoxedTrafficSignals.Count >= 1)
            {
                _currentSignalManager.TimeBoxedTrafficSignals.RemoveAt(_currentSignalManager.TimeBoxedTrafficSignals.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        DisplayTimeBoxes();
        
        _serializedSignalManager.ApplyModifiedProperties();
    }

    private void DisplayTimeBoxes()
    {
        _timeboxScrollPosition = EditorGUILayout.BeginScrollView(_timeboxScrollPosition);

        EditorUtils.DrawHorizontalLine(Color.gray, 400f);

        for (int i = 0; i < _currentSignalManager.TimeBoxedTrafficSignals.Count; i++)
        {
            ////FIX THIS CONDITION. NOT REFLECTING PROPERLY!!
            if (_currentSignalManager.TimeBoxedTrafficSignals[i].Signals == null || _currentSignalManager.TimeBoxedTrafficSignals[i].Signals.Length != _currentSignalManager.Signals.Length)
            {
                TrafficSignalsCollective trafficSignalsCollective = _currentSignalManager.TimeBoxedTrafficSignals[i];

                trafficSignalsCollective.Signals = new SignalDirectionsCollective[_currentSignalManager.Signals.Length];

                _currentSignalManager.TimeBoxedTrafficSignals[i] = trafficSignalsCollective;

                Repaint();
            }

            EditorGUILayout.LabelField("Time Box " +  (i + 1));

            _serializedSignalDirectionsArray = _serializedTimeBoxedSignalsList.GetArrayElementAtIndex(i).FindPropertyRelative("Signals");

            EditorGUI.indentLevel += 2;
            EditorGUILayout.PropertyField(_serializedSignalDirectionsArray, GUILayout.MaxWidth(400f));
            EditorGUI.indentLevel -= 2;

            EditorUtils.DrawHorizontalLine(Color.gray, 400f);
        }

        EditorGUILayout.EndScrollView();
    }
}
