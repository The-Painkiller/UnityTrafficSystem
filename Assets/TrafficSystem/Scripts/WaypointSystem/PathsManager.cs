using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// This class holds all the paths created by a group of waypoints across the map.
    /// </summary>
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

        /// <summary>
        /// Passes a random path from it's list of Paths.
        /// </summary>
        /// <returns></returns>
        public Waypoint[] GetRandomPath()
        {
            if (_paths.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, _paths.Count);
                if (_pathsClear[index])
                    return _paths[index].Waypoints;
                else
                    return GetRandomPath();
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gets called by Path Creator tool to get a path assigned to it's list.
        /// </summary>
        /// <param name="waypoints"></param>
        public void CreatePathFromSelectedWaypoints(Waypoint[] waypoints)
        {
            _paths.Add(new Path() { Waypoints = waypoints });
        }

#endif
    }

    /// <summary>
    /// A basic collection of an array of waypoints to help display in the inspector.
    /// </summary>
    [Serializable]
    public struct Path
    {
        [SerializeField]
        public Waypoint[] Waypoints;
    }
}