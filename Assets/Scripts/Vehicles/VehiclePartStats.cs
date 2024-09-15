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
    public List<WeaponSlotBehaviour> weaponsSlots = new List<WeaponSlotBehaviour>();
    public string ItemName { get; set; }
    public ItemType ItemType { get; set; }
    public int ItemValue { get; set; }
    public int ItemSize { get; set; }
    public string ItemDescription { get; set; }
    public void RefreshValues()
    {
        ItemName = partName;
        ItemValue = partValue;
        ItemType = ItemType.VehiclePart;
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
        weaponsSlots.AddRange(partPrefab.GetComponentsInChildren<WeaponSlotBehaviour>());

    }
    private void OnEnable()
    {
        RefreshValues();
    }
}
