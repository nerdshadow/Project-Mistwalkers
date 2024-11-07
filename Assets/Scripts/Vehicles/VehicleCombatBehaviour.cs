using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(VehicleMovement))]
public class VehicleCombatBehaviour : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField]
    VehicleMovement vehicleMovement;
    [SerializeField]
    VehicleBaseStats vehicleBaseStats;
    public UnityEvent vehicleDies = new UnityEvent();

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
        vehicleDies.Invoke();
        vehicleDies.RemoveAllListeners();
        combatEnabled = false;
        ManageAllTurrets();
        ChangeVehicleMovement();
        //Check left over hp| if a lot of minus hp then blown up vehicle else slown down vehicle with fire in engine
        float leftoverhpMod = (Mathf.Abs((float)currentHealth) / (float)maxHealth);
        //Debug.Log(leftoverhpMod);
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
        TurretBehaviour[] tBehs = GetComponentsInChildren<TurretBehaviour>();
        foreach (TurretBehaviour t in tBehs)
        {
            t.Die();
        }
        int r = 1;

        VehiclePartCombatBehaviour[] vpcBehs = GetComponentsInChildren<VehiclePartCombatBehaviour>();
        foreach (VehiclePartCombatBehaviour vpcBeh in vpcBehs)
        {
            if(vpcBeh == null)
                continue;

            if (vpcBeh.GetComponent<VehiclePartBehaviour>() != null)
            {                    
                Destroy(vpcBeh.GetComponent<VehiclePartBehaviour>());
                r = Random.Range(1, 5);
                //Debug.Log("r to destr = " + r);
                if (r == 1)
                {
                    vpcBeh.DetachPart();
                }
            }
            else
            {
                r = Random.Range(1, 11);
                if (r <= 8)
                {
                    vpcBeh.DetachPart();
                }
                else
                {
                    Destroy(vpcBeh);
                }
            }
        }
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
                //Debug.Log(vpcBeh.name);
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
        maxHealth = vehicleBaseStats.vehicleBaseHealth;
        maxHealth += vehicleMovement.cabHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth;
        if(vehicleMovement.bodyHolder != null && vehicleMovement.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>() != null)
            maxHealth += vehicleMovement.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats.partHealth;
    }
    public void ResetHealth()
    {
        Debug.Log("Reseting health of " + gameObject.name);
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
            if(combatEnabled == true)
                turret.currentTurretState = TurretStates.Combat;
            else
                turret.currentTurretState = TurretStates.Calm;
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
