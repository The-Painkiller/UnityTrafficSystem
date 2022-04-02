using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(WaypointManager))]
public class Vehicle : MonoBehaviour
{

    [SerializeField]
    private VehicleConfiguration _vehicleConfig = null;
    
    private float _movementSpeed = 10f;
    private float _turningSpeed = 5f;
    private float _speedSwitchTimeIncrement = 0.1f;
    private float _speedSwitchTresholdDistance = 1f;

    private Vector3 _currentDestination = Vector3.zero;

    private bool _isActiveOnPath = false;

    private int _totalPathLength = 0;
    private int _currentWaypointIndex = 0;

    private float _newSpeed = 0f;

    private WaypointManager _waypointManager = null;
    private NavMeshAgent _navMeshAgent = null;
    private Transform _transform = null;

    [SerializeField]
    private Waypoint _currentWaypoint = null;
    private Waypoint _previousWaypoint = null;
    private Waypoint _nextWaypoint = null;

    private bool _switchSpeed = false;

    private void Awake()
    {
        _transform = transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _waypointManager = GetComponent<WaypointManager>();
        _waypointManager.WaypointManagerInitialized += InitializePath;
        SetVehicleConfiguration();
    }

    private void SetVehicleConfiguration()
    {
        if (_vehicleConfig == null)
            return;

        _movementSpeed = _vehicleConfig.NormalMovementSpeed;
        _turningSpeed = _vehicleConfig.TurningSpeed;
        _speedSwitchTimeIncrement = _vehicleConfig.SpeedSwitchTimeStep;
        _speedSwitchTresholdDistance = _vehicleConfig.SpeedSwitchTresholdDistance;
        _navMeshAgent.acceleration = _vehicleConfig.Acceleration;
        _navMeshAgent.angularSpeed = _vehicleConfig.AngularSpeed;
    }

    public void InitializePath()
    {
        if (!_waypointManager || _waypointManager.GetTotalPathLength() == 0)
            return;

        _totalPathLength = _waypointManager.GetTotalPathLength();
        _transform.position = _waypointManager.GetPositionAtIndex(0);
        _transform.forward = _waypointManager.GetOrientationAtIndex(0);
        
        _navMeshAgent.isStopped = false;
        _isActiveOnPath = true;

        _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);
        DriveToNextPoint();
    }

    private void DriveToNextPoint()
    {
        _currentWaypointIndex++;
        if (_currentWaypointIndex >= _totalPathLength)
        {
            _navMeshAgent.autoBraking = true;
            _navMeshAgent.isStopped = true;
            return;
        }

        if (_currentWaypoint != null)
        {
            _previousWaypoint = _currentWaypoint;
        }

        if (_currentWaypointIndex + 1 < _waypointManager.GetTotalPathLength())
        {
            _nextWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex + 1);
         }

        _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);

        if (_currentWaypoint && _previousWaypoint && _nextWaypoint)
        {
            float angle = Vector3.Angle(_currentWaypoint.Position - _previousWaypoint.Position, _currentWaypoint.Position - _nextWaypoint.Position);

            Debug.Log("Angle: " + angle);

            _newSpeed = angle >= 35f && angle <= 145f ? _turningSpeed : _movementSpeed;
            Debug.Log(_newSpeed);
            SwitchSpeed();
        }

        DriveToDestination(_waypointManager.GetPositionAtIndex(_currentWaypointIndex));
    }

    private void DriveToDestination(Vector3 destination)
    {
        _currentDestination = destination;
        _navMeshAgent.SetDestination(_currentDestination);
    }


    private void FixedUpdate()
    {
        if (_switchSpeed)
            SwitchSpeed();
    }

    private void Update()
    {
        if (!_isActiveOnPath)
            return;

        if (AreEqual(_transform.localPosition, _currentDestination))
        {
            DriveToNextPoint();
        }
    }

    private void SwitchSpeed()
    {
        if (AreEqual(_navMeshAgent.speed, _newSpeed))
        {
            Debug.Log("Speed Switch Complete.");
            _switchSpeed = false;
            return;
        }

        Debug.Log("Switching Speed: " + _navMeshAgent.speed);
        _switchSpeed = true;
        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _newSpeed, _speedSwitchTimeIncrement);
    }

    private bool AreEqual(float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    private bool AreEqual(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b) <= 1.5f;
    }
}
