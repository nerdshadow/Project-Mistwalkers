using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public enum AmmoType
    {
        Bullet = 0,
        CannonShell = 1,
        LaserBatteries = 2
    }

[CreateAssetMenu(fileName = "AmmoStats", menuName = "SO/CreateAmmoStatsSO")]
public class AmmoStats : ScriptableObject, IItemInfo
{
    [Header("Info")]
    public string ammoName = "BasicAmmo";
    public string ammoDesc = "Ammo for war";
    public AmmoType ammoType = AmmoType.Bullet;
    [Header("CombatStats")]
    public int ammoDamage = 1;
    public float onHitImpulsePower = 1f;
    public float recoilImpulsePower = 1f;
    #region Interface
    public string ItemName { get; set; }
    public ItemType ItemType { get; set; } = ItemType.Ammo;
    public int ItemValue { get; set; } = 1;
    public int ItemSize { get; set; } = 0;
    public string ItemDescription { get; set; }
    private void OnEnable()
    {
        RefreshValues();
    }
    private void OnValidate()
    {
        RefreshValues();
    }
    public void RefreshValues()
    {
        ItemName = ammoName;
        ItemDescription = ammoDesc;
    }
    #endregion Interface
    [Space(10)]
    #region AmmoSpecifics

    #region Bullet
    [HideInInspector]
    public GameObject bulletTrailVfx_Prefab;
    [HideInInspector]
    public float trailLifetime = 0.1f;
    #endregion Bullet

    #region Shell
    [HideInInspector]
    public GameObject shellPrefab;
    [HideInInspector]
    public float shellSpeed = 100f;
    [HideInInspector]
    public float shellLifeTime = 20f;
    #endregion Shell

    #region Laser
    [HideInInspector]
    public GameObject laserTrailVfx_Prefab;

    #endregion Laser

    #endregion AmmoSpecifics
}
