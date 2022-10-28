using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Types of triggers that a vehicle might encounter.
    /// Signals or proximity to other vehicles.
    /// </summary>
    public enum TriggerTypes
    {
        None,
        Signal,
        Proximity
    }

    /// <summary>
    /// Trigger detection class.
    /// </summary>
    public class VehicleTriggerDetection : MonoBehaviour
    {
        private SignalIndicator _currentSignalIndicator = null;
        public SignalIndicator CurrentSignalIndicator => _currentSignalIndicator;

        public Action<TriggerTypes> TriggerEncountered;

        private List<Collider> _encounteredTriggers = new List<Collider>();

        private void OnTriggerEnter(Collider trigger)
        {
            if (!_encounteredTriggers.Contains(trigger))
            {
                _encounteredTriggers.Add(trigger);
            }

            if (trigger.tag == "Signal")
            {
                _currentSignalIndicator = trigger.GetComponent<SignalIndicator>();
                if (_currentSignalIndicator == null)
                    return;

                TriggerEncountered?.Invoke(TriggerTypes.Signal);
                _currentSignalIndicator.SignalChanged += OnSignalChanged;
            }
            else if (trigger.tag == "Vehicle")
            {
                TriggerEncountered?.Invoke(TriggerTypes.Proximity);
            }

        }

        private void OnTriggerExit(Collider trigger)
        {
            if (_encounteredTriggers.Contains(trigger))
            {
                _encounteredTriggers.Remove(trigger);
            }

            if (trigger.tag == "Signal")
            {
                if (_currentSignalIndicator == null)
                    return;

                _currentSignalIndicator.SignalChanged -= OnSignalChanged;
                _currentSignalIndicator = null;
                TriggerEncountered?.Invoke(TriggerTypes.None);
            }
            else if (trigger.tag == "Vehicle")
            {
                TriggerEncountered?.Invoke(TriggerTypes.None);
            }
        }

        /// <summary>
        /// This is called each time the signal that you are standing at, changes state.
        /// </summary>
        private void OnSignalChanged()
        {
            TriggerEncountered?.Invoke(TriggerTypes.Signal);
        }
    }
}