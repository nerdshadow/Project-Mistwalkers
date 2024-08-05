using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExplosion : MonoBehaviour
{
    public float explodeForce = 100f;
    public float explodeRad = 15f;
    public float explodeUp = 3f;
    [ContextMenu("Explode")]
    void ToExplode()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, explodeRad);
        foreach (Collider coll in colls) 
        {
            if (coll.GetComponent<Rigidbody>() == true)
            {
                coll.GetComponent<Rigidbody>().AddExplosionForce(explodeForce, transform.position, explodeRad * 1.5f, explodeUp, ForceMode.Impulse);
            }
        }
    }
}
