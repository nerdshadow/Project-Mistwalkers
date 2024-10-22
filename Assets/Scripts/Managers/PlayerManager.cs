using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField]
    public RuntimePlayerSaveData runtimeSave = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
    }
    private void InitManager()
    {
        runtimeSave = RuntimePlayerSaveData.Instance;
        LoadPlayerData();
        CheckPlayerZeroVehicle();
        //CheckPlayerZeroVehicle(standartVehicle, 0);
    }
    [ContextMenu("Load data")]
    private void LoadPlayerData()
    {
        if (runtimeSave == null)
        {
            Debug.LogWarning("No runtime save file");
            return;
        }
    }
    [ContextMenu("Save data")]
    public void SavePlayerData()
    {
        if (runtimeSave == null)
        {
            Debug.Log("No runtime save file");
            return;
        }
    }

    void RefreshInv()
    {
        runtimeSave.currentPlayerSaveData.playerInventory = new List<ScriptableObject>();
        runtimeSave.currentPlayerSaveData.playerInventory.AddRange(runtimeSave.currentPlayerSaveData.playerInventory);
    }

    [SerializeField]
    ScriptableObject testItem = null;
    [ContextMenu("Test Add item")]
    void AddItemToInv()
    {
        runtimeSave.AddItemToInv(testItem);
        RefreshInv();
    }
    public void AddItemToInv(ScriptableObject _item)
    {
        if (runtimeSave.AddItemToInv(_item) == false)
        {
            Debug.Log(_item + " added to player inv");
            RefreshInv();
        }
        else
        {
            return;
        }
    }
    public void RemoveItemFromInv(ScriptableObject _item)
    {
        if (runtimeSave.RemoveItemFromInv(_item) == false)
        {
            Debug.Log(_item + " removed from player inv");
            RefreshInv();
        }
        else
        {
            return;
        }
    }
    [SerializeField]
    GameObject standartVehicle;
    void CheckPlayerZeroVehicle()
    {
        if (runtimeSave.currentPlayerSaveData.playerVehiclesVar[0].vehicleBaseStats == null)
        {
            VehicleMovement vehBeh = standartVehicle.GetComponent<VehicleMovement>();
            runtimeSave.currentPlayerSaveData.playerVehiclesVar[0] = new VehicleSaveVar(vehBeh.currentVehicleStats,
                                                                vehBeh.currentVehicleCab.GetComponent<VehiclePartBehaviour>().partStats,
                                                                new List<TurretStats>(),
                                                                vehBeh.currentVehicleBody.GetComponent<VehiclePartBehaviour>().partStats,
                                                                new List<TurretStats>());
        }
    }
    void CheckPlayerZeroVehicle(GameObject _vehicle, int _index)
    {
        runtimeSave.ChangeVehicle(_vehicle, _index);
    }
}
