using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoStats", menuName = "SO/CreateAmmoStatsSO")]
public class AmmoStats : ScriptableObject
{
    [Header("Info")]
    public string ammmoName = "BasicAmmo";
    public enum AmmoType
    {
        Bullet = 0,
        CannonShell = 1,
        LaserBatteries = 2
    }
    public AmmoType ammoType = AmmoType.Bullet;
    [Header("CombatStats")]
    public int ammoDamage = 1;
    public float onHitImpulsePower = 1f;
    public float recoilImpulsePower = 1f;
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
    #endregion Shell

    #region Laser
    [HideInInspector]
    public GameObject laserTrailVfx_Prefab;
    #endregion Laser

    #endregion AmmoSpecifics
}
