using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSave_Runtime", menuName = "SO/PlayerSave_Runtime")]
public class RuntimePlayerSaveData : ScriptableObject
{
    public string playerName = "Player";
    public int playerRandSeed = 451;
    public short playerLastCityIndex = 0;
    public int playerMoney = 100;
    public short inventoryBaseSize = 20;
    public int inventory—urrentSize = 20;
    public List<ScriptableObject> playerInventory = new List<ScriptableObject>();
    public List<PathPoint> pathPoints = new List<PathPoint>();
    public VehicleSaveVar[] playerVehiclesVar = new VehicleSaveVar[5];
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
            playerInventory.Add(_item);
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
            playerInventory.Remove(_item);
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
        playerInventory = new List<ScriptableObject>();
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

        playerVehiclesVar[_indexInArr] = new VehicleSaveVar(vehicleBaseStats, cabStats, cabTurrets, bodyStats, bodyTurrets);
    }
    public void ChangePath(List<PathPoint> _pathPoints)
    {
        pathPoints = new List<PathPoint>();
        pathPoints.AddRange(_pathPoints);
    }
    public void ChangeData(SaveData saveData)
    {
        playerName = saveData.playerName;
        playerRandSeed = saveData.playerRandSeed;
        playerLastCityIndex = saveData.playerLastCityIndex;
        playerMoney = saveData.playerMoney;
        inventoryBaseSize = saveData.inventoryBaseSize;
        inventory—urrentSize = saveData.inventoryCurrentSize;
        playerInventory = saveData.playerInventory;
        pathPoints = saveData.pathPoints;
        playerVehiclesVar = saveData.playerVehiclesVar;
    }
    [ContextMenu("Try to save")]
    public void SaveDataToMachine()
    {
        SaveData saveData = new SaveData(playerName, playerRandSeed, playerLastCityIndex, playerMoney, 
            inventoryBaseSize, inventory—urrentSize, playerInventory, pathPoints, playerVehiclesVar);
        SaveLoadSystem.SaveData(saveData);
    }
    [ContextMenu("Try to load")]
    public void LoadDataFromMachine()
    {
        SaveLoadSystem.LoadData(this);
    }
}
