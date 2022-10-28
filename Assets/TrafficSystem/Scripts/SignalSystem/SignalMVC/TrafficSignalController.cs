namespace TrafficSystem
{
    using System;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Controller class for traffic signals.
    /// Depends on the View class.
    /// It represents one signal with multiple signal blocks.
    /// 
    /// Ex. 1 Traffic Signal controller can display Red, Yellow and Forward direction signal.
    /// </summary>
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

        /// <summary>
        /// Changes the signal to another state.
        /// First changes the signal to yellow, then 1 second later, changes it to any given state, Red or Green.
        /// </summary>
        /// <param name="targetSignalState"></param>
        /// <param name="directions"></param>
        /// <returns></returns>
        private IEnumerator ChangeSignal(TrafficSignalStateID targetSignalState, SignalDirectionID[] directions = null)
        {
            _signalLocked = true;
            _model.CurrentSignalState = TrafficSignalStateID.Yellow;
            _view.SwitchSignal(TrafficSignalStateID.Yellow);

            yield return new WaitForSeconds(1f);

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

        /// <summary>
        /// This will bypass the ChangeSignal coroutine and immediately change the signal to Red.
        /// </summary>
        private void ResetToRedImmediate()
        {
            _model.CurrentActiveDirections = null;
            _model.CurrentSignalState = TrafficSignalStateID.Red;
            _view.SwitchSignal(TrafficSignalStateID.Red);
        }

        /// <summary>
        /// Called publicly by the manager class, this sets the desired state
        /// and calls the ChangeSignal coroutine.
        /// </summary>
        /// <param name="signalState">Desired signal state.</param>
        /// <param name="direction">Desired number of directions to allow. If the state is Red, this could be Null or SignalDirectionID.None.</param>
        public void SwitchSignal(TrafficSignalStateID signalState, SignalDirectionID[] direction = null)
        {
            if (_model.CurrentSignalState == signalState && _model.CurrentActiveDirections == direction)
            {
                return;
            }

            if (_signalLocked)
                return;

            _model.CurrentSignalState = signalState;
            _model.CurrentActiveDirections = direction;

            StartCoroutine(ChangeSignal(signalState, direction));
        }

        /// <summary>
        /// Gets current Signal State.
        /// </summary>
        /// <returns></returns>
        public TrafficSignalStateID GetCurrentSignalState()
        {
            return _model.CurrentSignalState;
        }

        /// <summary>
        /// Gets the number of directions that are currently active.
        /// If the signal state is Red, this could return Null or SignalDirectionID.None.
        /// </summary>
        /// <returns></returns>
        public SignalDirectionID[] GetCurrentActiveSignalDirections()
        {
            return _model.CurrentActiveDirections;
        }
    }
}