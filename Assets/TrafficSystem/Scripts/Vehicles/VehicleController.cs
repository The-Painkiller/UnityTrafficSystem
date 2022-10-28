using System;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Vehicle controller class. 
    /// This class acts as the main manager class that runs the vehicle,
    /// sets the vehicle colour, interacts with the waypoint manager to travel across the path, as well as,
    /// deals with the triggers and collisions detection.
    /// </summary>
    public class VehicleController : MonoBehaviour
    {
        [SerializeField]
        private Vehicle _vehicle = null;

        private VehicleTriggerDetection _triggerDetector = null;
        private VehicleCollisionDetection _collisionDetector = null;
        private WaypointManager _waypointManager = null;

        private int _totalPathLength = 0;
        private int _currentWaypointIndex = 0;

        private bool _isVehicleTurnPending = false;
        public bool IsVehicleTurnPending => _isVehicleTurnPending;

        private bool _isVehicleStopped = false;
        private bool _isVehicleActive = false;

        private Waypoint _currentWaypoint = null;
        private Waypoint _previousWaypoint = null;
        private Waypoint _nextWaypoint = null;

        private Vector3 _nextWaypointDirectionVector = Vector3.zero;
        private Vector3 _previousWaypointDirecionVector = Vector3.zero;

        private SignalDirectionID _nextTurnDirection = SignalDirectionID.None;
        public SignalDirectionID NextTurnDirection => _nextTurnDirection;

        private SignalManager _currentSignalManager = null;
        private SignalManager _previousSignalManager = null;

        private VehicleColorVariation _colorVariation = null;
        private VehicleStallBehaviour _stallBehaviour = null;

        public static Action<VehicleController> VehicleReachedEnd;

        private const float THRESHOLD_TURN_ANGLE = 150f;

        private void Awake()
        {
            if (_vehicle == null)
            {
                enabled = false;
                gameObject.SetActive(false);
                return;
            }

            _waypointManager = GetComponent<WaypointManager>();
            _collisionDetector = GetComponent<VehicleCollisionDetection>();
            _stallBehaviour = GetComponent<VehicleStallBehaviour>();

            _triggerDetector = GetComponentInChildren<VehicleTriggerDetection>();
            _colorVariation = GetComponentInChildren<VehicleColorVariation>();

            _stallBehaviour.SelfDestruct += OnSelfDestruct;
        }

        private void FixedUpdate()
        {
            if (!_isVehicleActive || _isVehicleStopped)
            {
                return;
            }

            if (Vector3.Distance(_vehicle.VehicleTransform.position, _currentWaypoint.Position) < _vehicle.TurningPointDistance
                && _isVehicleTurnPending)
            {
                _vehicle.SetNextWaypointSpeed(true);
            }

            if (_vehicle.VehicleTransform.position.IsEqualTo(_currentWaypoint.Position))
            {
                DriveVehicleToNextPoint();
            }
        }

        /// <summary>
        /// Initializes the vehicle on a given path.
        /// </summary>
        /// <param name="path"></param>
        public void Initialize(Waypoint[] path)
        {
            if (_waypointManager == null || path.Length == 0)
            {
                VehicleReachedEnd?.Invoke(this);
                return;
            }

            _waypointManager.AssignPath(path);
            _currentWaypointIndex = 0;

            _triggerDetector.TriggerEncountered += OnTriggerEncountered;
            _collisionDetector.CollisionEncountered += OnCollisionEncountered;

            _totalPathLength = _waypointManager.GetTotalPathLength();

            _vehicle.Initialize(_waypointManager.GetPositionAtIndex(0), _waypointManager.GetOrientationAtIndex(0));
            _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);

            if (_colorVariation != null)
            {
                _colorVariation.Generate();
            }

            _isVehicleActive = true;
            DriveVehicleToNextPoint();
        }

        /// <summary>
        /// Finishes and clears the vehicle's current journey before sending back to the object pool.
        /// </summary>
        private void Finish()
        {
            _currentWaypointIndex = 0;
            _triggerDetector.TriggerEncountered -= OnTriggerEncountered;
            _collisionDetector.CollisionEncountered -= OnCollisionEncountered;
        }

        /// <summary>
        /// Picks the next waypoint on the path and drives the vehicle to that waypoint.
        /// </summary>
        private void DriveVehicleToNextPoint()
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _totalPathLength || _currentWaypoint == null)
            {
                ResetVehicle();
                return;
            }

            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex);
            _nextWaypoint = _waypointManager.GetWaypointAtIndex(_currentWaypointIndex + 1);

            _isVehicleTurnPending = CheckForPendingVehicleTurn(out _nextTurnDirection);

            _vehicle.DriveToDestination(_currentWaypoint.Position);
        }

        /// <summary>
        /// Resets the vehicle by clearing it's current data and triggering the related event.
        /// </summary>
        private void ResetVehicle()
        {
            ToggleVehicleMovement(false);
            Finish();
            VehicleReachedEnd?.Invoke(this);
        }

        /// <summary>
        /// Checks whether the next waypoint is at a turn.
        /// If the angle between waypoints is lesser than the threshold turning angle value, it's detected as a turn.
        /// </summary>
        /// <param name="direction">Passes the direction, whether it's a turn or still going forward. This is useful in acting as per signals as well as setting turning speed.</param>
        /// <returns></returns>
        private bool CheckForPendingVehicleTurn(out SignalDirectionID direction)
        {
            if (_nextWaypoint == null)
            {
                direction = SignalDirectionID.Forward;
                return false;
            }

            _previousWaypointDirecionVector = _currentWaypoint.Position - _previousWaypoint.Position;
            _nextWaypointDirectionVector = _currentWaypoint.Position - _nextWaypoint.Position;

            float angle = Vector3.Angle(_previousWaypointDirecionVector, _nextWaypointDirectionVector);

            if (angle <= THRESHOLD_TURN_ANGLE)
            {
                Vector3 cross = Vector3.Cross(_previousWaypointDirecionVector, _nextWaypointDirectionVector);

                direction = cross.y >= 0f ? SignalDirectionID.Left : SignalDirectionID.Right;

                return true;
            }

            direction = SignalDirectionID.Forward;
            return false;
        }

        private void OnTriggerEncountered(TriggerTypes collisionType)
        {
            switch (collisionType)
            {
                case TriggerTypes.None:
                    ToggleVehicleMovement(true);
                    _previousSignalManager = _currentSignalManager;
                    _currentSignalManager = null;
                    break;

                case TriggerTypes.Signal:
                    SignalEncountered();
                    break;

                case TriggerTypes.Proximity:
                    ToggleVehicleMovement(false);
                    break;
            }
        }

        /// <summary>
        /// Decides what to do if a signal has been encountered.
        /// </summary>
        private void SignalEncountered()
        {
            if (_triggerDetector.CurrentSignalIndicator != null)
            {
                _currentSignalManager = _triggerDetector.CurrentSignalIndicator.ParentSignalManager;

                if (_previousSignalManager == _currentSignalManager)
                    return;

                TrafficSignalStateID signalState = _triggerDetector.CurrentSignalIndicator.CurrentSignalState;

                SignalDirectionID[] signalDirections = _triggerDetector.CurrentSignalIndicator.CurrentActiveDirections;

                switch (signalState)
                {
                    case TrafficSignalStateID.None:
                        break;

                    case TrafficSignalStateID.Red:
                        ToggleVehicleMovement(false);
                        break;

                    case TrafficSignalStateID.Yellow:
                        break;

                    case TrafficSignalStateID.Green:
                        if (signalDirections.Contains(_nextTurnDirection))
                        {
                            ToggleVehicleMovement(true);
                        }
                        else
                        {
                            ToggleVehicleMovement(false);
                        }
                        break;
                }
            }
        }

        private void OnCollisionEncountered(CollisionTypes collision)
        {
            switch (collision)
            {
                case CollisionTypes.None:
                    ToggleVehicleMovement(true);
                    _stallBehaviour.Activate(false);
                    break;

                case CollisionTypes.Vehicle:
                    _stallBehaviour.Activate(true);
                    ToggleVehicleMovement(false);
                    break;
            }
        }

        /// <summary>
        /// Gets called when the Stall Behaviour triggers a self destruct.
        /// Just resets the vehicle back to the pool.
        /// </summary>
        private void OnSelfDestruct()
        {
            ResetVehicle();
        }

        /// <summary>
        /// Starts or stops the vehicle.
        /// </summary>
        /// <param name="start"></param>
        private void ToggleVehicleMovement(bool start)
        {
            if (start)
            {
                _vehicle.StartVehicle();
                _isVehicleStopped = false;
            }
            else
            {
                _vehicle.StopVehicle();
                _isVehicleStopped = true;
                _stallBehaviour.Activate(true);
            }
        }
    }
}