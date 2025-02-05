using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowType
{
    Formation = 0,
    EndPoint = 1
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShowCenterOfMass))]
public class VehicleMovement : MonoBehaviour
{
    #region Wheels
    [Header("Wheels")]
    [Space(5)]
    public bool isAllWheelDrive = false;
    [SerializeField]
    public VehicleBaseStats currentVehicleStats = null;
    [SerializeField]
    List<WheelCollider> turnWheels = new List<WheelCollider>();
    [SerializeField]
    List<WheelCollider> staticWheels = new List<WheelCollider>();
    #endregion Wheels

    #region Stats
    [Header("Stats")]
    [Space(5)]
    public bool playerDirectControl = false;
    public bool canDrive = true;
    public bool forceStop = false;
    public bool useForce = false;    
    public bool followSquadPosition = false;
    public float stopDistance = 5f;
    public float followDistance = 0.5f;
    public float minAngleForTurn = 5f;
    [SerializeField]
    float currentTorque = 0f;
    [SerializeField]
    float currentBrakeTorque = 0f;
    [SerializeField]
    float currentTurnAngle = 0f;    
    public Rigidbody rigidBody;
    [SerializeField]
    public Transform currentMoveTarget;
    Vector3 mainDifference;
    #endregion Stats

    #region Parts
    [Header("Parts")]
    [Space(5)]
    public GameObject currentVehicleBase;    
    public GameObject cabHolder;
    public VehiclePartBehaviour cabBeh;
    public GameObject currentVehicleCab;
    public VehiclePartBehaviour bodyBeh;
    public GameObject bodyHolder;
    public GameObject currentVehicleBody;
    #endregion Parts
    private void OnValidate()
    {
        if(!gameObject.activeInHierarchy || Application.isPlaying == true)
            return;

        //SerializeVehicle();
    }
    private void OnEnable()
    {
        SerializeVehicle();
        testMaxSpeed = currentVehicleStats.maxSpeed;
    }
    private void FixedUpdate()
    {
        if (canDrive == false || playerDirectControl == true)
        {
            return;
        }
        FollowTarget();
    }
    #region VehicleSer
    public void FindWheels()
    {
        if (turnWheels.Count + staticWheels.Count != 0)
        {
            turnWheels = new List<WheelCollider> ();
            staticWheels = new List<WheelCollider>();
        }        

        foreach (var potWheel in GetComponentsInChildren<WheelBehaviour>())
        {
            potWheel.ReManageWheelColliders();
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
    [ContextMenu("Update Components")]
    void UpdateComp()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = currentVehicleStats.vehicleBaseMass;

    }
    int AmountOfTopChildren(Transform _parent)
    {
        int amount = 0;
        for (int i = 0; i < _parent.childCount; i++)
        {
            if(_parent.GetChild(i) == null)
                break;
            //Debug.Log(_parent.GetChild(i) + " is " + amount + " child" + " in " + transform.root.name);
            amount++;
        }
        return amount;
    }
    [ContextMenu("Update Parts")]
    public void UpdateParts()
    {
        currentVehicleBase = this.gameObject;

        if (cabHolder != null)
        {
            int amountOfCabChild = AmountOfTopChildren(cabHolder.transform);

            if (amountOfCabChild == 1)
            {
                currentVehicleCab = cabHolder.transform.GetChild(0).gameObject;
                cabBeh = currentVehicleCab.GetComponent<VehiclePartBehaviour>();
            }
            else if (amountOfCabChild > 1)
            {
                currentVehicleCab = cabHolder.transform.GetChild(0).gameObject;
                cabBeh = currentVehicleCab.GetComponent<VehiclePartBehaviour>();
                Debug.Log("Too many parts for " + cabHolder.name + " in " + transform.root.name);
            }
        }

        if (bodyHolder != null)
        {
            int amountOfBodyChild = AmountOfTopChildren(bodyHolder.transform);

            if (amountOfBodyChild == 1)
            {
                currentVehicleBody = bodyHolder.transform.GetChild(0).gameObject;
                bodyBeh = currentVehicleBody.GetComponent<VehiclePartBehaviour>();
            }
            else if (amountOfBodyChild > 1)
            {
                currentVehicleBody = bodyHolder.transform.GetChild(0).gameObject;
                bodyBeh = currentVehicleBody.GetComponent<VehiclePartBehaviour>();
                Debug.Log("Too many parts for " + bodyHolder.name + " in " + transform.root.name);
            }
        }

        
    }
    public void SerializeVehicle()
    { 
        UpdateComp();
        UpdateParts();
        FindWheels();
    }
    #endregion VehicleSer
    public void StopVehicle()
    {
        canDrive = true;
        forceStop = true;
    }
    public void MoveVehicle()
    {
        canDrive = true;
        forceStop = false;
    }
    void FollowTarget()
    {
        if (currentMoveTarget == null || forceStop == true)
        {
            TryToBreakStop(true);
            return;
        }
        
        CheckDirections();
        CheckDistances();
        if (followSquadPosition == true)
        {
            MoveToSquadPos();
            ManageWheelsSquad();
        }
        else
        {
            MoveToTarget();        
            ManageWheels();
        }
    }
    void TryToBreakStop(bool resetWheels)
    {
        Move(Movement.stop);
        if(resetWheels == true)
            Turn(0f, 0f);
    }
    void MoveToTarget()
    {
        //move vehicle
        if (zDistance > stopDistance)
        {
            if (vehicleIsBehind == true)
            {
                Move(Movement.forward);
            }
            else
            {
                if (followSquadPosition == true)
                    Move(Movement.release);
                else
                    Move(Movement.backward);                    
            }
        }
        else
        {
            if(followSquadPosition == true)
                Move(Movement.release);
            else
                Move(Movement.stop);
        }
        //Turn
        Vector3 targetDir = currentMoveTarget.transform.position - transform.position;
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
        if (angleBetween >= minAngleForTurn)
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
    }
    [SerializeField]
    float testMaxSpeed;
    void MoveToSquadPos()
    {
        //move vehicle
        if (zDistance > followDistance)
        {
            if (vehicleIsBehind == true)
            {
                Move(Movement.forward);
                if (zDistance >= 3f && testMaxSpeed <= currentVehicleStats.maxSpeed * 1.5f)
                    testMaxSpeed += 1f * Time.deltaTime;
            }
            else
            {
                Move(Movement.release);
                if (testMaxSpeed != currentVehicleStats.maxSpeed)
                    testMaxSpeed = currentVehicleStats.maxSpeed;
            }
        }
        //Turn
        Vector3 targetDir = currentMoveTarget.transform.position - transform.position;
        float angleBetween = 0f;
        if (vehicleIsBehind == false)
        {
            //Turn(0f, angleBetween);
            currentTurnAngle = 0f;
            return;
        }
        if (vehicleIsBehind == true)
        {
            angleBetween = Vector3.Angle(targetDir, transform.forward);
        }
        //Debug.Log("Angle is " + angleBetween);
        //turn vehicle 
        if (angleBetween >= minAngleForTurn)
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
            currentTurnAngle = 0f;
            //Turn(0f, angleBetween);
        }
    }
    public void ManageWheels()
    {
        if(staticWheels.Count == 0 && turnWheels.Count == 00)
            return;

        //Check current values
        currentVelocity = rigidBody.velocity.z;
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
                //if(followSquadPosition == true)
                    wheel.motorTorque = currentTorque;
                //else if(currentSpeed < currentVehicleStats.maxSpeed)
                //    wheel.motorTorque = currentTorque;
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
                    //if (followSquadPosition == true)
                        wheel.motorTorque = currentTorque;
                    //else if (currentSpeed < currentVehicleStats.maxSpeed)
                    //    wheel.motorTorque = currentTorque;
                }
                else wheel.motorTorque = 0;

