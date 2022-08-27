using System;
using UnityEngine;

public enum CollisionTypes
{
    None,
    Signal,
    Proximity
}

public class VehicleCollisionDetector : MonoBehaviour
{
    private SignalIndicator _currentSignalIndicator = null;
    public SignalIndicator CurrentSignalIndicator => _currentSignalIndicator;

    public Action<CollisionTypes> TriggerEncountered;


    private void OnTriggerEnter(Collider trigger)
    {
        ///Add vehicle proximity logic.
        ///Add Triggers in a list and remove similarly.
        if (trigger.GetComponent<Collider>().isTrigger)
        {
            _currentSignalIndicator = trigger.gameObject.GetComponent<SignalIndicator>();
            if (_currentSignalIndicator == null)
            {
                return;
            }

            TriggerEncountered?.Invoke(CollisionTypes.Signal);

            _currentSignalIndicator.SignalChanged += OnSignalChanged;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (_currentSignalIndicator != null)
            _currentSignalIndicator.SignalChanged -= OnSignalChanged;
    }

    private void OnCollisionEnter(Collision collision)
    {
       ///Add collisions in a list and remove similarly.
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnSignalChanged()
    {
        TriggerEncountered?.Invoke(CollisionTypes.Signal);
    }
}
