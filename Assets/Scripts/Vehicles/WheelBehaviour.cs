using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
    public WheelCollider currentWheelColl = null;
    public Collider baseCollider = null;
    public bool isTurningWheel = true;
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
        if (baseCollider != null)
        {
            return;
        }
        else
        {
            baseCollider = GetComponentInParent<VehicleBehaviour>().GetComponent<Collider>();
        }

    }
    void IgnoreParent()
    {
        Physics.IgnoreCollision(currentWheelColl, baseCollider);
    }
}