                //wheel.brakeTorque = currentBrakeTorque;
            }
        }
        foreach (var wheel in turnWheels)
        {
            wheel.steerAngle = currentTurnAngle;
            if(Mathf.Abs(currentSpeed) <= 0.01f)
                wheel.brakeTorque = currentBrakeTorque;
        }
    }
    void ManageWheelsSquad()
    {
        if (staticWheels.Count == 0 && turnWheels.Count == 00)
            return;

        //Check current values
        currentVelocity = rigidBody.velocity.z;
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
            if (wheel.rpm < currentVehicleStats.maxRpm && currentSpeed < testMaxSpeed)
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
                if (wheel.rpm < currentVehicleStats.maxRpm && currentSpeed < testMaxSpeed)
                {
                    wheel.motorTorque = currentTorque;
                }
                else wheel.motorTorque = 0;

                //wheel.brakeTorque = currentBrakeTorque;
            }
        }
        foreach (var wheel in turnWheels)
        {
            wheel.steerAngle = currentTurnAngle;
            if (Mathf.Abs(currentSpeed) <= 0.01f)
                wheel.brakeTorque = currentBrakeTorque;
        }
    }
    public enum Movement
    {
        backward = -1,
        stop = 0,
        forward = 1,
        release = 2
    }
    public void Move(Movement _direction)
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
        Debug.Log(gameObject.name + " increasing torque");
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
        Debug.Log(gameObject.name + " decreasing torque");
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
        Debug.Log(gameObject.name + " breaking torque");
        //if (currentTorque > 0)
        //{
        //    currentTorque -= decelerationSpeed * 2;
        //    if (currentTorque < 0)
        //        currentTorque = 0;
        //}
        //else 
        //{ 
        //    currentTorque = 0;
        //}

        if (Mathf.Abs(currentSpeed) <= 0.01f)
        {
            currentTorque = 0f;
            currentBrakeTorque = Mathf.Infinity;
        }
        else
        {
            currentTorque = 0f;
            currentBrakeTorque = currentVehicleStats.brakeTorque;
        }
    }
    void ReleaseTorque()
    {
        Debug.Log(gameObject.name + " releasing torque");
        if (currentTorque > 0)
        {
            currentTorque -= decelerationSpeed;
            if (currentTorque < 0)
                currentTorque = 0;
            currentBrakeTorque += accelerationSpeed * 2;
            if(currentBrakeTorque > currentVehicleStats.brakeTorque)
                currentBrakeTorque = currentVehicleStats.brakeTorque;
        }
    }    
    void Turn(float _targetAngle, float _angleBetween)
    {
        if (vehicleIsBehind != true)
            _targetAngle *= -1f;
        if (currentTurnAngle > _targetAngle)
        {
            currentTurnAngle -= currentVehicleStats.turnSpeed * ((_angleBetween / 180f) * 100f) /** Time.deltaTime*/;
            if (_targetAngle > currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }
        else if (currentTurnAngle < _targetAngle)
        {
            currentTurnAngle += currentVehicleStats.turnSpeed * ((_angleBetween / 180f) * 100f) /** Time.deltaTime*/;
            if (_targetAngle < currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }        
    }
    public enum TurnDir
    {
        Left = -1,
        NoTurn = 0,
        Right = 1
    }
    public void Turn(TurnDir turnDir)
    {
        float _targetAngle = 0f;
        switch (turnDir)
        {
            case TurnDir.Left:
                _targetAngle = -currentVehicleStats.maxTurnAngle;
                break;
            case TurnDir.NoTurn:
                _targetAngle = 0f;
                break;
            case TurnDir.Right:
                _targetAngle = currentVehicleStats.maxTurnAngle;
                break;
            default:
                break;
        }
        if (currentTurnAngle == _targetAngle)
            return;
        if (currentTurnAngle > _targetAngle)
        {
            currentTurnAngle -= currentVehicleStats.turnSpeed /** Time.deltaTime*/;
            if (_targetAngle > currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }
        else if (currentTurnAngle < _targetAngle)
        {
            currentTurnAngle += currentVehicleStats.turnSpeed /** Time.deltaTime*/;
            if (_targetAngle < currentTurnAngle)
                currentTurnAngle = _targetAngle;
        }        
    }
    #region Navigation
    public void ChangeFollowTarget(Transform _nextTarget)
    {
        if (_nextTarget == null)
        {
            Debug.LogWarning("FollowLeader squadLeader is null");
            return;
        }

        currentMoveTarget = _nextTarget;
    }
    void CheckDirections()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = Vector3.Normalize(currentMoveTarget.transform.position - transform.position);

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
        toOther = Vector3.Normalize(currentMoveTarget.transform.position - transform.position);

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
        mainDifference = currentMoveTarget.transform.position - transform.position;
        xDistance = Mathf.Abs(mainDifference.x);
        //Debug.Log("xDist = " + xDistance);
        var distanceInY = Mathf.Abs(mainDifference.y);
        zDistance = Mathf.Abs(mainDifference.z);
        //Debug.Log("zDist = " + zDistance);
    }
    #endregion Navigation
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
        rigidBody.AddRelativeForce(transform.forward * pushpower, ForceMode.Acceleration);
    }
    #endregion Dev
}
