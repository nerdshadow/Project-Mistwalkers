using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleStats", menuName = "SO/CreateVehicleStatsSO")]
public class VehicleStats : ScriptableObject
{
    public float maxSpeed = 60f;
    public float forwardTorque = 100f;
    public float backwardTorque = 100f;
    public float brakeForce = 80f;
    public float maxTurnAngle = 15f;
}
