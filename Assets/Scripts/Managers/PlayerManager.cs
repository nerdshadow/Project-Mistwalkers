using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField]
    TestPlayerSaveData playerSave = null;
    [SerializeField]
    int playerCurrentMoney = 0;
    [SerializeField]
    List<ScriptableObject> playerCurrentInventory = new List<ScriptableObject>();
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

        //Load player playerInventory
        playerCurrentInventory.AddRange(playerSave.playerInventory);
        //Load player statistics
    }
    [ContextMenu("Save data")]
    private void SavePlayerData()
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
    void AddItemToInv(ScriptableObject _item)
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
}
