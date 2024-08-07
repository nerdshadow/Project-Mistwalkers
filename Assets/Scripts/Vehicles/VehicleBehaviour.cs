using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShowCenterOfMass))]
public class VehicleBehaviour : MonoBehaviour
{
    #region Wheels
    [Header("Wheels")]
    [Space(5)]
    public bool isAllWheelDrive = false;
    [SerializeField]
    VehicleBaseStats currentVehicleStats = null;
    [SerializeField]
    List<WheelCollider> turnWheels = new List<WheelCollider>();
    [SerializeField]
    List<WheelCollider> staticWheels = new List<WheelCollider>();
    #endregion Wheels

    #region Stats
    [Header("Stats")]
    [Space(5)]
    public bool canDrive = true;
    public bool useForce = false;
    public bool followPointList = false;
    [SerializeField]
    float currentTorque = 0f;
    [SerializeField]
    float currentBrakeTorque = 0f;
    [SerializeField]
    float currentTurnAngle = 0f;
    [SerializeField]
    GameObject currentCheckpoint;
    [SerializeField]
    List<GameObject> checkpoints = new List<GameObject>();
    Vector3 mainDifference;
    #endregion Stats

    #region Parts
    [Header("Parts")]
    [Space(5)]
    public GameObject vehicleBase;
    [SerializeField]
    GameObject cabHolder;
    public GameObject vehicleCab;
    [SerializeField]
    GameObject bodyHolder;
    public GameObject vehicleBody;
    #endregion Parts
    private void Start()
    {
        FindParts();
        FindWheels();
        if(followPointList == true)
            currentCheckpoint = checkpoints[0];
    }
    private void FixedUpdate()
    {
        //TryMove();
        if (canDrive == false)
            return;
        FollowCheckpoints();

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
    public void FindParts()
    {
        vehicleBase = this.gameObject;
        if (cabHolder != null && cabHolder.transform.GetChild(0).gameObject != null)
        {
            vehicleCab = cabHolder.transform.GetChild(0).gameObject;
        }
        if (bodyHolder != null && bodyHolder.transform.GetChild(0).gameObject != null)
        {
            vehicleBody = bodyHolder.transform.GetChild(0).gameObject;
        }
    }
    void FollowCheckpoints()
    {
        if (currentCheckpoint == null)
            return;
        CheckDirections();
        CheckDistances();
        TryToMoveToCheckpoint();
    }
    //TODO slow change of values
    void TryToMoveToCheckpoint()
    {
        //move vehicle
        if (zDistance > 5f)
        {
            if (vehicleIsBehind == true)
            {
                Move(Movement.forward);
                //currentTorque = currentVehicleStats.forwardTorque;
            }
            else
            {
                Move(Movement.backward);
            }
        }
        else
        {
            //int n = checkpoints.IndexOf(currentCheckpoint);
            if (followPointList == true && (checkpoints.IndexOf(currentCheckpoint) < checkpoints.Count - 1))
                currentCheckpoint = checkpoints[(checkpoints.IndexOf(currentCheckpoint) + 1)];
            else
            {
                //if(followPointList == true)
                //    Move(Movement.stop);
                //else
                //    Move(Movement.release);
                Move(Movement.stop);
            }
        }

        Vector3 targetDir = currentCheckpoint.transform.position - transform.position;
        float angleBetween = 0f;
        if (vehicleIsBehind == true)
        {
            angleBetween = Vector3.Angle(targetDir, transform.forward);
        }
        else
        {
            angleBetween = Vector3.Angle(targetDir, -transform.forward);
        }
        //Debug.Log("Angle is " + angleBetween);
        //turn vehicle 
        if (angleBetween >= 5f)
        {
            float targetAngle = 0f;
            if (vehicleIsLeft == true)
            {
                //currentTurnAngle = -currentVehicleStats.maxTurnAngle;
                targetAngle = (vehicleIsBehind == true) ? -currentVehicleStats.maxTurnAngle : currentVehicleStats.maxTurnAngle;
            }
            else
            {
                //currentTurnAngle = currentVehicleStats.maxTurnAngle;
                targetAngle = (vehicleIsBehind == true) ? currentVehicleStats.maxTurnAngle : -currentVehicleStats.maxTurnAngle;
            }
            Turn(targetAngle, angleBetween);
        }
        else
        {
            //currentTurnAngle = 0f;
            Turn(0f, angleBetween);
        }

        //Check current values
        currentVelocity = GetComponent<Rigidbody>().velocity.z;
        currentSpeed = currentVelocity * 3.6f;
        currentSpeed = Mathf.Abs(currentSpeed);


        if (staticWheels.Count > 0)
        {
            currentRpm = staticWheels[0].rpm;
            //currentBrakeTorque = staticWheels[0].brakeTorque;
        }
        else if (turnWheels.Count > 0)
        {
            currentRpm = turnWheels[0].rpm;
            //currentBrakeTorque = turnWheels[0].brakeTorque;
        }

        //Apply torque and angle
        foreach (var wheel in staticWheels)
        {
            if (wheel.rpm < currentVehicleStats.maxRpm && currentSpeed < currentVehicleStats.maxSpeed)
            {
                wheel.motorTorque = currentTorque;
            }
            else wheel.motorTorque = 0;

            wheel.brakeTorque = currentBrakeTorque;
        }
        if (isAllWheelDrive == true)
        {
            foreach (var wheel in turnWheels)
            {
                if (wheel.rpm < currentVehicleStats.maxRpm && currentSpeed < currentVehicleStats.maxSpeed)
                {
                    wheel.motorTorque = currentTorque;
                }
                else wheel.motorTorque = 0;

                wheel.brakeTorque = currentBrakeTorque;
            }
        }
        foreach (var wheel in turnWheels)
        {
            wheel.steerAngle = currentTurnAngle;
        }
    }
    enum Movement
    {
        backward = -1,
        stop = 0,
        forward = 1,
        release = 2
    }
    void Move(Movement _direction)
    {
        switch (_direction)
        {
            case Movement.backward:
                DecreaseTorque();
                break;
            case Movement.stop:
                BreakTorque();
                break;
            case Movement.forward:
                IncreaseTorque();
                break;
            case Movement.release:
                ReleaseTorque();
                break;
            default:
                currentTorque = 0f;
                break;
        }
    }
    [SerializeField]
    float accelerationSpeed = 2f;
    [SerializeField]
    float decelerationSpeed = 2f;
    void IncreaseTorque()
    {
        currentBrakeTorque = 0;
        if (currentTorque < currentVehicleStats.forwardTorque)
        {
            currentTorque += accelerationSpeed;
            if (currentTorque > currentVehicleStats.forwardTorque)
                currentTorque = currentVehicleStats.forwardTorque;
        }
    }
    void DecreaseTorque()
    {
        currentBrakeTorque = 0;
        if (currentTorque > -currentVehicleStats.backwardTorque)
        {
            currentTorque -= decelerationSpeed;
            if (currentTorque < -currentVehicleStats.backwardTorque)
                currentTorque = -currentVehicleStats.backwardTorque;
        }
    }
    void BreakTorque()
    {
        //if (currentTorque > 0)
        //{
        //    currentTorque -= decelerationSpeed * 2;
        //    if (currentTorque < 0)
        //        currentTorque = 0;
        //}
        //else { currentTorque = 0; }
        currentTorque = 0;

        currentBrakeTorque = currentVehicleStats.brakeTorque;
    }
    void ReleaseTorque()
    {
        currentBrakeTorque = 0;
        if (currentTorque > 0)
        {
            currentTorque -= decelerationSpeed;
            if (currentTorque < 0)
                currentTorque = 0;
        }
    }
    void CheckDirections()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = Vector3.Normalize(currentCheckpoint.transform.position - transform.position);

        if (Vector3.Dot(forward, toOther) < 0)
        {
            vehicleIsBehind = false;
            //print("Car is forward");
        }
        else if (Vector3.Dot(forward, toOther) > 0)
        {
            vehicleIsBehind = true;
            //print("Car is behind");
        }
        else
        {
            //print("Center");
        }
        Vector3 right = transform.TransformDirection(Vector3.right);
        toOther = Vector3.Normalize(currentCheckpoint.transform.position - transform.position);

        if (Vector3.Dot(right, toOther) < 0)
        {
            //print("Left");
            vehicleIsLeft = true;
        }
        else if (Vector3.Dot(right, toOther) > 0)
        {
            //print("Right");
            vehicleIsLeft = false;
        }
        else
        {
            //print("Center");
        }
    }
    void CheckDistances()
    {
        mainDifference = currentCheckpoint.transform.position - transform.position;
        xDistance = Mathf.Abs(mainDifference.x);
        //Debug.Log("xDist = " + xDistance);
        var distanceInY = Mathf.Abs(mainDifference.y);
        zDistance = Mathf.Abs(mainDifference.z);
        //Debug.Log("zDist = " + zDistance);
    }

