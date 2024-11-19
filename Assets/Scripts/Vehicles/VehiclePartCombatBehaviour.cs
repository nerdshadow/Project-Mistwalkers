using Unity.VisualScripting;
using UnityEngine;

public class VehiclePartCombatBehaviour : MonoBehaviour, IDamageable, IHealth
{
    VehiclePartBehaviour vehiclePartBeh = null;
    [field: SerializeField]
    public int maxHealth { get; set; }
    [field: SerializeField]
    public int currentHealth { get; set; }
    [field: SerializeField]
    public bool isDead { get; set; }

    [SerializeField]
    bool isDetachable = false;
    //[SerializeField]
    //bool isDetached = false;

    public bool parentIsDead = false;
    private void Start()
    {
        InitHealth();
        ResetHealth();
    }
    private void OnEnable()
    {
        if (GetComponentInParent<VehicleCombatBehaviour>() == null)
            return;
        GetComponentInParent<VehicleCombatBehaviour>().vehicleDies.AddListener(ParentDies);

        vehiclePartBeh = GetComponent<VehiclePartBehaviour>();
    }
    private void OnDisable()
    {
        if (GetComponentInParent<VehicleCombatBehaviour>() == null)
            return;
        GetComponentInParent<VehicleCombatBehaviour>().vehicleDies.RemoveListener(ParentDies);
    }
    public void InitHealth()
    {
        if (GetComponent<VehiclePartBehaviour>() != null)
            maxHealth = GetComponent<VehiclePartBehaviour>().partStats.partHealth;
        currentHealth = maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void ParentDies()
    {
        parentIsDead = true;
        if (GetComponentInParent<VehicleCombatBehaviour>() == null)
            return;
        GetComponentInParent<VehicleCombatBehaviour>().vehicleDies.RemoveListener(ParentDies);
        //Destroy(this);
    }
    public void DoDamage(int _damage)
    {
        if (parentIsDead == true)
        {
            return;
        }
        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            if (isDead == true)
                GetComponentInParent<VehicleCombatBehaviour>().DoDamage((int)(_damage * 1.5f));
            else
            {
                GetComponentInParent<VehicleCombatBehaviour>().DoDamage(_damage  - Mathf.Abs(currentHealth));
                isDead = true;
            }
            if (isDetachable == true)
            {
                DetachPart();
            }
        }
        else
        {
            GetComponentInParent<VehicleCombatBehaviour>().DoDamage(_damage);
        }
    }
    public void DoDamage(int _damage, Vector3 _dmgPos)
    {
    
    }
    public void DoDamage(int _damage, Vector3 _dmgPos, Vector3 _dmgVectorForce)
    {

    }

    public void DetachPart()
    {        
        Rigidbody body = this.gameObject.AddComponent<Rigidbody>();
        if(vehiclePartBeh != null)
            body.mass = vehiclePartBeh.partStats.partMass;
        else body.mass = 1f;
        body.drag = 0.3f;
        body.angularDrag = 0.2f;
        this.transform.SetParent(null, true);
        Vector3 pointOfEffect = body.worldCenterOfMass;
        pointOfEffect = new Vector3(pointOfEffect.x + 1 * Random.Range(-1f, 1f),
                                    pointOfEffect.y + 1 * Random.Range(-1f, -0.1f),
                                    pointOfEffect.z + 1 * Random.Range(-1f, 1f));
        body.AddExplosionForce(500 * body.mass, pointOfEffect, 5f, 500 * body.mass);
        Destroy(this);
    }
    [SerializeField]
    int debugDmg = 10;

    [ContextMenu("do test dmg")]
    void DebugDmg()
    {
        DoDamage(debugDmg);
    }
}
