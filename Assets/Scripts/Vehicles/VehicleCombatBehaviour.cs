using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
public class VehicleCombatBehaviour : MonoBehaviour, IIDamageable
{
    [Header("Stats")]
    VehicleMovement vehicleMovement;
    VehicleBaseStats vehicleBaseStats;
    [SerializeField]
    int currentHealth = 1;
    private void Start()
    {
        vehicleMovement = GetComponent<VehicleMovement>();
        vehicleBaseStats = vehicleMovement.currentVehicleStats;
        ResetHealth();
    }
    public void DoDamage(int _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0)
            Die();
    }
    void Die()
    {
        Debug.Log(this.gameObject.name + " died");
        vehicleMovement.StopVehicle();
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            if (rb.GetComponent<FixedJoint>() != null)
            {
                if (rb.GetComponent<VehiclePartBehaviour>() != null)
                {
                    //if (rb.transform.parent != null)
                    //{
                    //    GameObject potParent = rb.transform.parent.gameObject;
                    //    potParent.transform.transform.DetachChildren();
                    //    Destroy(potParent);
                    //}
                    Destroy(rb.GetComponent<VehiclePartBehaviour>());
                    //rb.mass *= 0.5f;
                }
                Destroy(rb.GetComponent<FixedJoint>());
                rb.transform.SetParent(null, true);
                if(rb.GetComponent<Collider>() != null)
                    rb.GetComponent<Collider>().isTrigger = false;
            }
            //if (rb != null && rb is WheelCollider)
            //{
            //    Destroy(rb.transform.parent.gameObject);
            //}
            rb.drag = 0.8f;
            rb.angularDrag = 0.1f;
        }
        //gameObject.transform.DetachChildren();
        //Add explosion for effect
        Vector3 pointOfEffect = transform.position;
        pointOfEffect = GetComponent<Rigidbody>().worldCenterOfMass;
        pointOfEffect = new Vector3(pointOfEffect.x + 1 * Random.Range(-1f, 1f),
                                    pointOfEffect.y + 1 * Random.Range(-1f, 1f),
                                    pointOfEffect.z + 1 * Random.Range(-1f, 1f));
        //test
        Collider[] targetColliders = Physics.OverlapSphere(pointOfEffect, testExplRad);
        foreach (Collider collider in targetColliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log(rb.name);
                rb.AddExplosionForce(testExplForce * rb.mass, pointOfEffect, testExplRad, testExplUpForce * rb.mass);
            }
        }
    }
    public void ResetHealth()
    {
        currentHealth = vehicleBaseStats.vehicleBaseHealth
        + vehicleMovement.cabHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth
        + vehicleMovement.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth;
    }
    [Space(10)]
    [Header("Dev")]
    [SerializeField]
    int testDamage = 10;
    [SerializeField]
    float testExplRad = 10f;
    [SerializeField]
    float testExplForce = 1000f;
    [SerializeField]
    float testExplUpForce = 3f;
    [ContextMenu("Take dev damage")]
    void TakeDevDamage()
    {
        DoDamage(testDamage);
    }
}
