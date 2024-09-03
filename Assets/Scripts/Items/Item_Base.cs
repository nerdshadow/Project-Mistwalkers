using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandItem", menuName = "SO/Item")]
public class Item_Base : ScriptableObject, IItemInfo
{
    public string itemName = "Junk";
    public int itemValue = 1; //cost
    public ItemType itemType = ItemType.Misc; // change cost depending on city 
    public int itemSize = 1; // how much playerInventory space
    public string itemDescription = "Just junk"; //just like me
    private void OnValidate()
    {
        RefreshValues();
    }
    private void OnEnable()
    {
        RefreshValues();
    }
    public string ItemName { get; set; }
    public ItemType ItemType { get; set; }
    public int ItemValue { get; set; }
    public int ItemSize { get; set; }
    public string ItemDescription { get; set; }

    void RefreshValues()
    {
        ItemName = itemName;
        ItemValue = itemValue;
        ItemType = itemType;
        ItemSize = itemSize;
        ItemDescription = itemDescription;
    }
}
