using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_VehicleCompHolder : MonoBehaviour
{
    TMP_Text partName;
    public ItemType compType = ItemType.VehicleBase;
    UI_BaseItemHolder compHolder;
    RectTransform turretSlots;
    [SerializeField]
    UI_TurretSlot UI_TurretSlotPrefab;
    public void ChangeName(string _newName)
    {
        partName.text = _newName;
    }
    public bool ChangeComp(ScriptableObject _potPart)
    {
        if (_potPart is not IItemInfo)
        {
            Debug.LogWarning("Item do not have IItemInfo");
            return false;
        }
        switch (compType)
        {
            case ItemType.VehicleBase:
                if (_potPart is VehicleBaseStats)
                {
                    ChangeName(((IItemInfo)_potPart).ItemName);
                    compHolder.ChangeHoldItemInfo(_potPart);
                    if(turretSlots != null)
                        Destroy(turretSlots);
                }
                break;
            case ItemType.VehiclePart:
                if (_potPart is VehiclePartStats)
                {
                    ChangeName(((IItemInfo)_potPart).ItemName);
                    compHolder.ChangeHoldItemInfo(_potPart);

                    for (int i = 0; i < turretSlots.transform.childCount; i++)
                        Destroy(turretSlots.transform.GetChild(i).gameObject);

                    List<WeaponSlotBehaviour> buffSlots = new List<WeaponSlotBehaviour>();
                    buffSlots.AddRange(((VehiclePartStats)_potPart).partPrefab.GetComponentsInChildren<WeaponSlotBehaviour>());
                    foreach (WeaponSlotBehaviour slot in buffSlots)
                    {
                        UI_TurretSlot buffUISlot = Instantiate(UI_TurretSlotPrefab, turretSlots);
                        buffUISlot.ChangeSlotTurret(slot);
                    }
                }
                break;
            default:
                Debug.LogWarning("Miss type");
                break;
        }

        return false;
    }
}
