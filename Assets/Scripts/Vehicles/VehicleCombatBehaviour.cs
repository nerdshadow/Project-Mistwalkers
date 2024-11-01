using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
public class VehicleCombatBehaviour : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    VehicleMovement vehicleMovement;
    VehicleBaseStats vehicleBaseStats;

    private void Start()
    {
        vehicleMovement = GetComponent<VehicleMovement>();
        vehicleBaseStats = vehicleMovement.currentVehicleStats;
        InitHealth();
        ResetHealth();

        ManageAllTurrets();
    }
    private void OnEnable()
    {

    }
    private void FixedUpdate()
    {
        HealthConditionsBeh();
    }
    #region Health
    [Space(10)]
    [Header("Health")]
    [SerializeField]
    int maxHealth = 1;
    [SerializeField]
    int currentHealth = 1;
    public bool isDead = false;
    public void DoDamage(int _damage)
    {
        if (isDead == true)
            return;
        currentHealth -= _damage;
        if (currentHealth <= 0)
            Die();
    }
    public void DoDamage(int _damage, Vector3 _dmgPos)
    {

    }
    public void DoDamage(int _damage, Vector3 _dmgPos, Vector3 _dmgVectorForce)
    {

    }
    void Die()
    {
        Debug.Log(this.gameObject.name + " died");
        isDead = true;
        ChangeVehicleMovement();
        //Check left over hp| if a lot of minus hp then blown up vehicle else slown down vehicle with fire in engine
        float leftoverhpMod = (Mathf.Abs((float)currentHealth) / (float)maxHealth);
        Debug.Log(leftoverhpMod);
        if (leftoverhpMod >= 0.4f)
        {
            //blow immid
            Debug.Log("blow");
            BlowVehicle(leftoverhpMod);        
        }
        else
        {
            //slowly destroy vehicle
            Debug.Log("decay");
            KillVehicle();
        }
    }
    void ChangeVehicleMovement()
    {
        vehicleMovement.StopVehicle();

    }
    private void KillVehicle()
    {
        //wait some time
        StartCoroutine(DecayVehicle());
        
    }
    public float timeToDecay = 3f;
    IEnumerator DecayVehicle()
    {
        yield return new WaitForSeconds(timeToDecay);
        BlowVehicle(0.4f);
    }
    void BlowVehicle(float hp)
    {
        float leftoverhpMod = hp;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            if (rb.GetComponent<FixedJoint>() != null)
            {
                if (rb.GetComponent<VehiclePartBehaviour>() != null)
                {                    
                    Destroy(rb.GetComponent<VehiclePartBehaviour>());
                    int r = Random.Range(1, 5);
                    //Debug.Log("r to destr = " + r);
                    if (r == 1)
                    {
                        Destroy(rb.GetComponent<FixedJoint>());
                        rb.transform.SetParent(null, true);
                    }
                }
                else
                {                    
                    Destroy(rb.GetComponent<FixedJoint>());
                    rb.transform.SetParent(null, true);
                    if (rb.GetComponent<Collider>() != null)
                        rb.GetComponent<Collider>().isTrigger = false;
                }
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
                //Debug.Log(rb.name);
                rb.AddExplosionForce(testExplForce * rb.mass * leftoverhpMod, pointOfEffect, testExplRad, testExplUpForce * rb.mass);
            }
        }
    }
    void HealthConditionsBeh()
    {
        if (currentHealth > (int)(maxHealth * 0.6f))
            return;
        if (currentHealth <= (int)(maxHealth * 0.1f))
        {
            //show smoke and fire
        }
        else if (currentHealth <= (int)(maxHealth * 0.3f))
        {
            //show more smoke
        }
        else if (currentHealth <= (int)(maxHealth * 0.6f))
        {
            //Show smoke from vehicle
        }
    }
    void InitHealth()
    {
        maxHealth = vehicleBaseStats.vehicleBaseHealth
            + vehicleMovement.cabHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth
            + vehicleMovement.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    #endregion Health
    #region Weapons
    [Space(10)]
    [Header("Weapons")]
    public bool combatEnabled;
    void ManageAllTurrets()
    {
        TurretBehaviour[] turretBehaviours = GetComponentsInChildren<TurretBehaviour>();
        foreach (TurretBehaviour turret  in turretBehaviours)
        {
            turret.canAim = combatEnabled;
            turret.canFire = combatEnabled;
        }
    }

    #endregion Weapons
    #region Dev
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
    #endregion Dev
}
