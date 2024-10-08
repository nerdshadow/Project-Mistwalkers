using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RuntimeSaveData", menuName = "SO/PlayerSave_Runtime")]
public class RuntimePlayerSaveData : ScriptableObject
{
    public static RuntimePlayerSaveData instance = null;
    public static RuntimePlayerSaveData Instance
    {
        get
        {
            if(instance == null)
                instance = Resources.Load<RuntimePlayerSaveData>(path: "RuntimeSaveData");
            return instance;
        }
    }
    public PlayerSaveData currentPlayerSaveData;
    public bool AddItemToInv(ScriptableObject _item)
    {
        //if (playerInventory.Count >= inventoryBaseSize)
        //{
        //    Debug.LogWarning("Inv at max capacity");
        //    return;
        //}

        if (_item == null)
        {
            Debug.LogWarning("item ref is null");
            return false;
        }
        if (_item is IItemInfo)
        {
            currentPlayerSaveData.playerInventory.Add(_item);
            return true;
        }
        else
        {
            Debug.LogWarning("item ref do not have item interface");
            return false;
        }

    }
    public bool RemoveItemFromInv(ScriptableObject _item)
    {
        if (_item == null)
        {
            Debug.LogWarning("item ref is null");
            return false;
        }
        if (_item is IItemInfo)
        {
            currentPlayerSaveData.playerInventory.Remove(_item);
            return true;
        }
        else
        {
            Debug.LogWarning("item ref do not have item interface");
            return false;
        }
    }
    [ContextMenu("ClearInv")]
    public void ClearInv()
    {
        currentPlayerSaveData.playerInventory = new List<ScriptableObject>();
    }
    public void ChangeVehicle(GameObject _potVehicle, int _indexInArr)
    {
        VehicleBaseStats vehicleBaseStats = _potVehicle.GetComponent<VehicleBehaviour>().currentVehicleStats;

        VehiclePartStats cabStats = _potVehicle.GetComponent<VehicleBehaviour>().currentVehicleCab.GetComponent<VehiclePartBehaviour>().partStats;

        List<TurretBehaviour> buffTurr = new List<TurretBehaviour>();
        buffTurr.AddRange(_potVehicle.GetComponent<VehicleBehaviour>().currentVehicleCab.GetComponentsInChildren<TurretBehaviour>());
        List<TurretStats> cabTurrets = new List<TurretStats>();
        foreach (TurretBehaviour turrB in buffTurr)
        {
            cabTurrets.Add(turrB.turretStats);
        }

        VehiclePartStats bodyStats = _potVehicle.GetComponentInChildren<VehicleBehaviour>().currentVehicleBody.GetComponent<VehiclePartBehaviour>().partStats;

        buffTurr = new List<TurretBehaviour>();
        buffTurr.AddRange(_potVehicle.GetComponent<VehicleBehaviour>().currentVehicleBody.GetComponentsInChildren<TurretBehaviour>());
        List<TurretStats> bodyTurrets = new List<TurretStats>();
        foreach (TurretBehaviour turrB in buffTurr)
        {
            bodyTurrets.Add(turrB.turretStats);
        }

        currentPlayerSaveData.playerVehiclesVar[_indexInArr] = new VehicleSaveVar(vehicleBaseStats, cabStats, cabTurrets, bodyStats, bodyTurrets);
    }
    public void ChangePath(List<PathPoint> _pathPoints)
    {
        currentPlayerSaveData.pathPoints = new List<PathPoint>();
        currentPlayerSaveData.pathPoints.AddRange(_pathPoints);
    }
    public void ChangeData(PlayerSaveData saveData)
    {
        currentPlayerSaveData.playerName = saveData.playerName;
        currentPlayerSaveData.playerRandSeed = saveData.playerRandSeed;
        currentPlayerSaveData.playerLastCityIndex = saveData.playerLastCityIndex;
        currentPlayerSaveData.playerMoney = saveData.playerMoney;
        currentPlayerSaveData.inventoryBaseSize = saveData.inventoryBaseSize;
        currentPlayerSaveData.inventoryCurrentSize = saveData.inventoryCurrentSize;
        currentPlayerSaveData.playerInventory = saveData.playerInventory;
        currentPlayerSaveData.pathPoints = saveData.pathPoints;
        currentPlayerSaveData.playerVehiclesVar = saveData.playerVehiclesVar;
    }
    [ContextMenu("Try to save")]
    public void SaveDataToMachine()
    {
        PlayerSaveData saveData = currentPlayerSaveData;
        SaveLoadSystem.SavePlayerData(saveData);
    }
    [ContextMenu("Try to load")]
    public void LoadDataFromMachine()
    {
        SaveLoadSystem.LoadPlayerData(this);
    }
}
