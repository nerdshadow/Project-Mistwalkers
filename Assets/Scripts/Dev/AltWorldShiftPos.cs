using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltWorldShiftPos : MonoBehaviour
{
    Vector3 offset = Vector3.zero;
    public Transform referenceObject;
    public List<GameObject> whatToTeleport = new List<GameObject>(); // road and vehicle root objects
    private void LateUpdate()
    {
        offset = referenceObject.position;
        //offset = referenceObject.GetComponent<Rigidbody>().position;
    }

    [ContextMenu("Move at start")]
    void Shift()
    {
        foreach (GameObject obj in whatToTeleport)
        {
            Vector3 newLoc = Vector3.zero;
            //if (obj.GetComponent<Rigidbody>() != null)
            //{
            //    Rigidbody rb = obj.GetComponent<Rigidbody>();
            //    newLoc = rb.position - new Vector3(0, 0, offset.z);
            //    obj.GetComponent<Rigidbody>().MovePosition(newLoc);
            //    continue;
            //}
            newLoc = obj.transform.position - new Vector3(0, 0, offset.z);
            obj.transform.position = newLoc;
        }
    }
}
