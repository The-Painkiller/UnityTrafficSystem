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

    public void DriveToDestination(Vector3 destination)
    {
        _newSpeed = _movementSpeed;
        _currentDestination = destination;
        _navMeshAgent.SetDestination(_currentDestination);
    }

    public void SetSpeed(float speed)
    {
        _newSpeed = speed;
    }

    public void SetSpeedCommon(bool isTurning)
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

    private void FixedUpdate()
    {
        if (!_isActiveOnPath)
            return;

        SpeedSwitch();
    }

    private void SpeedSwitch()
    {
        if (_navMeshAgent.speed.IsEqualTo(_newSpeed))
            return;

        _navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _newSpeed, _speedSwitchTimeIncrement);        
    }

    public void Initialize(Vector3 startPosition,  Vector3 startForwardOrientation)
    {
        _transform.position = startPosition;
        _transform.forward = startForwardOrientation;
        _navMeshAgent.isStopped = false;
        _isActiveOnPath = true;
    }

    public void StartVehicle()
    {
        _newSpeed = _cachedSpeed.IsEqualTo(0f) ? _movementSpeed : _cachedSpeed;
        _navMeshAgent.autoBraking = false;
        _navMeshAgent.isStopped = false;
    }


    public void StopVehicle()
    {
        _cachedSpeed = _navMeshAgent.speed;
        _newSpeed = 0;
        _navMeshAgent.autoBraking = true;
        _navMeshAgent.isStopped = true;
    }
}
