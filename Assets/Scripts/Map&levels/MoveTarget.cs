using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public float speed = 2f;
    public bool moveByReference = true;
    Vector3 initialPos = Vector3.zero;
    float distance = 100f;
    public Transform reference;

    private void FixedUpdate()
    {
        if (moveByReference == true && reference != null)
            MoveByRef_Simple();
        else
            MoveBySpeed();
    }
    void MoveByRef_Simple()
    {
        Vector3 newLoc = new Vector3(transform.position.x, transform.position.y, distance + reference.position.z);
        transform.transform.position = newLoc;
    }
    void MoveBySpeed()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
