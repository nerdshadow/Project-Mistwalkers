using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WeaponSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public WeaponSlotBehaviour slotBeh;
    public TurretStats currentTurretStats;
    public TMP_Text weaponSlotSize;
    public TMP_Text weaponName;
    public Image weaponImage;
    public UnityEvent<UI_WeaponSlot> onWeaponSlotClicked = new UnityEvent<UI_WeaponSlot>();
    public UnityEvent<TurretStats, UI_WeaponSlot> changeWeaponSlot = new UnityEvent<TurretStats, UI_WeaponSlot>()
        ;
    public void RefreshUI(string _weaponName)
    {
        weaponName.text = _weaponName;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse clicked slot " + this.name);
        onWeaponSlotClicked.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse on slot " + this.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse off slot " + this.name);        
    }
    private void OnDestroy()
    {
        onWeaponSlotClicked.RemoveAllListeners();
        changeWeaponSlot.RemoveAllListeners();
    }
    private void OnDisable()
    {
        onWeaponSlotClicked.RemoveAllListeners();
        changeWeaponSlot.RemoveAllListeners();
    }
}
