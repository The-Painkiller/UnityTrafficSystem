using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Types of rigid body collisions that can occur on a vehicle.
    /// </summary>
    public enum CollisionTypes
    {
        None,
        Vehicle
    }

    /// <summary>
    /// Detects rigid body collisions on a vehicle.
    /// This includes other vehicles or any obstacles (not implemented yet).
    /// </summary>
    public class VehicleCollisionDetection : MonoBehaviour
    {
        public Action<CollisionTypes> CollisionEncountered;
        private List<Collision> _encounteredCollisions = new List<Collision>();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.isTrigger)
                return;

            if (!_encounteredCollisions.Contains(collision))
            {
                _encounteredCollisions.Add(collision);
            }

            if (collision.gameObject.tag == "Vehicle")
            {
                CollisionEncountered?.Invoke(CollisionTypes.Vehicle);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_encounteredCollisions.Contains(collision))
            {
                _encounteredCollisions.Remove(collision);
            }

            if (_encounteredCollisions.Count == 0)
            {
                CollisionEncountered?.Invoke(CollisionTypes.None);
            }
        }
    }
}