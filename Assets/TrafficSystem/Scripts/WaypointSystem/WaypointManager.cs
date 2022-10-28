using UnityEngine;

namespace TrafficSystem
{
/// <summary>
/// Manager class for a path of waypoints.
/// Gets attached on a vehicle and manages the waypoints on the vehicle's assigned path.
/// </summary>
/// 
    public class WaypointManager : MonoBehaviour
    {
        private Waypoint[] _path = null;

        public void AssignPath(Waypoint[] path)
        {
            _path = path;
        }

        public Vector3 GetPositionAtIndex(int index)
        {
            if (index < 0 || index >= _path.Length)
                return Vector3.zero;

            return _path[index].Position;
        }

        public Vector3 GetOrientationAtIndex(int index)
        {
            if (index < 0 || index + 1 >= _path.Length)
                return Vector3.zero;


            return _path[index + 1].transform.position - _path[index].transform.position;
        }

        public Waypoint GetWaypointAtIndex(int index)
        {
            if (index < 0 || index >= _path.Length)
                return null;

            return _path[index];
        }

        public int GetTotalPathLength()
        {
            if (_path == null)
                return 0;

            return _path.Length;
        }
    }
}