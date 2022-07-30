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
    private float _newSpeed = 0f;
    private float _cachedSpeed = 0f;

    private Vector3 _currentDestination = Vector3.zero;
    private Vector3 _nextWaypointDirectionVector = Vector3.zero;
    private Vector3 _previousWaypointDirecionVector = Vector3.zero;

    private bool _isActiveOnPath = false;
    private bool _isTurning = false;

    private int _totalPathLength = 0;
    private int _currentWaypointIndex = 0;


    private WaypointManager _waypointManager = null;
    private NavMeshAgent _navMeshAgent = null;
    private Transform _transform = null;

    private Waypoint _currentWaypoint = null;
    private Waypoint _previousWaypoint = null;
    private Waypoint _nextWaypoint = null;

    private SignalDirectionID _nextTurnDirection = SignalDirectionID.None;
    public SignalDirectionID NextTurnDirection => _nextTurnDirection;

    public bool IsTurning => _isTurning;

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

        _isTurning = CheckForTurnAhead(out _nextTurnDirection);
        DriveToDestination(_currentWaypoint.Position);
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

        if (Vector3.Distance(_transform.position, _currentDestination) < _turningPointDistance && IsTurning)
        {
            _newSpeed = _turningSpeed;
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
    }

    private bool CheckForTurnAhead(out SignalDirectionID direction)
    {
        if (_nextWaypoint == null)
        {
            direction = SignalDirectionID.None;
            return false;
        }

        _previousWaypointDirecionVector = _currentWaypoint.Position - _previousWaypoint.Position;
        _nextWaypointDirectionVector = _currentWaypoint.Position - _nextWaypoint.Position;

        float angle = Vector3.Angle(_previousWaypointDirecionVector, _nextWaypointDirectionVector);

        if (angle <= 145f)
        {
            Vector3 cross = Vector3.Cross(_previousWaypointDirecionVector, _nextWaypointDirectionVector);

            direction = cross.y >= 0f ? SignalDirectionID.Left : SignalDirectionID.Right;

            return true;
        }

        direction = SignalDirectionID.Forward;
        return false;
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

    public void StartVehicle()
    {
        _newSpeed = _cachedSpeed == 0f ? _movementSpeed : _cachedSpeed;
    }

    public void StopVehicle()
    {
        _cachedSpeed = _navMeshAgent.speed;
        _newSpeed = 0;
    }
}
