using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalManager : MonoBehaviour
{
    [SerializeField]
    private TrafficSignalController[] _signals;

    private void Awake()
    {
        if (_signals == null || _signals.Length == 0)
        {
            enabled = false;
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Red")]
    public void SwitchToRedSignal()
    {
        _signals[0].SwitchSignal(TrafficSignalStateID.Red);
    }

    [ContextMenu("Yellow")]
    public void SwitchToYellowSignal()
    {
        _signals[0].SwitchSignal(TrafficSignalStateID.Yellow);
    }

    [ContextMenu("Green")]
    public void SwitchToGreenSignal()
    {
        _signals[0].SwitchSignal(TrafficSignalStateID.Green, new SignalDirectionID[] { SignalDirectionID.Forward, SignalDirectionID.Left });
    }

}
