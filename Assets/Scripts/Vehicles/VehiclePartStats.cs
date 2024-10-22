using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PartType
{
    Cab = 0,
    Body = 1
}
[CreateAssetMenu(fileName = "VehiclePartStats", menuName = "SO/CreateVehiclePartStatsSO")]
public class VehiclePartStats : ScriptableObject, IItemInfo
{
    #region Info
    [Header("Info")]
    [Space(5)]
    public string partName = "Simple Part";
    public PartType partType = PartType.Cab;
    public int partValue = 100;
    public int partSize = 10;
    public string partDescription = "Simple vehicle part";
    public float partMass = 300f;
    public GameObject partPrefab;
    [SerializeField]
    public VehicleBaseStats relatedBase;
    public List<TurretSlotBehaviour> weaponsSlots = new List<TurretSlotBehaviour>();
    public string ItemName { get; set; }
    public ItemType ItemType { get; set; } = ItemType.VehiclePart;
    public RelatedFaction ItemFaction { get; set; } = RelatedFaction.None;
    public int ItemValue { get; set; }
    public int ItemSize { get; set; }
    public string ItemDescription { get; set; }
    public void RefreshValues()
    {
        ItemName = partName;
        ItemValue = partValue;
        ItemType = ItemType.VehiclePart;
        ItemFaction = RelatedFaction.None;
        ItemSize = partSize;
        ItemDescription = partDescription;
    }
    #endregion Info
    private void OnValidate()
    {
        RefreshValues();
        weaponsSlots.Clear();
        if (partPrefab == null)
            return;
        weaponsSlots.AddRange(partPrefab.GetComponentsInChildren<TurretSlotBehaviour>());

    }
    private void OnEnable()
    {
        RefreshValues();
    }
}
