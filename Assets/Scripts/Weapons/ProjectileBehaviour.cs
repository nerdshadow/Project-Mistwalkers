using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour
{
    public int projectileDamage = 1;
    public float lifetime = 5f;
    public float impulsePower = 1f;
    public GameObject operatorGO;
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }
    //protected Collider SomethingInFront()
    //{
    //    RaycastHit hit;
    //    Debug.DrawRay(this.transform.position, this.transform.forward, Color.magenta);
    //    if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 2f * GetComponentInChildren<CapsuleCollider>().height))
    //    {
    //        GetComponent<Rigidbody>().position = hit.point;
    //        return hit.collider;
    //    }
    //    return null;
    //}
    //protected void OnTriggerEnter(Collider other)
    //{
    //    DoImpact(other);
    //    //if (other.GetComponentInParent<CharacterStats>())
    //    //{
    //    //    other.GetComponentInParent<CharacterStats>().ChangeHealth(-projectileDamage);
    //    //    Destroy(gameObject);
    //    //}
    //}
    protected void OnCollisionEnter(Collision collision)
    {
        DoImpact(collision.collider);
        DoDamage(collision.collider);
        //play VFX
        Destroy(gameObject);
    }

    void DoImpact(Collider collider)
    {
        if (collider == null || collider.transform.root == operatorGO.transform)
            return;
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if (rb == null)
            rb = collider.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceVector = (GetComponent<Rigidbody>().velocity - transform.position);

            forceVector = forceVector.normalized;
            rb.AddForce(forceVector * impulsePower, ForceMode.Impulse);
        }
    }
    void DoDamage(Collider collider)
    {
        if (collider.transform.root == operatorGO.transform)
        {
            Debug.Log("Hiting parent");
        }
        else
        {
            IDamageable potTarget = collider.GetComponent<IDamageable>();
            if (potTarget != null)
            {
                potTarget.DoDamage(projectileDamage);
            }
            else
            {
                potTarget = collider.GetComponentInParent<IDamageable>();
                if (potTarget != null)
                {
                    potTarget.DoDamage(projectileDamage);
                }
            }
        }
    }
}
