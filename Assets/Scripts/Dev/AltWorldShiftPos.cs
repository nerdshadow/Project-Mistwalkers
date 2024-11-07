using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Shift();
    }
    void ConstantShift()
    {
        
    }

    [ContextMenu("Move at start")]
    void Shift()
    {
        Vector3 newLoc = Vector3.zero;
        foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Debug.Log("Root object " + g);
            if (g.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = g.GetComponent<Rigidbody>();
                newLoc = rb.position - new Vector3(0, 0, offset.z);
                rb.position = newLoc;

                //continue;
            }
            newLoc = g.transform.position - new Vector3(0, 0, offset.z);
            g.transform.position = newLoc;
        }
        //foreach (GameObject obj in whatToTeleport)
        //{
        //    Vector3 newLoc = Vector3.zero;
        //    //if (obj.GetComponent<Rigidbody>() != null)
        //    //{
        //    //    Rigidbody rb = obj.GetComponent<Rigidbody>();
        //    //    newLoc = rb.position - new Vector3(0, 0, offset.z);
        //    //    rb.position = newLoc;
        //    //    foreach (Rigidbody childrb in rb.GetComponentsInChildren<Rigidbody>())
        //    //    {
        //    //        newLoc = childrb.position - new Vector3(0, 0, offset.z);
        //    //        childrb.position = newLoc;
        //    //    }
        //    //    continue;
        //    //}
        //    newLoc = obj.transform.position - new Vector3(0, 0, offset.z);
        //    obj.transform.position = newLoc;
        //}
    }
}
