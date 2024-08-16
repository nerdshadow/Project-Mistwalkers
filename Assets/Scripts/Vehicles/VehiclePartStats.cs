using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehiclePartStats", menuName = "SO/CreateVehiclePartStatsSO")]
public class VehiclePartStats : ScriptableObject
{
    #region Info
    [Header("Info")]
    [Space(5)]
    public string partName = "Simple Part";
    public enum PartType
    {
        cab = 0,
        body = 1
    }
    public PartType partType = PartType.cab;
    public float partMass = 300f;
    public GameObject partPrefab;
    [SerializeField]
    public VehicleBaseStats relatedBase;
    public List<Transform> weaponsSlots = new List<Transform>();
    #endregion Info
}
