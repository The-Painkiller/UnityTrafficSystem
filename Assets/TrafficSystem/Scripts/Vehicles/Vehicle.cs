using UnityEngine;
using UnityEngine.AI;

namespace TrafficSystem
{
    /// <summary>
    /// Vehicle class. Depends on the navmesh agent and Waypoint Manager classes.
    /// This class is simply used to start or stop the vehicle and set directions/next waypoints to the vehicle and move it around.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(WaypointManager))]
    public class Vehicle : MonoBehaviour
    {
        [SerializeField]
        private VehicleConfiguration _vehicleConfig = null;

        private float _movementSpeed = 10f;
        private float _turningSpeed = 5f;
        private float _speedSwitchTimeIncrement = 0.1f;
        private float _newSpeed = 0f;
        private float _cachedSpeed = 0f;
        private float _turningPointDistance = 1f;
        public float TurningPointDistance => _turningPointDistance;

        private Vector3 _currentDestination = Vector3.zero;

        private bool _isActiveOnPath = false;

        private Transform _transform = null;
        public Transform VehicleTransform => _transform;

        private NavMeshAgent _navMeshAgent = null;

        private void Awake()
        {
            _transform = transform;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            SetVehicleConfiguration();
        }

        /// <summary>
        /// Sets some basic vehicle configurations like speeds, acceleration etc., for the navmesh.
        /// </summary>
        private void SetVehicleConfiguration()
        {
            if (_vehicleConfig == null)
                return;

            _movementSpeed = _vehicleConfig.NormalMovementSpeed;
            _turningSpeed = _vehicleConfig.TurningSpeed;
            _speedSwitchTimeIncrement = _vehicleConfig.SpeedSwitchTimeStep;
            _turningPointDistance = _vehicleConfig.TurningPointDistance;
            _navMeshAgent.acceleration = _vehicleConfig.Acceleration;
            _navMeshAgent.angularSpeed = _vehicleConfig.AngularSpeed;
        }

        /// <summary>
        /// Sets the navmesh agent's speed and the next destination to travel to.
        /// </summary>
        /// <param name="destination"></param>
        public void DriveToDestination(Vector3 destination)
        {
            _newSpeed = _movementSpeed;
            _currentDestination = destination;
            _navMeshAgent.SetDestination(_currentDestination);
        }

        /// <summary>
        /// Simply sets the speed of the next waypoint to travel to.
        /// </summary>
        /// <param name="isTurning">Whether the vehicle will turn for going to the next waypoint. Turning speed is slower than normal speed.</param>
        public void SetNextWaypointSpeed(bool isTurning)
        {
            if (isTurning)
            {
                SetSpeed(_turningSpeed);
            }
            else
            {
                SetSpeed(_movementSpeed);
            }
        }

        /// <summary>
        /// Simply sets the speed of the next waypoint to travel to.
        /// </summary>
        /// <param name="speed"></param>
        private void SetSpeed(float speed)
        {
            _newSpeed = speed;
        }

        private void FixedUpdate()
        {
            if (!_isActiveOnPath)
                return;

            SpeedSwitch();
        }

        /// <summary>
        /// Run in FixedUpdate for transitioning between current and the newly set speed in a set time.
        /// </summary>
        private void SpeedSwitch()
        {
            if (_navMeshAgent.speed.IsEqualTo(_newSpeed))
                return;

            _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _newSpeed, _speedSwitchTimeIncrement);
        }

        /// <summary>
        /// Initializes the vehicle to the first waypoint and begins it's journey.
        /// </summary>
        /// <param name="startPosition">The starting position to appear at and activate.</param>
        /// <param name="startForwardOrientation">The starting orientation to appear at and activate.</param>
        public void Initialize(Vector3 startPosition, Vector3 startForwardOrientation)
        {
            _navMeshAgent.SetDestination(startPosition);
            _transform.position = startPosition;
            _transform.forward = startForwardOrientation;
            _navMeshAgent.isStopped = false;
            _isActiveOnPath = true;
        }

        /// <summary>
        /// Simply starts the vehicle.
        /// </summary>
        public void StartVehicle()
        {
            _newSpeed = _cachedSpeed.IsEqualTo(0f) ? _movementSpeed : _cachedSpeed;
            _navMeshAgent.autoBraking = false;
            _navMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Stops the vehicle.
        /// </summary>
        public void StopVehicle()
        {
            _cachedSpeed = _navMeshAgent.speed;
            _newSpeed = 0;
            _navMeshAgent.autoBraking = true;
            _navMeshAgent.isStopped = true;
        }
    }
}