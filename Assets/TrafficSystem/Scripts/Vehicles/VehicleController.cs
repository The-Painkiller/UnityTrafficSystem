using System;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    private Vehicle _vehicle;

    private VehicleCollisionDetector _collisionDetector;
    private WaypointManager _waypointManager = null;

    private int _totalPathLength = 0;
    private int _currentWaypointIndex = 0;

    private bool _isVehicleTurnPending = false;
    public bool IsVehicleTurnPending => _isVehicleTurnPending;

    private bool _isVehicleStopped = false;

    private Waypoint _currentWaypoint = null;
    private Waypoint _previousWaypoint = null;
    private Waypoint _nextWaypoint = null;

    private Vector3 _nextWaypointDirectionVector = Vector3.zero;
    private Vector3 _previousWaypointDirecionVector = Vector3.zero;

    private SignalDirectionID _nextTurnDirection = SignalDirectionID.None;
    public SignalDirectionID NextTurnDirection => _nextTurnDirection;

    private void Awake()
    {
        if (_vehicle == null)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }

        _waypointManager = GetComponent<WaypointManager>();
        _waypointManager.WaypointManagerInitialized += Initialize;

        _collisionDetector = GetComponentInChildren<VehicleCollisionDetector>();
    }
    private void Start()
    {
        _collisionDetector.TriggerEncountered += OnTriggerEncountered;
        DriveVehicleToNextPoint();
    }
    
    private void OnDestroy()
    {
        _collisionDetector.TriggerEncountered -= OnTriggerEncountered;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(_vehicle.VehicleTransform.position, _currentWaypoint.Position) < _vehicle.TurningPointDistance 
            && _isVehicleTurnPending
            && !_isVehicleStopped)
        {
            _vehicle.SetSpeedCommon(true);
        }

        if (_vehicle.VehicleTransform.position.IsEqualTo(_currentWaypoint.Position))
        {
            DriveVehicleToNextPoint();
        }
    }

    private void Initialize()
    {
        if (!_waypointManager || _waypointManager.GetTotalPathLength() == 0)
            return;

        _totalPathLength = _waypointManager.GetTotalPathLength();

        _vehicle.Initialize(_waypointManager.GetPositionAtIndex(0), _waypointManager.GetOrientationAtIndex(0));
        _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);
    }

    private void DriveVehicleToNextPoint()
    {
        _currentWaypointIndex++;
        if (_currentWaypointIndex >= _totalPathLength || _currentWaypoint == null)
        {
            _vehicle.StopVehicle();
            return;
        }

        _previousWaypoint = _currentWaypoint;
        _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);
        _nextWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex + 1);

        _isVehicleTurnPending = CheckForPendingVehicleTurn(out _nextTurnDirection);

        _vehicle.DriveToDestination(_currentWaypoint.Position);
    }

    private bool CheckForPendingVehicleTurn(out SignalDirectionID direction)
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

    private void OnTriggerEncountered(CollisionTypes collisionType)
    {
        switch (collisionType)
        {
            case CollisionTypes.None:
                break;

            case CollisionTypes.Signal:
                SignalEncountered();
                break;

            case CollisionTypes.Proximity:
                break;
        }
    }

    private void SignalEncountered()
    {
        if (_collisionDetector.CurrentSignalIndicator != null)
        {
            TrafficSignalStateID signalState = _collisionDetector.CurrentSignalIndicator.CurrentSignalState;
            
            SignalDirectionID[] signalDirections = _collisionDetector.CurrentSignalIndicator.CurrentActiveDirections;

            switch (signalState)
            {
                case TrafficSignalStateID.None:
                    break;

                case TrafficSignalStateID.Red:
                    _vehicle.StopVehicle();
                    _isVehicleStopped = true;
                    break;

                case TrafficSignalStateID.Yellow:
                    break;

                case TrafficSignalStateID.Green:
                    if (signalDirections.Contains(_nextTurnDirection))
                    {
                        _vehicle.StartVehicle();
                    }
                    _isVehicleStopped = false;
                    break;
            }
        }
    }
}
