using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSave_Test", menuName = "SO/PlayerSave_TEST")]
public class TestPlayerSaveData : ScriptableObject
{
    public string playerName = "Player";
    public int playerRandSeed = 451;
    public short playerLastCityIndex = 0;
    public int playerMoney = 100;
    public short inventoryMaxSize = 20;
    public List<Item_Base> playerInventory = new List<Item_Base>();
    public List<VehicleBaseStats> playerVehicles = new List<VehicleBaseStats>();

    //public void AddItem(Item_Base _item)
    //{
    //    if (playerInventory.Count >= inventoryMaxSize)
    //        return;

    //    playerInventory.Add(_item);
    //}

    public void SaveVehicle(GameObject _vehicle)
    {
        
    }
}
