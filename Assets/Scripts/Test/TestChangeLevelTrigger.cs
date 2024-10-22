using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeLevelTrigger : MonoBehaviour
{
    Collider triggerColl;
    GameManager gameManager;
    private void OnEnable()
    {
        triggerColl = GetComponent<Collider>();        
        gameManager = GameManager.instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<VehicleMovement>())
        {
            //change level to next
            Debug.Log("LoadingLevel");
        }
    }
}
