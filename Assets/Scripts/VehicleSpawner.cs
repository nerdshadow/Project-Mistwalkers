using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    GameObject currentVehicle = null;
    public Transform spawnPosition;
    public VehicleBaseStats vehicleBaseSO;
    public VehiclePartStats vehicleCabPartSO;
    public VehiclePartStats vehicleBodyPartSO;
    bool canSpawn = true;
    private void OnValidate()
    {
        InitParts();
    }
    void InitParts()
    {
        if (vehicleBodyPartSO != null && vehicleCabPartSO != null)
        {         
            if (vehicleBodyPartSO.partType != VehiclePartStats.PartType.body
                || vehicleCabPartSO.partType != VehiclePartStats.PartType.cab
                || vehicleBodyPartSO.relatedBase != vehicleBaseSO
                || vehicleCabPartSO.relatedBase != vehicleBaseSO)
            {
                canSpawn = false;
                return;
            }
        }
        if (vehicleBodyPartSO == null && vehicleCabPartSO != null)
        {
            if (vehicleCabPartSO.partType != VehiclePartStats.PartType.cab                
                || vehicleCabPartSO.relatedBase != vehicleBaseSO)
            {
                canSpawn = false;
                return;
            }
        }
        if (vehicleCabPartSO == null)
        {
            canSpawn = false;
            return;
        }
        canSpawn = true;
    }
    void AssemblyVehicle()
    {
        if (currentVehicle != null)
            Destroy(currentVehicle);
        currentVehicle = Instantiate(vehicleBaseSO.vehicleBasePrefab, spawnPosition.position, Quaternion.identity);
        //currentVehicle.SetActive(false);
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();

        Destroy(vehicleBehaviour.currentVehicleCab);
        Instantiate(vehicleCabPartSO.partPrefab, vehicleBehaviour.cabHolder.transform);

        if (vehicleBodyPartSO != null)
        {
            Destroy(vehicleBehaviour.currentVehicleBody);
            Instantiate(vehicleBodyPartSO.partPrefab, vehicleBehaviour.bodyHolder.transform);
        }

        StartCoroutine(AssebleNextFrame());        
    }
    [ContextMenu("Spawn vehicle")]
    void SpawnVehicle()
    {
        if (canSpawn == false)
        {
            Debug.Log("Cant spawn vehicle with that parametrs");
            return;
        }
        AssemblyVehicle();
    }

    IEnumerator AssebleNextFrame()
    {
        yield return new WaitForFixedUpdate();

        List<WheelBehaviour> wBehs = new List<WheelBehaviour>();
        wBehs.AddRange(currentVehicle.GetComponentsInChildren<WheelBehaviour>());

        currentVehicle.GetComponent<VehicleBehaviour>().SerializeVehicle();
        foreach (WheelBehaviour wheelBehaviour in wBehs)
        {
            wheelBehaviour.ReManageWheelColliders();
        }

        currentVehicle.SetActive(true);
    }
}
