using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddForce : MonoBehaviour
{
    [ContextMenu("add force up")]
    void AddForceUp()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * 10, ForceMode.Impulse);
    }
}
