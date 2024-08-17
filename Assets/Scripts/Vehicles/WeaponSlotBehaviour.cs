using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSlotBehaviour : MonoBehaviour
{
    public BasicTurretBehaviour currentWeapon;
    public TurretSize SlotTurretSize = TurretSize.Small;
    private void OnValidate()
    {
        if (transform.childCount == 0)
            return;

        currentWeapon = transform.GetChild(0).GetComponent<BasicTurretBehaviour>();
        if(currentWeapon == null)
            return;

        if (currentWeapon.turretStats.TurretSize != SlotTurretSize && currentWeapon != null)
            Debug.LogWarning("Missmatch of TurretsSize in " + this.name + " in " + gameObject.scene.name);

    }
}
