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
    private float _turningPointDistance = 1f;

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
        _turningPointDistance = _vehicleConfig.TurningPointDistance;
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
        if (_currentWaypointIndex >= _totalPathLength || _currentWaypoint == null)
        {
            _navMeshAgent.autoBraking = true;
            _navMeshAgent.isStopped = true;
            return;
        }

        _previousWaypoint = _currentWaypoint;
        _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);
        _nextWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex + 1);

        _newSpeed = _movementSpeed;
        DriveToDestination(_currentWaypoint.Position);
    }

    private void DriveToNextPoint(int x)
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

        if(!_navMeshAgent.speed.IsEqualTo(_movementSpeed))
        {
            _newSpeed = _movementSpeed;
            SpeedSwitch();
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
        if (!_isActiveOnPath)
            return;

        SpeedSwitch();

        if (Vector3.Distance(_transform.position, _currentDestination) < _turningPointDistance)
        {
            HandleTurning();
        }

        if (_transform.position.IsEqualTo(_currentDestination))
        {
            DriveToNextPoint();
        }
    }

    private void SpeedSwitch()
    {
        if (_navMeshAgent.speed.IsEqualTo(_newSpeed))
            return;

        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _newSpeed, _speedSwitchTimeIncrement);
        //Debug.Log("Switching Speed: " + _navMeshAgent.speed);
    }

    private void HandleTurning()
    {
        if (_nextWaypoint == null)
            return;

        float angle = Vector3.Angle(_currentWaypoint.Position - _previousWaypoint.Position, _currentWaypoint.Position - _nextWaypoint.Position);

        if (angle >= 35f && angle <= 145f)
        {
            _newSpeed = _turningSpeed;
        }
    }
}
