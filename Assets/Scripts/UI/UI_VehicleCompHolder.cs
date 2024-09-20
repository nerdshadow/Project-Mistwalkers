using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_VehicleCompHolder : MonoBehaviour
{
    public TMP_Text partName;
    public ItemType compType = ItemType.VehicleBase;
    public UI_BaseItemHolder compHolder;
    public RectTransform turretSlots;
    [SerializeField]
    UI_TurretSlot UI_TurretSlotPrefab;
    [SerializeField]
    List<UI_TurretSlot> UI_CurrentSlots = new List<UI_TurretSlot>();
    public void ChangeName(string _newName)
    {
        partName.text = _newName;
    }
    //public bool ChangeComp(ScriptableObject _potPart)
    //{
    //    if (_potPart is not IItemInfo)
    //    {
    //        Debug.LogWarning("Item do not have IItemInfo");
    //        return false;
    //    }
    //    switch (compType)
    //    {
    //        case ItemType.VehicleBase:
    //            if (_potPart is VehicleBaseStats)
    //            {
    //                ChangeName(((IItemInfo)_potPart).ItemName);
    //                compHolder.ChangeHoldItemInfo(_potPart);
    //                if(turretSlots != null)
    //                    Destroy(turretSlots);
    //            }
    //            break;
    //        case ItemType.VehiclePart:
    //            if (_potPart is VehiclePartStats)
    //            {
    //                ChangeName(((IItemInfo)_potPart).ItemName);
    //                compHolder.ChangeHoldItemInfo(_potPart);

    //                for (int i = 0; i < turretSlots.transform.childCount; i++)
    //                    Destroy(turretSlots.transform.GetChild(i).gameObject);

    //                List<TurretSlotBehaviour> buffSlots = new List<TurretSlotBehaviour>();
    //                buffSlots.AddRange(((VehiclePartStats)_potPart).partPrefab.GetComponentsInChildren<TurretSlotBehaviour>());
    //                foreach (TurretSlotBehaviour slot in buffSlots)
    //                {
    //                    UI_TurretSlot buffUISlot = Instantiate(UI_TurretSlotPrefab, turretSlots);
    //                    buffUISlot.ChangeSlotTurret(slot);
    //                }
    //            }
    //            break;
    //        default:
    //            Debug.LogWarning("Miss type");
    //            break;
    //    }

    //    return false;
    //}
    public bool ChangeComp(MonoBehaviour _potComp)
    {
        //Debug.Log(_potComp is VehicleBehaviour);
        //if (_potComp is not VehiclePartBehaviour || _potComp is not VehicleBehaviour)
        //{
        //    Debug.LogWarning("PotComp is not a Part");
        //    return false;
        //}

        if (_potComp == null)
        {
            Debug.Log(this.name + " hided");
            this.gameObject.SetActive(false);
            return true;
        }
        else
        {
            Debug.Log(this.name + " unhided");
            this.gameObject.SetActive(true);
        }
        UI_CurrentSlots = new List<UI_TurretSlot>();
        switch (compType)
        {
            case ItemType.VehicleBase:
                if (_potComp is VehicleBehaviour)
                {
                    VehicleBehaviour buffBase = (VehicleBehaviour)_potComp;
                    ChangeName(((IItemInfo)buffBase.currentVehicleStats).ItemName);
                    compHolder.ChangeHoldItemInfo(buffBase.currentVehicleStats);
                    if (turretSlots != null)
                        Destroy(turretSlots);
                    return true;
                }
                break;
            case ItemType.VehiclePart:
                if (_potComp is VehiclePartBehaviour)
                {
                    VehiclePartBehaviour buffPart = (VehiclePartBehaviour)_potComp;
                    ChangeName(((IItemInfo)buffPart.partStats).ItemName);
                    compHolder.ChangeHoldItemInfo(buffPart.partStats);

                    for (int i = 0; i < turretSlots.transform.childCount; i++)
                        Destroy(turretSlots.transform.GetChild(i).gameObject);

                    List<TurretSlotBehaviour> buffSlots = new List<TurretSlotBehaviour>();
                    buffSlots.AddRange(buffPart.GetComponentsInChildren<TurretSlotBehaviour>());
                    foreach (TurretSlotBehaviour slot in buffSlots)
                    {
                        UI_TurretSlot buffUISlot = Instantiate(UI_TurretSlotPrefab, turretSlots);
                        buffUISlot.ChangeSlotTurret(slot);
                        buffUISlot.onTurretSlotClicked.AddListener(GetComponentInParent<UI_CityMenuBehaviour>().CreateListOfTurrets);
                    }
                    return true;
                }
                break;
            default:
                Debug.LogWarning("Miss type");
                break;
        }

        return false;
    }
}
