using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "TrafficSystem/New Vehicle Configuration")]
public class VehicleConfiguration : ScriptableObject
{
    public float NormalMovementSpeed = 50f;
    public float TurningSpeed = 15f;
    public float Acceleration = 70f;
    public float AngularSpeed = 200f;
    public float SpeedSwitchTimeStep = 0.1f;
    public float SpeedSwitchTresholdDistance = 1f;

}
