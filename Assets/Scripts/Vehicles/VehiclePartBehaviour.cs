using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePartBehaviour : MonoBehaviour
{
    [Header("Stats")]
    [Space(5)]    
    public VehiclePartStats partStats = null;
    public PartType partType = PartType.Cab;
    [SerializeField]
    VehicleMovement currentBase = null;
    [Header("Weapons")]
    [Space(5)]
    List<GameObject> weaponHolders = new List<GameObject>();
    List<TurretBehaviour> basicTurrets = new List<TurretBehaviour>();
    private void OnValidate()
    {
        UpdateComp();  
    }
    private void OnEnable()
    {
        UpdateComp();
    }
    void UpdateComp()
    {
        if(partStats != null)
        {
            partType = partStats.partType;
        }
        if (currentBase == null) 
        {
            currentBase = GetComponentInParent<VehicleMovement>();
        }
        if (currentBase != null)
        {
            if (partType == PartType.Cab)
            {
                currentBase.currentVehicleCab = this.gameObject;
            }
            else if (partType == PartType.Body)
            {
                currentBase.currentVehicleBody = this.gameObject;
            }
        }
        if (currentBase != null)
        {
            
        }
        if (weaponHolders.Count != 0)
        {
            foreach (GameObject wHolder in weaponHolders)
            {
                if (wHolder.GetComponent<TurretBehaviour>() != null 
                    &&  !basicTurrets.Contains(wHolder.GetComponent<TurretBehaviour>()))
                {
                    basicTurrets.Add(wHolder.GetComponent<TurretBehaviour>());
                }
            }            
        }
    }
}
