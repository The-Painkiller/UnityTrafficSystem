using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrafficSignalView))]
public class TrafficSignalController : MonoBehaviour
{
    [SerializeField]
    private SignalDirectionID[] _supportedDirections;
    [SerializeField]
    private int _intervalPerSignal;

    private TrafficSignalView _view;
    private TrafficSignalModel _model;

    private void Awake()
    {
        _view = GetComponent<TrafficSignalView>();
        if (_supportedDirections == null 
            || _supportedDirections.Length == 0 
            || _view == null)
        {
            enabled = false;
            gameObject.SetActive(false);
        }

        _model = new TrafficSignalModel(_supportedDirections, _intervalPerSignal);

        SwitchSignal(TrafficSignalStateID.Red);
    }

    public void SwitchSignal(TrafficSignalStateID signalState, SignalDirectionID [] direction  = null)
    {
        if (_model.CurrentSignalState == signalState)
            return;

        _model.CurrentSignalState = signalState;
        _model.CurrentActiveDirections = direction;

        switch (signalState)
        {
            case TrafficSignalStateID.Red:
            case TrafficSignalStateID.Yellow:
                _view.SwitchSignal(signalState);
                break;

            case TrafficSignalStateID.Green:
                _view.SwitchSignal(signalState, direction);
                break;
        }
    }

    public TrafficSignalStateID CurrentSignalState()
    {
        return _model.CurrentSignalState;
    }

    public bool IsSignalActive(SignalDirectionID signal)
    {
        if (_model.CurrentActiveDirections == null)
            return false;

        return _model.CurrentActiveDirections.Contains(signal);
    }
}
