using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandItem", menuName = "SO/Item")]
public class Item_Base : ScriptableObject, IItemInfo
{
    [SerializeField]
    string itemName = "Junk";
    [SerializeField]
    int itemValue = 1; //cost
    [SerializeField]
    ItemType itemType = ItemType.Misc;
    [SerializeField]
    RelatedFaction itemFaction = global::RelatedFaction.None; // change cost depending on city 
    [SerializeField]
    int itemSize = 1; // how much playerInventory space
    [SerializeField]
    string itemDescription = "Just junk"; //just like me
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
    public RelatedFaction ItemFaction { get; set; }

    public void RefreshValues()
    {
        ItemName = itemName;
        ItemValue = itemValue;
        ItemType = itemType;
        ItemFaction = itemFaction;
        ItemSize = itemSize;
        ItemDescription = itemDescription;
    }
}
