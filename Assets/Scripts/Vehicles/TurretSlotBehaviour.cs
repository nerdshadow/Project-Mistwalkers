using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretSlotBehaviour : MonoBehaviour
{
    public TurretStats currentTurretStats;
    public TurretSize slotTurretSize = TurretSize.Small;
    public TurretBehaviour currentTurretBeh;
    [SerializeField]
    TurretStats testWeapon;
    //private void OnValidate()
    //{
    //    RefreshTurretInSlot();
    //}
    private void OnEnable()
    {
        RefreshTurretInSlot();
    }
    [ContextMenu("Refresh Turr")]
    public void RefreshTurretInSlot()
    {
        if (transform.childCount == 0)
            return;

        currentTurretBeh = transform.GetChild(0).GetComponent<TurretBehaviour>();
        currentTurretStats = transform.GetChild(0).GetComponent<TurretBehaviour>().turretStats;
        if (currentTurretStats == null)
            return;

        if(currentTurretStats.TurretSize != slotTurretSize && currentTurretStats != null)
            Debug.LogWarning("Missmatch of Turret Size in " + this.name + " in " + gameObject.scene.name);

    }
    public void SpawnTurretInSlot(TurretStats _turret)
    {
        if (_turret == null)
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                currentTurretBeh = null;
                currentTurretStats = null;
            }
            return;
        }
        if (_turret.TurretSize != slotTurretSize)
        {
            Debug.Log("Incorrect weapon Size");
            return;
        }
        if (currentTurretBeh != null)
        {
            Destroy(transform.GetChild(0).gameObject);
            currentTurretBeh = null;
            currentTurretStats = null;
        }
        GameObject buffWeapon = Instantiate(_turret.turretPrefab, transform);
        currentTurretBeh = buffWeapon.GetComponent<TurretBehaviour>();
        currentTurretStats = _turret;
    }


    [ContextMenu("TestSpawn")]
    void Spawntest()
    {
        SpawnTurretInSlot(testWeapon);
    }
}
