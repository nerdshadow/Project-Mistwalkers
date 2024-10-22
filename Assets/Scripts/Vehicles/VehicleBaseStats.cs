using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleBaseStats", menuName = "SO/CreateVehicleStatsSO")]
public class VehicleBaseStats : ScriptableObject, IItemInfo
{
    [Header("Info")]
    public string vehicleBaseName = "Simple Vehicle Base";
    public int vehicleBaseValue = 1000;
    public int vehicleBaseSize = 25;
    public string vehicleBaseDesc = "Vehicle base";
    public float vehicleBaseMass = 1000f;
    [Header("MovementStats")]
    public float maxSpeed = 60f;
    public float maxRpm = 500f;
    public float forwardTorque = 100f;
    public float backwardTorque = 100f;
    public float brakeTorque = 80f;
    public float maxTurnAngle = 15f;
    public float turnSpeed = 1f;
    [Header("VFX")]
    public Color baseColor = Color.white;
    [Header("BasePrefab")]
    public GameObject vehicleBasePrefab;

    public string ItemName { get; set; }
    public ItemType ItemType { get; set; } = ItemType.VehicleBase;
    public RelatedFaction ItemFaction { get; set; } = RelatedFaction.None;
    public int ItemValue { get; set; }
    public int ItemSize { get; set; }
    public string ItemDescription { get; set; }
    public void RefreshValues()
    {
        ItemName = vehicleBaseName;
        ItemValue = vehicleBaseValue;
        ItemType = ItemType.VehicleBase;
        ItemFaction = RelatedFaction.None;
        ItemSize = vehicleBaseSize;
        ItemDescription = vehicleBaseDesc;
    }
    private void OnValidate()
    {
        RefreshValues();   
    }
    private void OnEnable()
    {
        RefreshValues();
    }
}
