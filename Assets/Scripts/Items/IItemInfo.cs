using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Misc = 0,
    Artifact = 1,
    Military = 2,
    Scientific = 3,
    Ammo = 4,
    Turret = 5,
    VehicleBase =6,
    VehiclePart = 7
}

public interface IItemInfo 
{
    string ItemName { get; set; }
    ItemType ItemType { get; set; }
    int ItemValue { get; set; }
    int ItemSize { get; set; }
    string ItemDescription { get; set; }
    void RefreshValues();
}
