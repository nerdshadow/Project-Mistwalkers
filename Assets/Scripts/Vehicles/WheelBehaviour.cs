using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
    public WheelCollider currentWheelColl = null;
    public List<Collider> vehicleColliders = null;
    public bool isTurningWheel = true;
    private void OnDisable()
    {
        vehicleColliders.Clear();
    }
    private void OnEnable()
    {
        TryGetWheelCollInParent();
        //currentWheelColl.ConfigureVehicleSubsteps(2, 2, 2);
        IgnoreParent();
    }

    //private void FixedUpdate()
    //{
    //    UpdateWheelVFX();
    //}
    private void Update()
    {
        UpdateWheelVFX();
    }
    void UpdateWheelVFX()
    {
        if (currentWheelColl != null)
        {
            currentWheelColl.GetWorldPose(out var pos, out var rot);
            
            transform.position = pos;
            transform.rotation = rot;
        }
    }
    void TryGetWheelCollInParent()
    {
        if (currentWheelColl != null)
        {
            return;
        }
        else 
        {
            currentWheelColl = GetComponentInParent<WheelCollider>();
        }

        vehicleColliders.Add(GetComponentInParent<VehicleBehaviour>().GetComponent<Collider>());
        vehicleColliders.AddRange(GetComponentInParent<VehicleBehaviour>().GetComponentsInChildren<Collider>());            

    }
    void IgnoreParent()
    {
        foreach (Collider collider in vehicleColliders) 
        {
            Physics.IgnoreCollision(currentWheelColl, collider);
        }
    }
}
