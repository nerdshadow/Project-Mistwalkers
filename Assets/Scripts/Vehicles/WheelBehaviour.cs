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
        //Debug.Log("Wheel disables");
        vehicleColliders = new List<Collider>();
    }
    private void OnEnable()
    {
        //Debug.Log("Wheel enables");
        ReManageWheelColliders();
        //currentWheelColl.ConfigureVehicleSubsteps(2, 2, 2);
    }

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
    [ContextMenu("ReManage Wh Colls")]
    public void ReManageWheelColliders()
    {
        TryGetWheelCollInParent();
        IgnoreParent();
    }
    [ContextMenu("Find colliders")]
    public void TryGetWheelCollInParent()
    {
        if(vehicleColliders.Count > 0)
            vehicleColliders = new List<Collider>();

        if (currentWheelColl == null)
        {
            currentWheelColl = GetComponentInParent<WheelCollider>();
        }
        List<Collider> poTColliders = new List<Collider>();

        poTColliders.Add(transform.root.GetComponent<Collider>());
        poTColliders.AddRange(transform.root.GetComponentsInChildren<Collider>());

        foreach (Collider collider in poTColliders)
        {
            if (collider != null && !vehicleColliders.Contains(collider))
                if(collider != currentWheelColl)
                    vehicleColliders.Add(collider);

        }
    }
    public void IgnoreParent()
    {
        //foreach (Collider collider in vehicleColliders) 
        //{
        //    if(collider == null)
        //        vehicleColliders.Remove(collider);
        //    Physics.IgnoreCollision(currentWheelColl, collider);
        //}

        for (int i = 0; i < vehicleColliders.Count; i++)
        {
            if (vehicleColliders[i] == null)
            {
                vehicleColliders.RemoveAt(i);
                continue;
            }
            Physics.IgnoreCollision(currentWheelColl, vehicleColliders[i]);
        }
    }
}
