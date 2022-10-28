using UnityEngine;
using System;

namespace TrafficSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class SignalIndicator : MonoBehaviour
    {
        [SerializeField]
        private Collider _collider = null;

        private TrafficSignalController _signalController = null;

        public TrafficSignalStateID CurrentSignalState = TrafficSignalStateID.None;
        public SignalDirectionID[] CurrentActiveDirections = null;

        public Action SignalChanged = null;

        private SignalManager _signalManager = null;
        public SignalManager ParentSignalManager => _signalManager;

#if UNITY_EDITOR
        public Collider SignalInteractionCollider
        {
            get { return _collider; }
            set { _collider = value; }
        }
#endif

        public void AssignSignalController(TrafficSignalController controller)
        {
            _signalController = controller;
            if (_signalController == null)
            {
                enabled = false;
                gameObject.SetActive(false);
                return;
            }

            _signalController.SignalChanged += OnSignalChanged;

            CurrentSignalState = _signalController.GetCurrentSignalState();
            CurrentActiveDirections = _signalController.GetCurrentActiveSignalDirections();
        }

        private void Awake()
        {
            _signalManager = GetComponentInParent<SignalManager>();
            if (_signalManager == null)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnSignalChanged()
        {
            CurrentSignalState = _signalController.GetCurrentSignalState();
            CurrentActiveDirections = _signalController.GetCurrentActiveSignalDirections();

            SignalChanged?.Invoke();
        }

        private void OnDestroy()
        {
            _signalController.SignalChanged -= OnSignalChanged;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }

            if (_collider == null || _collider.isTrigger)
            {
                return;
            }

            _collider.isTrigger = true;
        }
#endif


    }
}