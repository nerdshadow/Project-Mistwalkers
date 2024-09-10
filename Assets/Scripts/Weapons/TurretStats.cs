using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    Machinegun = 0,
    Cannon = 1,
    Energy = 2
}
public enum TurretSize
{
    Small = 0,
    Medium = 1,
    Large = 2
}
[CreateAssetMenu(fileName = "TurretStats", menuName = "SO/CreateTurretStatsSO")]
public class TurretStats : ScriptableObject, IItemInfo
{
    [Header("Info")]
    public string turretName = "Simple Turret";
    public string turretDesc = "Hand made turret";
    public TurretType TurretType = TurretType.Machinegun;
    public TurretSize TurretSize = TurretSize.Small;
    public int weight = 50;

    [Header("CombatStats")]
    public float reloadSpeed = 1f;
    public float maxRange = 10f;
    public bool spreadOn = true;
    public Vector3 minSpreadAmount = new Vector3(0.01f, 0.01f, 0.01f); //No more than 1 at axis   
    public Vector3 maxSpreadAmount = new Vector3(0.02f, 0.02f, 0.02f); //No more than 1 at axis && bigger than minSpread
    public float spreadEnlargingSpeed = 1;
    public float spreadWaitTime = 1f;

    [Header("Aim")]
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 1f;
    public float verticalMaxAngle_pos = 90f; // >= 0
    public float verticalMaxAngle_neg = -90f; // <= 0

    [Header("VFX")]
    public Color mainColor = Color.white;
    //Trail color, shader etc

    [Header("BasePrefab")]
    public GameObject turretPrefab;

    #region Interface
    public string ItemName { get; set; }
    public ItemType ItemType { get; set; } = ItemType.Turret;
    public int ItemValue { get; set; } = 1;
    public int ItemSize { get; set; } = 3;
    public string ItemDescription { get; set; }
    private void OnValidate()
    {
        RefreshValues();
    }
    private void OnEnable()
    {
        RefreshValues();
    }
    public void RefreshValues()
    {
        ItemName = turretName;
    }

    #endregion Interface
}
