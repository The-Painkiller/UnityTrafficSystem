using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Manager class for a single or a group of signals at any given intersection.
    /// Timeboxing sequence runs here through which signals can by cycled on an intersection.
    /// </summary>
    public class SignalManager : MonoBehaviour
    {
        public SignalObjects[] Signals = null;
        public int IntervalPerSignalInSeconds = 3;


        [Tooltip("Each Index in the root list represents a time box.\nLength of _timeBoxedTrafficSignals = _numberOfTimeBoxes.\nLength of _timeBoxedTrafficSignals[i].CurrentDirection = Length of _signals.\nLength _timeBoxedTrafficSignals[i].CurrentDirection[j] = number of Directions currently active on signal i.\nE.g. In timebox 0, signals 1 & 3 can have forward and right active for each,meanwhile signals 2 & 4 are off. In timebox 1, the signals change.")]
        public List<TrafficSignalsCollective> TimeBoxedTrafficSignals = null;


        private int _currentTimeboxIndex = 0;
        private float _currentTimer = 0f;

        private void Awake()
        {
            if (Signals == null || Signals.Length == 0)
            {
                enabled = false;
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            AssignSignalsToIndicators();
            InvokeRepeating("CycleTimeBox", 0, IntervalPerSignalInSeconds);
        }

        /// <summary>
        /// Assigns signals to their respective indicators, which are basically signal trigger colliders that interact with vehicle Trigger detection.
        /// </summary>
        private void AssignSignalsToIndicators()
        {
            for (int i = 0; i < Signals.Length; i++)
            {
                Signals[i].SignalCollider.AssignSignalController(Signals[i].Signal);
            }
        }

        /// <summary>
        /// switches the signals to the next timebox cycle.
        /// </summary>
        private void CycleTimeBox()
        {
            if (_currentTimeboxIndex < 0 || _currentTimeboxIndex >= TimeBoxedTrafficSignals.Count)
                _currentTimeboxIndex = 0;

            for (int i = 0; i < Signals.Length; i++)
            {
                if (TimeBoxedTrafficSignals[_currentTimeboxIndex].Signals == null
                    || TimeBoxedTrafficSignals[_currentTimeboxIndex].Signals[i].CurrentDirections.Length == 0
                    || TimeBoxedTrafficSignals[_currentTimeboxIndex].Signals[i].CurrentDirections[0] == SignalDirectionID.None)
                {
                    Signals[i].Signal.SwitchSignal(TrafficSignalStateID.Red);
                    continue;
                }

                Signals[i].Signal.SwitchSignal(TrafficSignalStateID.Green, TimeBoxedTrafficSignals[_currentTimeboxIndex].Signals[i].CurrentDirections);
            }
        }

        private void Update()
        {
            if (_currentTimer == 0f)
            {
                CycleTimeBox();
                _currentTimeboxIndex++;
            }

            _currentTimer += Time.deltaTime;
            if (_currentTimer >= IntervalPerSignalInSeconds)
            {
                _currentTimer = 0f;
            }
        }


#if UNITY_EDITOR

        GUIStyle _style = new GUIStyle();
        private void OnDrawGizmosSelected()
        {
            if (Signals == null || Signals.Length == 0)
                return;

            for (int i = 0; i < Signals.Length; i++)
            {
                if (Signals[i].Signal == null)
                {
                    continue;
                }

                _style.normal.textColor = Color.green;
                _style.fontSize = 20;
                _style.fontStyle = FontStyle.Bold;

                UnityEditor.Handles.Label(Signals[i].Signal.transform.position, i.ToString(), _style);
            }
        }

        [ContextMenu("Red")]
        public void SwitchToRedSignal()
        {
            Signals[0].Signal.SwitchSignal(TrafficSignalStateID.Red);
        }

        [ContextMenu("Yellow")]
        public void SwitchToYellowSignal()
        {
            Signals[0].Signal.SwitchSignal(TrafficSignalStateID.Yellow);
        }

        [ContextMenu("Green")]
        public void SwitchToGreenSignal()
        {
            Signals[0].Signal.SwitchSignal(TrafficSignalStateID.Green, new SignalDirectionID[] { SignalDirectionID.Forward, SignalDirectionID.Left });
        }
#endif
    }

    /// <summary>
    /// A collection struct of an array of SignalDirectionCollective
    /// for better data structuring. 
    /// One element here represents one signal's supported directions.
    /// So if an intersection has 3 signals, the Signals.Length will also be 3.
    /// </summary>
    [Serializable]
    public struct TrafficSignalsCollective
    {
        [SerializeField]
        public SignalDirectionsCollective[] Signals;
    }

    /// <summary>
    /// A collection of an array of all the current directions within a signal.
    /// </summary>
    [Serializable]
    public struct SignalDirectionsCollective
    {
        public SignalDirectionID[] CurrentDirections;
    }

    /// <summary>
    /// A simple collection struct of a signal and it's indicator collider for better data structuring.
    /// </summary>
    [Serializable]
    public struct SignalObjects
    {
        public TrafficSignalController Signal;
        public SignalIndicator SignalCollider;
    }
}