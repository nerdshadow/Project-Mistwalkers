using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehiclePartCombatBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField]
    int partMaxHealth = 10;
    [SerializeField]
    int partCurrentHealth = 10;
    [SerializeField]
    bool isDetachable = false;
    [SerializeField]
    bool isDestroyed = false;
    private void Start()
    {
        InitHealth();
        partCurrentHealth = partMaxHealth;
    }

    private void InitHealth()
    {
        if (GetComponent<VehiclePartBehaviour>() != null)
            partMaxHealth = GetComponent<VehiclePartBehaviour>().partStats.partHealth;
        partCurrentHealth = partMaxHealth;
    }

    public void DoDamage(int _damage)
    {
        partCurrentHealth -= _damage;
        if (partCurrentHealth <= 0)
        {
            if (isDestroyed == true)
                GetComponentInParent<VehicleCombatBehaviour>().DoDamage((int)(_damage * 1.5f));
            else
            {
                GetComponentInParent<VehicleCombatBehaviour>().DoDamage(_damage  - Mathf.Abs(partCurrentHealth));
                isDestroyed = true;
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

    private void DetachPart()
    {
        FixedJoint joint = GetComponent<FixedJoint>();
        DestroyImmediate(joint);
        Rigidbody body = GetComponent<Rigidbody>();
        body.drag = 0.1f;
        body.angularDrag = 0.1f;
        this.transform.SetParent(null, true);
        Vector3 pointOfEffect = GetComponent<Rigidbody>().worldCenterOfMass;
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
