using System;
using System.Collections.Generic;
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
    public Action<bool> CollisionEncountered;

    private List<Collider> _encounteredTriggers = new List<Collider>();
    private List<Collision> _encounteredCollisions = new List<Collision>();

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.isTrigger)
        {
            if (!_encounteredTriggers.Contains(trigger))
            {
                _encounteredTriggers.Add(trigger);
            }

            _currentSignalIndicator = trigger.gameObject.GetComponent<SignalIndicator>();
            if (_currentSignalIndicator != null)
            {
                TriggerEncountered?.Invoke(CollisionTypes.Signal);
                _currentSignalIndicator.SignalChanged += OnSignalChanged;
                return;
            }

            if (trigger.GetComponent<VehicleCollisionDetector>() != null)
            {
                TriggerEncountered?.Invoke(CollisionTypes.Proximity);
            }
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (_currentSignalIndicator != null)
            _currentSignalIndicator.SignalChanged -= OnSignalChanged;

        if (_encounteredTriggers.Contains(trigger))
        {
            _encounteredTriggers.Remove(trigger);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_encounteredCollisions.Contains(collision))
        {
            _encounteredCollisions.Add(collision);
            CollisionEncountered?.Invoke(true);
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (_encounteredCollisions.Contains(collision))
        {
            _encounteredCollisions.Remove(collision);
        }

        if (_encounteredCollisions.Count == 0)
        {
            CollisionEncountered?.Invoke(false);
        }
    }

    private void OnSignalChanged()
    {
        TriggerEncountered?.Invoke(CollisionTypes.Signal);
    }
}
