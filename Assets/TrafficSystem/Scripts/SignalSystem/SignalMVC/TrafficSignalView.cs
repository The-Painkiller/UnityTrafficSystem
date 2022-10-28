namespace TrafficSystem
{
    using UnityEngine;
    using System;

    /// <summary>
    /// View class for a Traffic Signal.
    /// Simply manages the display for each signal block based on what the Controller passes.
    /// </summary>
    public class TrafficSignalView : MonoBehaviour
    {
        [SerializeField]
        private SignalRenderer _redSignal;
        [SerializeField]
        private SignalRenderer _yellowSignal;
        [SerializeField]
        private SignalRenderer[] _greenSignals;

        /// <summary>
        /// Switches the signal's display from state to state as well as active directions.
        /// </summary>
        /// <param name="signalStateID">Desired signal state.</param>
        /// <param name="directionID">Desired active directions to display in case of Green state.</param>
        public void SwitchSignal(TrafficSignalStateID signalStateID, SignalDirectionID[] directionID = null)
        {
            switch (signalStateID)
            {
                case TrafficSignalStateID.Red:
                    ToggleSignal(_redSignal, true);
                    ToggleSignal(_yellowSignal, false);

                    for (int i = 0; i < _greenSignals.Length; i++)
                    {
                        ToggleSignal(_greenSignals[i], false);
                    }
                    break;

                case TrafficSignalStateID.Yellow:
                    ToggleSignal(_redSignal, false);
                    ToggleSignal(_yellowSignal, true);

                    for (int i = 0; i < _greenSignals.Length; i++)
                    {
                        ToggleSignal(_greenSignals[i], false);
                    }
                    break;

                case TrafficSignalStateID.Green:
                    ToggleSignal(_redSignal, false);
                    ToggleSignal(_yellowSignal, false);

                    if (directionID == null || _greenSignals.Length < directionID.Length)
                        return;

                    for (int i = 0; i < _greenSignals.Length; i++)
                    {
                        ToggleSignal(_greenSignals[i], false);
                        for (int j = 0; j < directionID.Length; j++)
                        {
                            if (directionID[j] == _greenSignals[i].SignalDirection)
                            {
                                ToggleSignal(_greenSignals[i], true);
                                break;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }


        private void ToggleSignal(SignalRenderer signal, bool toggle)
        {
            signal.SignalObject.enabled = toggle;
        }
    }

    /// <summary>
    /// Just a simple struct to display Sprite renderer and the representive direction of the signal block together in the inspector.
    /// </summary>
    [Serializable]
    public struct SignalRenderer
    {
        public SpriteRenderer SignalObject;
        public SignalDirectionID SignalDirection;
    }
}