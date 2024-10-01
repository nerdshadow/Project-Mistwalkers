using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct VehicleSaveVar
{
    public VehicleBaseStats vehicleBaseStats;
    public VehiclePartStats cabStats;
    public List<TurretStats> cabTurrets;
    public VehiclePartStats bodyStats;
    public List<TurretStats> bodyTurrets;

    public VehicleSaveVar(VehicleBaseStats _vhBS, VehiclePartStats _cSt, List<TurretStats> _cTSt, VehiclePartStats _bSt, List<TurretStats> _bTSt)
    {
        vehicleBaseStats = _vhBS;
        cabStats = _cSt;
        cabTurrets = _cTSt;
        bodyStats = _bSt;
        bodyTurrets = _bTSt;
    }
}
public struct SaveData
{
    public string playerName;
    public int playerRandSeed;
    public short playerLastCityIndex;
    public int playerMoney;
    public short inventoryBaseSize;
    public int inventoryCurrentSize;
    public List<ScriptableObject> playerInventory;
    public List<PathPoint> pathPoints;
    public VehicleSaveVar[] playerVehiclesVar;

    public SaveData(string playerName, int playerRandSeed, short playerLastCityIndex, 
        int playerMoney, short inventoryBaseSize, int inventory—urrentSize, 
        List<ScriptableObject> playerInventory, List<PathPoint> pathPoints, VehicleSaveVar[] playerVehiclesVar)
    {
        this.playerName = playerName;
        this.playerRandSeed = playerRandSeed;
        this.playerLastCityIndex = playerLastCityIndex;
        this.playerMoney = playerMoney;
        this.inventoryBaseSize = inventoryBaseSize;
        this.inventoryCurrentSize = inventory—urrentSize;
        this.playerInventory = playerInventory;
        this.pathPoints = pathPoints;
        this.playerVehiclesVar = playerVehiclesVar;
    }
}

public class SaveLoadSystem : MonoBehaviour
{
    public static string saveFilePath = Application.persistentDataPath + "/playerSave.json";
    public static void SaveData(SaveData saveData)
    {
        string savePlayerData = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, savePlayerData);
    }
    public static bool LoadData(RuntimePlayerSaveData _runtimeSave)
    {
        if (File.Exists(saveFilePath))
        {
            string loadPlayerData = File.ReadAllText(saveFilePath);
            _runtimeSave.ChangeData(JsonUtility.FromJson<SaveData>(loadPlayerData));
            return true;
        }
        else
        {
            Debug.LogWarning("No save file");
            return false;
        }
    }
    public static bool DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            return true;
        }
        else
        {
            Debug.LogWarning("No save file");
            return false;
        }
    }
}
