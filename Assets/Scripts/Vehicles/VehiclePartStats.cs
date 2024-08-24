using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum PartType
    {
        Cab = 0,
        Body = 1
    }
[CreateAssetMenu(fileName = "VehiclePartStats", menuName = "SO/CreateVehiclePartStatsSO")]
public class VehiclePartStats : ScriptableObject
{
    #region Info
    [Header("Info")]
    [Space(5)]
    public string partName = "Simple Part";
    public PartType partType = PartType.Cab;
    public float partMass = 300f;
    public GameObject partPrefab;
    [SerializeField]
    public VehicleBaseStats relatedBase;
    public List<WeaponSlotBehaviour> weaponsSlots = new List<WeaponSlotBehaviour>();
    #endregion Info
    private void OnValidate()
    {
        weaponsSlots.Clear();
        if (partPrefab == null)
            return;

        weaponsSlots.AddRange(partPrefab.GetComponentsInChildren<WeaponSlotBehaviour>());
    }    
}
