using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FixedJoint))]
public class VehiclePartBehaviour : MonoBehaviour
{
    [Header("Stats")]
    [Space(5)]
    
    public VehiclePartStats partStats = null;
    public PartType partType = PartType.Cab;
    [SerializeField]
    Rigidbody rigidBody;
    [SerializeField]
    VehicleBehaviour currentBase = null;
    [Header("Weapons")]
    [Space(5)]
    List<GameObject> weaponHolders = new List<GameObject>();
    List<BasicTurretBehaviour> basicTurrets = new List<BasicTurretBehaviour>();
    private void OnValidate()
    {
        UpdateComp();  
    }
    private void OnEnable()
    {
        UpdateComp();
    }
    void UpdateComp()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
        if (rigidBody != null && partStats != null)
        {
            rigidBody.mass = partStats.partMass;
        }
        if(partStats != null)
        {
            partType = partStats.partType;
        }
        if (currentBase == null) 
        {
            currentBase = GetComponentInParent<VehicleBehaviour>();
        }
        if (currentBase != null)
        {
            if (partType == PartType.Cab)
            {
                currentBase.currentVehicleCab = this.gameObject;
            }
            else if (partType == PartType.Body)
            {
                currentBase.currentVehicleBody = this.gameObject;
            }
        }
        if (currentBase != null)
        {
            GetComponent<FixedJoint>().connectedBody = currentBase.rigidBody;
        }
        if (weaponHolders.Count != 0)
        {
            foreach (GameObject wHolder in weaponHolders)
            {
                if (wHolder.GetComponent<BasicTurretBehaviour>() != null 
                    &&  !basicTurrets.Contains(wHolder.GetComponent<BasicTurretBehaviour>()))
                {
                    basicTurrets.Add(wHolder.GetComponent<BasicTurretBehaviour>());
                }
            }            
        }
    }
}
