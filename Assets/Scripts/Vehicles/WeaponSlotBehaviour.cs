using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSlotBehaviour : MonoBehaviour
{
    public TurretStats currentWeaponStats;
    public TurretSize SlotTurretSize = TurretSize.Small;
    public TurretBehaviour currentWeaponBeh;
    [SerializeField]
    TurretStats testWeapon;
    private void OnValidate()
    {
        RefreshWeaponInSlot();
    }
    public void RefreshWeaponInSlot()
    {
        if (transform.childCount == 0)
            return;

        currentWeaponStats = transform.GetChild(0).GetComponent<TurretBehaviour>().turretStats;
        if (currentWeaponStats == null)
            return;

        if(currentWeaponStats.TurretSize != SlotTurretSize && currentWeaponStats != null)
            Debug.LogWarning("Missmatch of Turret Size in " + this.name + " in " + gameObject.scene.name);
    }
    public void SpawnWeaponInSlot(TurretStats _turret)
    {
        if (_turret == null)
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                currentWeaponBeh = null;
                currentWeaponStats = null;
            }
            return;
        }
        if (_turret.TurretSize != SlotTurretSize)
        {
            Debug.Log("Incorrect weapon Size");
            return;
        }
        if (currentWeaponBeh != null)
        {
            Destroy(transform.GetChild(0).gameObject);
            currentWeaponBeh = null;
            currentWeaponStats = null;
        }
        GameObject buffWeapon = Instantiate(_turret.turretPrefab, transform);
        currentWeaponBeh = buffWeapon.GetComponent<TurretBehaviour>();
        currentWeaponStats = _turret;
    }


    [ContextMenu("TestSpawn")]
    void Spawntest()
    {
        SpawnWeaponInSlot(testWeapon);
    }
}
