using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_VehicleSpawner : MonoBehaviour
{
    [SerializeField]
    VehicleSpawner currentVehicleSpawner;

    [SerializeField]
    TMP_Dropdown basesDropDown;
    public List<VehicleBaseStats> possibleVehiclesBases = new List<VehicleBaseStats>();

    [SerializeField]
    TMP_Dropdown cabsDropDown;
    public List<VehiclePartStats> possibleVehiclesCabs = new List<VehiclePartStats>();

    [SerializeField]
    TMP_Dropdown bodiesDropDown;
    public List<VehiclePartStats> possibleVehiclesBodies = new List<VehiclePartStats>();

    private void OnValidate()
    {
        RefreshBases();
        RefreshCabs();
        RefreshBodies();
    }
    [ContextMenu("Refresh Spawner")]
    void RefreshSpawner()
    {
        RefreshBases();
        RefreshCabs();
        RefreshBodies();
    }
    void RefreshBases()
    {
        if (basesDropDown == null || possibleVehiclesBases.Count == 0)
            return;

        basesDropDown.ClearOptions();
        foreach (VehicleBaseStats vehicleBaseStats in possibleVehiclesBases)
        {
            basesDropDown.options.Add(new TMP_Dropdown.OptionData(vehicleBaseStats.vehicleBaseName, null));
        }
        basesDropDown.value = 0;
        ChangeBase(0);
        basesDropDown.RefreshShownValue();
    }
    void RefreshCabs()
    {
        if (cabsDropDown == null || possibleVehiclesCabs.Count == 0)
            return;

        cabsDropDown.ClearOptions();
        foreach (VehiclePartStats vehiclePartStats in possibleVehiclesCabs)
        {
            if (vehiclePartStats.partType != VehiclePartStats.PartType.cab)
            {
                Debug.Log("There is !Cab part as " + vehiclePartStats.partName);
                continue;
            }
            if (vehiclePartStats.relatedBase != currentVehicleSpawner.vehicleBaseSO)
            {
                //do not add to list
                continue;
            }
            cabsDropDown.options.Add(new TMP_Dropdown.OptionData(vehiclePartStats.partName, null));
        }
        cabsDropDown.value = 0;
        ChangeCab(0);
        cabsDropDown.RefreshShownValue();
    }
    void RefreshBodies()
    {
        if (bodiesDropDown == null)
            return;
        if(possibleVehiclesBodies.Count <= 0)
            bodiesDropDown.gameObject.SetActive(false);
        else
            bodiesDropDown.gameObject.SetActive(true);

        bodiesDropDown.ClearOptions();
        foreach (VehiclePartStats vehiclePartStats in possibleVehiclesBodies)
        {
            if (vehiclePartStats.partType != VehiclePartStats.PartType.body)
            {
                Debug.Log("There is !Body part as " + vehiclePartStats.partName);
                continue;
            }
            if (vehiclePartStats.relatedBase != currentVehicleSpawner.vehicleBaseSO)
            {
                //do not add to list
                continue;
            }
            bodiesDropDown.options.Add(new TMP_Dropdown.OptionData(vehiclePartStats.partName, null));
        }

        if (bodiesDropDown.options.Count == 0)
        {
            currentVehicleSpawner.vehicleBodyPartSO = null;
            bodiesDropDown.gameObject.SetActive(false);
        }
        else
        {
            bodiesDropDown.gameObject.SetActive(true);
            bodiesDropDown.value = 0;
            ChangeBody(0);
        }
        bodiesDropDown.RefreshShownValue();
    }
    public void ChangeBase(Int32 _index)
    {
        //Debug.Log("Returning base index is " + _index);
        if (currentVehicleSpawner == null)
            return;
        VehicleBaseStats potVehicleBase = null;
        foreach (VehicleBaseStats _vehBases in possibleVehiclesBases)
        {
            if (_vehBases.vehicleBaseName == basesDropDown.options[_index].text)
                potVehicleBase = _vehBases;
        }
        if (potVehicleBase == null)
        {
            Debug.Log("No such base");
            return;
        }
        currentVehicleSpawner.vehicleBaseSO = potVehicleBase;
        RefreshCabs();
        RefreshBodies();
    }
    public void ChangeCab(Int32 _index)
    {
        //Debug.Log("Returning cab index is " + _index);

        if (currentVehicleSpawner == null)
            return;
        VehiclePartStats potVehicleCab = null;
        foreach (VehiclePartStats _vehCab in possibleVehiclesCabs)
        {
            if (_vehCab.partName == cabsDropDown.options[_index].text)
                potVehicleCab = _vehCab;
        }
        if (potVehicleCab == null)
        {
            Debug.Log("No such cab");
            return;
        }
        currentVehicleSpawner.vehicleCabPartSO = potVehicleCab;
    }
    public void ChangeBody(Int32 _index)
    {
        //Debug.Log("Returning body index is " + _index);

        if (currentVehicleSpawner == null)
            return;
        if (bodiesDropDown.options.Count <= 0)
            currentVehicleSpawner.vehicleBodyPartSO = null;
        VehiclePartStats potVehicleBody = null;
        foreach (VehiclePartStats _vehBody in possibleVehiclesBodies)
        {
            if (_vehBody.partName == bodiesDropDown.options[_index].text)
                potVehicleBody = _vehBody;
        }
        if (potVehicleBody == null)
        {
            Debug.Log("No such body");
            return;
        }
        currentVehicleSpawner.vehicleBodyPartSO = potVehicleBody;
    }

    [ContextMenu("SpawnVehicle")]
    public void TrySpawnVehicle()
    {
        if (currentVehicleSpawner == null)
            return;

        currentVehicleSpawner.AssemblyVehicle();
    }

}
