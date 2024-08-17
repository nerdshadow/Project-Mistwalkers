using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour
{
    public float projectileDamage = 1f;
    public float lifetime = 5f;
    public float impulsePower = 1f;
    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }
    protected Collider SomethingInFront()
    {
        RaycastHit hit;
        Debug.DrawRay(this.transform.position, this.transform.forward, Color.magenta);
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 2f * GetComponentInChildren<CapsuleCollider>().height))
        {
            GetComponent<Rigidbody>().position = hit.point;
            return hit.collider;
        }
        return null;
    }
    protected void OnTriggerEnter(Collider other)
    {
        DoImpact(other);
        //if (other.GetComponentInParent<CharacterStats>())
        //{
        //    other.GetComponentInParent<CharacterStats>().ChangeHealth(-projectileDamage);
        //    Destroy(gameObject);
        //}
    }
    protected void OnCollisionEnter(Collision collision)
    {
        DoImpact(collision.collider);
        //collision.collider.GetComponentInParent<IDestroyable>()?.ChangeHealth(-projectileDamage);
        Destroy(gameObject);
    }

    void DoImpact(Collider collider)
    {
        if (collider == null)
            return;

        if (collider.GetComponentInParent<Rigidbody>())
        {
            Vector3 forceVector = (GetComponent<Rigidbody>().velocity - transform.position);
            //if (collider.GetComponentInParent<CharacterStats>() != null)
            //    forceVector.y = 0;
            forceVector = forceVector.normalized;
            collider.GetComponentInParent<Rigidbody>().AddForce(forceVector * impulsePower, ForceMode.Impulse);
        }
    }
}
