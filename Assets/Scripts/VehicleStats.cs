using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleStats", menuName = "SO/CreateVehicleStatsSO")]
public class VehicleStats : ScriptableObject
{
    public float maxSpeed = 60f;
    public float maxRpm = 500f;
    public float forwardTorque = 100f;
    public float backwardTorque = 100f;
    public float brakeTorque = 80f;
    public float maxTurnAngle = 15f;
    public float turnSpeed = 1f;
}
