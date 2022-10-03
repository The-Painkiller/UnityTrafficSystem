using System;
using System.Collections;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{
    [SerializeField]
    private VehicleController[] _vehicles = null;
    
    private VehicleController _currentVehicleController = null;

    private PathsManager _pathsManager = null;

    private Queue _vehiclesPool = new Queue();

    private float _vehicleGenerateTimer = 0f;
    private void Awake()
    {
        if (_vehicles == null)
            return;

        _vehiclesPool.Clear();
        
        for (int i = 0; i < _vehicles.Length; i++)
        {
            _currentVehicleController = Instantiate(_vehicles[i]);
            EnqueueVehicle(_currentVehicleController);
        }

        VehicleController.VehicleReachedEnd += OnVehicleReachedEnd;
    }

    private void Start()
    {
        _pathsManager = PathsManager.Instance;
        if (_pathsManager == null)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        VehicleController.VehicleReachedEnd -= OnVehicleReachedEnd;
    }

    private void EnqueueVehicle(VehicleController vehicleController)
    {
        vehicleController.gameObject.SetActive(false);
        _vehiclesPool.Enqueue(vehicleController);
    }

    private void OnVehicleReachedEnd(VehicleController vehicle)
    {
        EnqueueVehicle(vehicle);
    }

    private void GenerateVehicle()
    {
        if (_vehiclesPool.Count == 0)
        {
            return;
        }

        _currentVehicleController = _vehiclesPool.Dequeue() as VehicleController;
        _currentVehicleController.gameObject.SetActive(true);
        _currentVehicleController.Initialize(_pathsManager.GetRandomPath());
    }

    private void Update()
    {
        if (_vehicleGenerateTimer == 0f)
        {
            GenerateVehicle();
        }

        _vehicleGenerateTimer += Time.deltaTime;
        if (_vehicleGenerateTimer >= 10f)
        {
            _vehicleGenerateTimer = 0f;
        }
    }
}
