using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    public SignalObjects[] Signals = null;
    public int IntervalPerSignalInSeconds = 3;
    

    [Tooltip("Each Index in the root list represents a time box.\nLength of _timeBoxedTrafficSignals = _numberOfTimeBoxes.\nLength of _timeBoxedTrafficSignals[i].CurrentDirection = Length of _signals.\nLength _timeBoxedTrafficSignals[i].CurrentDirection[j] = number of Directions currently active on signal i.\nE.g. In timebox 0, signals 1 & 3 can have forward and right active for each,meanwhile signals 2 & 4 are off. In timebox 1, the signals change.")]
    public List<TrafficSignalsCollective> TimeBoxedTrafficSignals = null;


    private int _currentTimeboxIndex = 0;

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

        CycleTimeBox();
    }

    private void AssignSignalsToIndicators()
    {
        for (int i = 0; i < Signals.Length; i++)
        {
            Signals[i].SignalCollider.AssignSignalController(Signals[i].Signal);
        }
    }

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
                StartCoroutine(CycleTimeboxPostInterval());
                continue;
            }

            Signals[i].Signal.SwitchSignal(TrafficSignalStateID.Green, TimeBoxedTrafficSignals[_currentTimeboxIndex].Signals[i].CurrentDirections);
        }

        StartCoroutine(CycleTimeboxPostInterval());
    }

    private IEnumerator CycleTimeboxPostInterval()
    {
        yield return new WaitForSeconds(IntervalPerSignalInSeconds);
        _currentTimeboxIndex++;
        CycleTimeBox();
    }


#if UNITY_EDITOR
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

[Serializable]
public struct TrafficSignalsCollective
{
    [SerializeField]
    public SignalDirectionsCollective[] Signals;
}

[Serializable]
public struct SignalDirectionsCollective
{
    public SignalDirectionID[] CurrentDirections;
}

[Serializable]
public struct SignalObjects
{
    public TrafficSignalController Signal;
    public SignalIndicator SignalCollider;
}
