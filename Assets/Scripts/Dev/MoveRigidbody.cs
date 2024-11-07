using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigidbody : MonoBehaviour
{
    [ContextMenu("Move rigid")]
    void MoveRigid()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + new Vector3(0, 0.02f, 1f));
    }
}
