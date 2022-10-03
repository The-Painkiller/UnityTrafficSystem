using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathsManager : MonoBehaviour
{
    [SerializeField]
    private List<Path> _paths = null;

    private bool[] _pathsClear = null;

    public static PathsManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (_paths == null || _paths.Count == 0)
        {
            return;
        }

        _pathsClear = new bool[_paths.Count];
        for (int i = 0; i < _pathsClear.Length; i++)
        {
            _pathsClear[i] = _paths[i].Waypoints != null && _paths[i].Waypoints.Length > 1;
        }
    }

    public Waypoint[] GetRandomPath()
    {
        if (_paths.Count > 0)
        {
            return _paths[UnityEngine.Random.Range(0, _paths.Count)].Waypoints;
        }

        return null;
    }

#if UNITY_EDITOR
    public void CreatePathFromSelectedWaypoints(Waypoint[] waypoints)
    {
        _paths.Add(new Path() { Waypoints = waypoints });
    }
#endif
}

[Serializable]
public struct Path
{
    [SerializeField]
    public Waypoint[] Waypoints;
}
