using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosiveProjectile : ProjectileBehaviour
{
    GameObject explosionVFX;
    public List<Collider> potColls = new List<Collider>();
    protected override void Update()
    {
        base.Update();
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        CheckPotentialTargets();
        DoImpact(collision.collider);
        DoDamage(collision.collider);
        //play VFX
        Destroy(gameObject);
    }
    void CheckPotentialTargets()
    {        
        potColls.AddRange(Physics.OverlapSphere(transform.position, explRadius));
        foreach (Collider coll in potColls.ToList())
        {
            if (coll == null || coll.transform.root == operatorGO.transform)
            {
                potColls.Remove(coll);
                continue;
            }
            Rigidbody rb = coll.GetComponent<Rigidbody>();
            if (rb == null)
                rb = coll.GetComponentInParent<Rigidbody>();
            if (rb == null)
            {
                potColls.Remove(coll);
            }
        }
    }
    protected override void DoImpact(Collider collider)
    {
        foreach (Collider coll in potColls)
        {
            Rigidbody rb = coll.GetComponent<Rigidbody>();
            if (rb == null)
                rb = coll.GetComponentInParent<Rigidbody>();

            rb.AddExplosionForce(impulsePower, transform.position, explRadius * 1.2f);
        }
    }
    protected override void DoDamage(Collider collider)
    {
        List<Transform> parents = new List<Transform>();
        foreach (Collider coll in potColls)
        {
            IDamageable potTarget = coll.GetComponent<IDamageable>();
            if (potTarget != null)
            {
                if (parents.Contains(coll.transform.root) == false)
                {
                    potTarget.DoDamage(projectileDamage);
                    parents.Add(coll.transform.root);
                }
                else
                    potTarget.DoDamage(1);
            }
            else
            {
                potTarget = coll.GetComponentInParent<IDamageable>();
                if(potTarget == null)
                    continue;
                if (parents.Contains(coll.transform.root) == false)
                {
                    potTarget.DoDamage(projectileDamage);
                    parents.Add(coll.transform.root);
                }
                else
                    potTarget.DoDamage(1);
            }
        }        
    }
}
