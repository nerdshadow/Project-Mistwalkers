using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleBehaviour : MonoBehaviour
{
    public enum Status
    {
        Idle,
        Forward,
        BackWard,
        Stop
    }
    public Status currentStatus = Status.Idle;
    public enum RStatus
    {
        Idle,
        Left,
        Right,
    }
    public RStatus currentRStatus = RStatus.Idle;
    public bool isAllWheelDrive = false;
    [SerializeField]
    VehicleStats currentVehicleStats = null;
    [SerializeField]
    List<WheelCollider> turnWheels = new List<WheelCollider>();
    [SerializeField]
    List<WheelCollider> staticWheels = new List<WheelCollider>();

    float currentAcceleration = 0f;
    float currentBrakeForce = 0f;
    float currentTurnAngle = 0f;
    private void Start()
    {
        FindWheels();
    }
    void MoveForward()
    {
        currentAcceleration = currentVehicleStats.forwardTorque;
    }
    void MoveBackward()
    {
        currentAcceleration = -currentVehicleStats.backwardTorque;
    }
    void ToBrake()
    {
        currentAcceleration = 0f;
        currentBrakeForce = currentVehicleStats.brakeForce;
    }
    void ToIdle()
    {
        currentAcceleration = 0f;
        currentBrakeForce = 0f;
    }
    void MoveTo(Transform targetPos)
    {
        
    }
    void RotateL()
    {
        currentTurnAngle = -currentVehicleStats.maxTurnAngle;
    }
    void RotateR()
    {
        currentTurnAngle = currentVehicleStats.maxTurnAngle;
    }
    void TryMove()
    {
        foreach (var wheel in staticWheels) { wheel.motorTorque = currentAcceleration; }
        if(isAllWheelDrive == true)
            foreach (var wheel in turnWheels) { wheel.motorTorque = currentAcceleration; }

        foreach (var wheel in staticWheels) { wheel.brakeTorque = currentBrakeForce; }
        foreach (var wheel in turnWheels) 
        { 
            wheel.brakeTorque = currentBrakeForce; 
            wheel.steerAngle = currentTurnAngle;
        }
        currentBrakeForce = 0f;

        switch (currentStatus)
        {
            case Status.Idle:
                ToIdle();
                break;
            case Status.Forward:
                MoveForward();
                break;
            case Status.BackWard:
                MoveBackward();
                break;
            case Status.Stop:
                ToBrake();
                break;
            default:
                Debug.LogWarning("No status");
                ToIdle();
                break;
        }
        switch (currentRStatus)
        {
            case RStatus.Idle:
                currentTurnAngle = 0f;
                break;
            case RStatus.Left:
                RotateL();
                break;
            case RStatus.Right:
                RotateR();
                break;
            default :
                break;
        }
    }
    private void FixedUpdate()
    {
        TryMove();   
    }
    void FindWheels()
    {
        if (turnWheels.Count + staticWheels.Count != 0)
            return;

        foreach (var potWheel in GetComponentsInChildren<WheelBehaviour>()) 
        {
            if (potWheel.isTurningWheel == true)
            {
                turnWheels.Add(potWheel.currentWheelColl);
            }
            else 
            {
                staticWheels.Add(potWheel.currentWheelColl);
            }
        }
    }
}
