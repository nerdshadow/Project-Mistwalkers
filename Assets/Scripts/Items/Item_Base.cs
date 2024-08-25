using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Misc = 0,
    Artifact = 1,
    Military = 2,
    Scientific = 3
}

[CreateAssetMenu(fileName = "RandItem", menuName = "SO/Item")]
public class Item_Base : ScriptableObject
{
    public string itemName = "Junk";
    public int itemValue = 1; //cost
    public ItemType itemType = ItemType.Misc; // change cost depending on city 
    public int itemSize = 1; // how much playerInventory space
    public string itemDescription = "Just junk"; //just like me
}
