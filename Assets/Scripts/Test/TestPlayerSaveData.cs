using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "PlayerSave_Test", menuName = "SO/PlayerSave_TEST")]
public class TestPlayerSaveData : ScriptableObject
{
    public string playerName = "Player";
    public int playerRandSeed = 451;
    public short playerLastCityIndex = 0;
    public int playerMoney = 100;
    public short inventoryBaseSize = 20;
    public int inventory—urrentSize = 20;
    public List<ScriptableObject> playerInventory = new List<ScriptableObject>();
    public List<VehicleBaseStats> playerVehicles = new List<VehicleBaseStats>();

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
    [ContextMenu("ClearInv")]
    public void ClearInv()
    {
        playerInventory = new List<ScriptableObject>();
    }
    //public void SaveVehicle(GameObject _vehicle)
    //{

    //} 
}
