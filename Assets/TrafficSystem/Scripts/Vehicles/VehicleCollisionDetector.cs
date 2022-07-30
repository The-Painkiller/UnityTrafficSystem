using System;
using UnityEngine;

public enum CollisionTypes
{
    None,
    Signal,
    VehicleProximity
}

public class VehicleCollisionDetector : MonoBehaviour
{
    private SignalIndicator _currentSignalIndicator = null;
    public SignalIndicator CurrentSignalIndicator => _currentSignalIndicator;

    public Action<CollisionTypes> TriggerEncountered;


    private void OnTriggerEnter(Collider trigger)
    {
        ///Add vehicle proximity logic.
        
        _currentSignalIndicator = trigger.GetComponent<SignalIndicator>();
        if (_currentSignalIndicator == null)
        {
            return;
        }

        TriggerEncountered?.Invoke(CollisionTypes.Signal);

        _currentSignalIndicator.SignalChanged += OnSignalChanged;
    }

    private void OnTriggerExit(Collider trigger)
    {
        _currentSignalIndicator.SignalChanged -= OnSignalChanged;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnSignalChanged()
    {

    }
}
