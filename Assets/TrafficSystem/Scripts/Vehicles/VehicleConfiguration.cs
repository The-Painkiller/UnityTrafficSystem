using UnityEngine;

namespace TrafficSystem
{
    /// <summary>
    /// Sets configuration of a vehicle like their max speed, and navmesh travelling values.
    /// Different vehicles can have different configurations.
    /// </summary>
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "TrafficSystem/New Vehicle Configuration")]
    public class VehicleConfiguration : ScriptableObject
    {
        public float NormalMovementSpeed = 50f;
        public float TurningSpeed = 15f;
        public float Acceleration = 70f;
        public float AngularSpeed = 200f;
        public float SpeedSwitchTimeStep = 0.1f;
        public float TurningPointDistance = 1f;

    }
}