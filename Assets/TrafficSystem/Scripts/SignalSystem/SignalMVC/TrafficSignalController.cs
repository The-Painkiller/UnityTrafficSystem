using System;
using System.Collections;
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

    private bool _signalLocked = false;

    public Action SignalChanged = null;

#if UNITY_EDITOR
    public SignalDirectionID[] SupportedDirections
    {
        get { return _supportedDirections; }
        set { _supportedDirections = value; }
    }

    public int IntervalPerSignal
    {
        get { return _intervalPerSignal; }
        set { _intervalPerSignal = value; }
    }
#endif

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

        ResetToRedImmediate();
    }

    private IEnumerator ChangeSignal(TrafficSignalStateID targetSignalState, SignalDirectionID[] directions = null)
    {
        _signalLocked = true;
        _model.CurrentSignalState = TrafficSignalStateID.Yellow;
        _view.SwitchSignal(TrafficSignalStateID.Yellow);
        
        yield return new WaitForSeconds(0.5f);
        
        _model.CurrentSignalState = targetSignalState;
        _model.CurrentActiveDirections = directions;
        
        switch (targetSignalState)
        {
            case TrafficSignalStateID.Red:
            case TrafficSignalStateID.Yellow:
                _view.SwitchSignal(targetSignalState);
                break;

            case TrafficSignalStateID.Green:
                _view.SwitchSignal(targetSignalState, directions);
                break;
        }

        SignalChanged?.Invoke();
        _signalLocked = false;
    }

    private void ResetToRedImmediate()
    {
        _model.CurrentActiveDirections = null;
        _model.CurrentSignalState = TrafficSignalStateID.Red;
        _view.SwitchSignal(TrafficSignalStateID.Red);
    }

    public void SwitchSignal(TrafficSignalStateID signalState, SignalDirectionID [] direction  = null)
    {
        if (_model.CurrentSignalState == signalState && _model.CurrentActiveDirections == direction)
        {
            SignalChanged?.Invoke();
            return;
        }

        if (_signalLocked)
            return;

        _model.CurrentSignalState = signalState;
        _model.CurrentActiveDirections = direction;

        StartCoroutine(ChangeSignal(signalState, direction));
    }


    public TrafficSignalStateID GetCurrentSignalState()
    {
        return _model.CurrentSignalState;
    }

    public SignalDirectionID[] GetCurrentActiveSignalDirections()
    {
        return _model.CurrentActiveDirections;
    }

    public bool IsSignalActive(SignalDirectionID signal)
    {
        if (_model.CurrentActiveDirections == null)
            return false;

        return _model.CurrentActiveDirections.Contains(signal);
    }
}
