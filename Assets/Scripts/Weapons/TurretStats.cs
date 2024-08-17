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
public class TurretStats : ScriptableObject
{
    [Header("Info")]
    public string turretName = "Simple Turret";
    public TurretType TurretType = TurretType.Machinegun;
    public TurretSize TurretSize = TurretSize.Small;
    public int weight = 50;
    [Header("CombatStats")]
    public float reloadSpeed = 1f;
    public float maxRange = 10f;
    public bool spreadOn = true;
    public Vector3 minSpreadAmount = new Vector3(0.1f, 0.1f, 0.1f); //No more than 1 at axis   
    public Vector3 maxSpreadAmount = new Vector3(0.2f, 0.2f, 0.2f); //No more than 1 at axis && bigger than minSpread
    public float spreadEnlargingSpeed = 1;
    public float spreadWaitTime = 1f;
    [Header("Aim")]
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 1f;
    public float verticalMaxAngle = 90f;
    [Header("VFX")]
    public Color mainColor = Color.white;
    //Trail color, shader etc
    [Header("BasePrefab")]
    public GameObject turretPrefab;
    
}
