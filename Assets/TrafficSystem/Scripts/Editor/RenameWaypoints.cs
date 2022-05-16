using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameWaypoints : Editor
{
    private static Object[] _objects;

    private static PathsManager _pathsManager;

    private void Awake()
    {
        _pathsManager = FindObjectOfType<PathsManager>();
    }

    [MenuItem("Traffic System/RenameWaypoints")]
    public static void Rename()
    {
        _objects = Selection.objects;

        if (_objects == null || _objects.Length == 0)
            return;

        for (int i = 0; i < _objects.Length; i++)
        {
            _objects[i].name = "Waypoint" + i.ToString();
        }
    }
}