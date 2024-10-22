using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum ItemType
{    
    Misc = 0,
    Ammo = 1,
    Turret = 2,
    VehicleBase = 3,
    VehiclePart = 4,
    Artifact = 5,
}
[Serializable]
public enum RelatedFaction
{
    None = 0,
    Nomads = 1,
    Military = 2,
    Scientific = 3,
}

public interface IItemInfo 
{
    string ItemName { get; set; }
    ItemType ItemType { get; set; }
    RelatedFaction ItemFaction { get; set; }
    int ItemValue { get; set; }
    int ItemSize { get; set; }
    string ItemDescription { get; set; }
    void RefreshValues();
}