    void Turn(float _targetAngle, float _angleBetween)
    {
        if (vehicleIsBehind != true)
            _targetAngle *= -1f;
        if (currentTurnAngle > _targetAngle)
        {
            currentTurnAngle -= currentVehicleStats.turnSpeed * ((_angleBetween / 180f) * 100f);
            if (_targetAngle > currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }
        else if (currentTurnAngle < _targetAngle)
        {
            currentTurnAngle += currentVehicleStats.turnSpeed * ((_angleBetween / 180f) * 100f);
            if (_targetAngle < currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }
    }
    #region Checks
    [Space(10)]
    [Header("Checks")]
    [Space(5)]
    [SerializeField]
    bool vehicleIsBehind = false;
    [SerializeField]
    bool vehicleIsLeft = false;
    [SerializeField]
    float zDistance = 0f;
    [SerializeField]
    float xDistance = 0f;
    [SerializeField]
    float currentVelocity;
    [SerializeField]
    float currentSpeed;
    [SerializeField]
    float velocityLimit;
    [SerializeField]
    float currentRpm;
    #endregion Checks
    #region Dev
    [Header("Dev")]
    [Space(5)]
    [SerializeField]
    float pushpower = 1000f;
    [ContextMenu("Push")]
    void PushCar()
    {
        GetComponent<Rigidbody>().AddRelativeForce(transform.forward * pushpower, ForceMode.Acceleration);
    }
    #endregion Dev
}
