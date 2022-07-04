using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class TrafficSignalCreator : EditorWindow
{
    private const float WINDOW_WIDTH = 800f;
    private const float WINDOW_HEIGHT = 600f;

    private static SignalManager _currentSignalManager = null;

    private SerializedObject _serializedSignalManager;
    private SerializedProperty _serializedSignalsArray;
    private SerializedProperty _serializedTimeBoxedSignalsList;
    private Vector2 _timeboxScrollPosition = Vector2.zero;
    private int? _numberOfTimeBoxes = 0;

    [MenuItem("Traffic System/Traffic Signal Creator")]
    public static void Initialize()
    {
        TrafficSignalCreator window = (TrafficSignalCreator)GetWindow(typeof(TrafficSignalCreator));

        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, WINDOW_WIDTH, WINDOW_HEIGHT);
        window.Show();
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

        GUILayout.Space(EditorUtils.SPACE_SIZE_LARGE);
        DisplaySignalManager();
    }

    private void DisplaySignalManager()
    {
        EditorGUILayout.PropertyField(_serializedSignalsArray, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_LARGE));

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        _currentSignalManager.IntervalPerSignalInSeconds = EditorGUILayout.IntField("Time Per Signal", _currentSignalManager.IntervalPerSignalInSeconds, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM));

        GUILayout.Space(EditorUtils.SPACE_SIZE_MEDIUM);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.IntField("Number of Time Boxes", _currentSignalManager.TimeBoxedTrafficSignals.Count, GUILayout.MaxWidth(EditorUtils.FIELD_SIZE_MEDIUM));

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
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginScrollView(_timeboxScrollPosition);

        for (int i = 0; i < _currentSignalManager.NumberOfTimeBoxes; i++)
        {
            EditorGUILayout.BeginVertical();



            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();
    }
}
