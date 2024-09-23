using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField]
    public TestPlayerSaveData playerSave = null;
    [SerializeField]
    int playerCurrentMoney = 0;
    [SerializeField]
    public List<ScriptableObject> playerCurrentInventory = new List<ScriptableObject>();
    public VehicleSaveVar[] playerCurrentVehicleVars = new VehicleSaveVar[5];
    public GameObject[] playerCurrentVehicles = new GameObject[5];
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
        LoadPlayerData();
        CheckPlayerZeroVehicle();
        //CheckPlayerZeroVehicle(standartVehicle, 0);
    }
    [ContextMenu("Load data")]
    private void LoadPlayerData()
    {
        if (playerSave == null)
        {
            Debug.Log("No save file");
            return;
        }
        //Load player money
        playerCurrentMoney = playerSave.playerMoney;
        //Load player last levelScene/position

        //Load player vehicles
        playerCurrentVehicleVars = playerSave.playerVehiclesVar;
        //Load player playerInventory
        playerCurrentInventory.AddRange(playerSave.playerInventory);
        //Load player statistics
    }
    [ContextMenu("Save data")]
    public void SavePlayerData()
    {
        if (playerSave == null)
        {
            Debug.Log("No save file");
            return;
        }
        //Save player money
        playerSave.playerMoney = playerCurrentMoney;
        //Save player last levelScene/position

        //Save player vehicles
        playerSave.playerVehiclesVar = playerCurrentVehicleVars;
        //Save player playerInventory
        playerSave.playerInventory = playerCurrentInventory;
        //Save player statistics
    }

    void RefreshInv()
    {
        playerCurrentInventory = new List<ScriptableObject>();
        playerCurrentInventory.AddRange(playerSave.playerInventory);
    }

    [SerializeField]
    ScriptableObject testItem = null;
    [ContextMenu("Test Add item")]
    void AddItemToInv()
    {
        playerSave.AddItemToInv(testItem);
        RefreshInv();
    }
    public void AddItemToInv(ScriptableObject _item)
    {
        if (playerSave.AddItemToInv(_item) == false)
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
        if (playerSave.RemoveItemFromInv(_item) == false)
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
        if (playerSave.playerVehiclesVar[0].vehicleBaseStats == null)
        {
            VehicleBehaviour vehBeh = standartVehicle.GetComponent<VehicleBehaviour>();
            playerSave.playerVehiclesVar[0] = new VehicleSaveVar(vehBeh.currentVehicleStats,
                                                                vehBeh.currentVehicleCab.GetComponent<VehiclePartBehaviour>().partStats,
                                                                new List<TurretStats>(),
                                                                vehBeh.currentVehicleBody.GetComponent<VehiclePartBehaviour>().partStats,
                                                                new List<TurretStats>());
        }
    }
    void CheckPlayerZeroVehicle(GameObject _vehicle, int _index)
    {
        playerSave.ChangeVehicle(_vehicle, _index);
    }
}
