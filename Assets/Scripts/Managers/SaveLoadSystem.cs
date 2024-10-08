using System;
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
[Serializable]
public struct PlayerSaveData
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

    public PlayerSaveData(string playerName, int playerRandSeed, short playerLastCityIndex, 
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
public struct SettingsSaveData
{
    //graphic
    public bool fullscreenMode;
    //audio
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public float voicevolume;

    public SettingsSaveData(bool fscreen, float mastervolume, float sfxvolume, float musicvolume, float voicevolume)
    {
        this.fullscreenMode = fscreen;
        this.masterVolume = mastervolume;
        this.sfxVolume = sfxvolume;
        this.musicVolume = musicvolume;
        this.voicevolume = voicevolume;
    }
}

public static class SaveLoadSystem
{
    public static string saveFilePath = Application.persistentDataPath + "/playerSave.json";
    public static string settingsFilePath = Application.persistentDataPath + "/Settings.json";
    public static SettingsSaveData defaultSettings = new SettingsSaveData(true, 1, 1, 1, 1);    
    
    public static void SavePlayerData(PlayerSaveData saveData)
    {
        string savePlayerData = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, savePlayerData);
    }
    public static bool LoadPlayerData(RuntimePlayerSaveData _runtimeSave)
    {
        if (File.Exists(saveFilePath))
        {
            string loadPlayerData = File.ReadAllText(saveFilePath);
            _runtimeSave.ChangeData(JsonUtility.FromJson<PlayerSaveData>(loadPlayerData));
            return true;
        }
        else
        {
            Debug.LogWarning("No save file");
            return false;
        }
    }
    public static bool DeletePlayerSaveData()
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

    public static void SaveSettingsData(SettingsSaveData _settingsData)
    {
        string saveSettingsData = JsonUtility.ToJson(_settingsData);
        File.WriteAllText(settingsFilePath, saveSettingsData);
    }
    public static bool LoadSeetingsData()
    {
        if (File.Exists(settingsFilePath))
        {
            string loadSettingsData = File.ReadAllText(settingsFilePath);
            SettingsSaveData settingsPot = JsonUtility.FromJson<SettingsSaveData>(loadSettingsData);
            Screen.fullScreen = settingsPot.fullscreenMode;
            //try at risk of blowing up
            AudioManager.instance.SetMasterVolume(settingsPot.masterVolume);
            AudioManager.instance.SetMusicVolume(settingsPot.musicVolume);
            AudioManager.instance.SetSFXVolume(settingsPot.sfxVolume);
            AudioManager.instance.SetVoiceVolume(settingsPot.voicevolume);
            //
            return true;
        }
        else
        {
            Debug.LogWarning("no settings data");
            return false;
        }
    }
    public static void ResetSettingsData()
    {
        string saveSettingsData = JsonUtility.ToJson(defaultSettings);
        File.WriteAllText(settingsFilePath, saveSettingsData);
        LoadSeetingsData();
    }
}
