using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    private Vehicle _vehicle;

    private VehicleCollisionDetector _collisionDetector;

    private void Awake()
    {
        if (_vehicle == null)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }

        _collisionDetector = GetComponentInChildren<VehicleCollisionDetector>();
    }

    private void Start()
    {
        _collisionDetector.TriggerEncountered += OnTriggerEncountered;
        _vehicle.InitializePath();
    }

    private void OnDestroy()
    {
        _collisionDetector.TriggerEncountered -= OnTriggerEncountered;
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

            case CollisionTypes.VehicleProximity:
                break;
        }
    }

    private void SignalEncountered()
    {
        if (_collisionDetector.CurrentSignalIndicator != null)
        {
            TrafficSignalStateID signalState = _collisionDetector.CurrentSignalIndicator.CurrentSignalState;

            switch (signalState)
            {
                case TrafficSignalStateID.None:
                    break;

                case TrafficSignalStateID.Red:
                    _vehicle.StopVehicle();
                    break;

                case TrafficSignalStateID.Yellow:
                    break;

                case TrafficSignalStateID.Green:
                    _vehicle.StartVehicle();
                    break;
            }
        }
    }
}
