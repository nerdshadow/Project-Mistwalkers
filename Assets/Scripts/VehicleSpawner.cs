using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public GameObject currentVehicle = null;
    public Transform spawnPosition;
    public VehicleBaseStats vehicleBaseSO;
    public VehiclePartStats vehicleCabPartSO;
    public VehiclePartStats vehicleBodyPartSO;
    bool canSpawn = true;
    private void OnValidate()
    {
        InitParts();
    }
    private void Start()
    {
        AssemblyVehicle();
    }
    void InitParts()
    {
        if (vehicleBodyPartSO != null && vehicleCabPartSO != null)
        {
            if (vehicleBodyPartSO.partType != PartType.Body
                || vehicleCabPartSO.partType != PartType.Cab
                || vehicleBodyPartSO.relatedBase != vehicleBaseSO
                || vehicleCabPartSO.relatedBase != vehicleBaseSO)
            {
                canSpawn = false;
                return;
            }
        }
        if (vehicleBodyPartSO == null && vehicleCabPartSO != null)
        {
            if (vehicleCabPartSO.partType != PartType.Cab
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
    public void AssemblyVehicle()
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
        InitParts();
        if (canSpawn == false)
        {
            Debug.Log("Cant spawn vehicle with that parametrs");
            return;
        }
        AssemblyVehicle();
    }

    public void ChangeCab()
    {
        InitParts();
        if (currentVehicle == null || canSpawn == false || Application.isPlaying == false)
            return;

        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();

        Destroy(vehicleBehaviour.currentVehicleCab);
        Instantiate(vehicleCabPartSO.partPrefab, vehicleBehaviour.cabHolder.transform);
        StartCoroutine(UpdateWheels());

    }
    public void ChangeBody()
    {
        InitParts();
        if (currentVehicle == null || canSpawn == false)
            return;

        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();

        if (vehicleBodyPartSO != null)
        {
            Destroy(vehicleBehaviour.currentVehicleBody);
            Instantiate(vehicleBodyPartSO.partPrefab, vehicleBehaviour.bodyHolder.transform);
        }
        StartCoroutine(UpdateWheels());
    }
    IEnumerator AssebleNextFrame()
    {
        yield return new WaitForFixedUpdate();
        //currentVehicle.GetComponent<VehicleBehaviour>().SerializeVehicle();
        currentVehicle.SetActive(true);
        StartCoroutine(UpdateWheels());
        currentVehicle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
    IEnumerator UpdateWheels()
    {
        yield return new WaitForFixedUpdate();
        List<WheelBehaviour> wBehs = new List<WheelBehaviour>();
        wBehs.AddRange(currentVehicle.GetComponentsInChildren<WheelBehaviour>());
        foreach (WheelBehaviour wheelBehaviour in wBehs)
        {
            wheelBehaviour.ReManageWheelColliders();
        }
    }
    IEnumerator UpdateParts()
    {
        yield return new WaitForFixedUpdate();
        VehicleBehaviour vehicleBeh = currentVehicle.GetComponent<VehicleBehaviour>();

        VehiclePartBehaviour[] potParts = vehicleBeh.GetComponentsInChildren<VehiclePartBehaviour>();
        foreach (VehiclePartBehaviour part in potParts)
        {
            if (part.partType == PartType.Cab)
                vehicleBeh.currentVehicleCab = part.gameObject;
            if(part.partType == PartType.Body)
                vehicleBeh.currentVehicleBody = part.gameObject;
        }
    }
}
