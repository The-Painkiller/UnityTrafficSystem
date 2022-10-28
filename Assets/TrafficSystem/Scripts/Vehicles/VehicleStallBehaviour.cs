using System;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Stalling behaviour handling for the vehicle.
    /// What to do if a vehicle doesn't run for more than a set number of seconds.
    /// </summary>
    public class VehicleStallBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float _selfDestructTimer = 30f;

        private float _currentTimer = 0f;
        private bool _isActive = false;

        public Action SelfDestruct = null;

        /// <summary>
        /// Gets activated as soon as the vehicle stops.
        /// (A glorified bug fixing! "Just reset it!")
        /// </summary>
        /// <param name="activate"></param>
        public void Activate(bool activate)
        {
            _isActive = activate;

            if (_isActive)
            {
                _currentTimer = 0;
            }
        }

        private void Update()
        {
            if (!_isActive)
                return;

            _currentTimer += Time.deltaTime;

            if (_currentTimer >= _selfDestructTimer)
            {
                SelfDestruct?.Invoke();
                _isActive = false;
            }
        }
    }
}