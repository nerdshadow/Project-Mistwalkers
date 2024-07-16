using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
    public WheelCollider currentWheelColl = null;
    private void OnEnable()
    {
        TryGetWheelCollInParent();
    }

    private void FixedUpdate()
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
    }
}
