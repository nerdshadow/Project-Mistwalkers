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
    public bool forceStop = false;
    public bool useForce = false;
    public bool followPointList = false;
    public float stopDistance = 5f;
    public float minAngleForTurn = 5f;
    [SerializeField]
    float currentTorque = 0f;
    [SerializeField]
    float currentBrakeTorque = 0f;
    [SerializeField]
    float currentTurnAngle = 0f;    
    public Rigidbody rigidBody;
    [SerializeField]
    GameObject currentMoveTarget;
    [SerializeField]
    List<GameObject> moveTargets = new List<GameObject>();
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
        if(!gameObject.activeInHierarchy)
            return;

        SerializeVehicle();
    }
    private void OnEnable()
    {
        SerializeVehicle();
    }
    private void FixedUpdate()
    {
        //TryMove();
        if (canDrive == false)
            return;
        FollowTarget();

    }
    #region VehicleSer
    void FindWheels()
    {
        if (turnWheels.Count + staticWheels.Count != 0)
        {
            turnWheels = new List<WheelCollider> ();
            staticWheels = new List<WheelCollider>();
        }        

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
    [ContextMenu("Update Components")]
    void UpdateComp()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = currentVehicleStats.vehicleBaseMass;

        if (followPointList == true)
            currentMoveTarget = moveTargets[0];
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
            }
            else if (amountOfCabChild > 1)
            {
                currentVehicleCab = cabHolder.transform.GetChild(0).gameObject;
                Debug.Log("Too many parts for " + cabHolder.name + " in " + transform.root.name);
            }
        }

        if (bodyHolder != null)
        {
            int amountOfBodyChild = AmountOfTopChildren(bodyHolder.transform);

            if (amountOfBodyChild == 1)
            {
                currentVehicleBody = bodyHolder.transform.GetChild(0).gameObject;
            }
            else if (amountOfBodyChild > 1)
            {
                currentVehicleBody = bodyHolder.transform.GetChild(0).gameObject;
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
    //[ContextMenu("Reassemble from behs")]
    //void ReassebleParts()
    //{
    //    if (cabBeh != null)
    //    {
    //        if (currentVehicleCab != cabBeh.gameObject)
    //        {
    //            Destroy(currentVehicleCab);
    //            Instantiate(cabBeh.partStats.partPrefab, cabHolder.transform);
    //        }
    //    }
    //}
    #endregion VehicleSer
    void FollowTarget()
    {
        if (currentMoveTarget == null || forceStop == true)
        {
            TryToStop();
        }
        else
        {
            CheckDirections();
            CheckDistances();
            TryToMove();        
        }
        ManageWheels();
    }
    public void TryToStop()
    {
        Move(Movement.stop);
        Turn(0f, 0f);
    }
    public void TryToMove()
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
                Move(Movement.backward);
            }
        }
        else
        {
            if (followPointList == true && (moveTargets.IndexOf(currentMoveTarget) < moveTargets.Count - 1))
                currentMoveTarget = moveTargets[(moveTargets.IndexOf(currentMoveTarget) + 1)];
            else
            {
                //if(followPointList == true)
                //    Move(Movement.stop);
                //else
                //    Move(Movement.release);
                Move(Movement.stop);
            }
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
    void ManageWheels()
    {
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
    public enum Movement
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
        rigidBody.AddRelativeForce(transform.forward * pushpower, ForceMode.Acceleration);
    }
    #endregion Dev
}
