using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShowCenterOfMass : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position + GetComponent<Rigidbody>().centerOfMass;
        Gizmos.DrawWireSphere(pos, 0.3f);

        Gizmos.color = Color.white;
    }
#endif
}
