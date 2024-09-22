using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_VehicleSpawner : MonoBehaviour
{
    [SerializeField]
    VehicleSpawner currentVehicleSpawner;
    public bool stayInPlace = true;
    [SerializeField]
    TMP_Dropdown basesDropDown;
    public List<VehicleBaseStats> possibleVehiclesBases = new List<VehicleBaseStats>();

    [SerializeField]
    TMP_Dropdown cabsDropDown;
    public List<VehiclePartStats> possibleVehiclesCabs = new List<VehiclePartStats>();

    [SerializeField]
    TMP_Dropdown bodiesDropDown;
    public List<VehiclePartStats> possibleVehiclesBodies = new List<VehiclePartStats>();

    //private void OnValidate()
    //{
    //    RefreshBases();
    //    RefreshCabs();
    //    RefreshBodies();
    //}
    private void OnEnable()
    {
        cabsDropDown.onValueChanged.AddListener(delegate { RefreshCabSlots(); });
        RefreshSpawner();
    }
    private void OnDisable()
    {
        cabsDropDown.onValueChanged.RemoveListener(delegate { RefreshCabSlots(); });
    }
    [ContextMenu("Refresh Spawner")]
    void RefreshSpawner()
    {
        RefreshBases();
        RefreshCabs();
        RefreshBodies();
    }
    void RefreshBases()
    {
        if (basesDropDown == null || possibleVehiclesBases.Count == 0)
            return;

        basesDropDown.ClearOptions();
        foreach (VehicleBaseStats vehicleBaseStats in possibleVehiclesBases)
        {
            basesDropDown.options.Add(new TMP_Dropdown.OptionData(vehicleBaseStats.vehicleBaseName, null));
        }
        basesDropDown.value = 0;
        ChangeBase(0);
        basesDropDown.RefreshShownValue();
    }
    void RefreshCabs()
    {
        if (cabsDropDown == null || possibleVehiclesCabs.Count == 0)
            return;

        cabsDropDown.ClearOptions();
        foreach (VehiclePartStats vehiclePartStats in possibleVehiclesCabs)
        {
            if (vehiclePartStats.partType != PartType.Cab)
            {
                Debug.Log("There is !Cab part as " + vehiclePartStats.partName);
                continue;
            }
            if (vehiclePartStats.relatedBase != currentVehicleSpawner.vehicleBaseSO)
            {
                //do not add to list
                continue;
            }
            cabsDropDown.options.Add(new TMP_Dropdown.OptionData(vehiclePartStats.partName, null));
        }
        cabsDropDown.value = 0;        
        ChangeCab(0);
        cabsDropDown.RefreshShownValue();
    }
    void RefreshBodies()
    {
        if (bodiesDropDown == null)
            return;
        if(possibleVehiclesBodies.Count <= 0)
            bodiesDropDown.gameObject.SetActive(false);
        else
            bodiesDropDown.gameObject.SetActive(true);

        bodiesDropDown.ClearOptions();
        foreach (VehiclePartStats vehiclePartStats in possibleVehiclesBodies)
        {
            if (vehiclePartStats.partType != PartType.Body)
            {
                Debug.Log("There is !Body part as " + vehiclePartStats.partName);
                continue;
            }
            if (vehiclePartStats.relatedBase != currentVehicleSpawner.vehicleBaseSO)
            {
                //do not add to list
                continue;
            }
            bodiesDropDown.options.Add(new TMP_Dropdown.OptionData(vehiclePartStats.partName, null));
        }

        if (bodiesDropDown.options.Count == 0)
        {
            currentVehicleSpawner.vehicleBodyPartSO = null;
            bodiesDropDown.gameObject.SetActive(false);
        }
        else
        {
            bodiesDropDown.gameObject.SetActive(true);
            bodiesDropDown.value = 0;
            ChangeBody(0);
        }
        bodiesDropDown.RefreshShownValue();
    }
    public void ChangeBase(Int32 _index)
    {
        //Debug.Log("Returning base index is " + _index);
        if (currentVehicleSpawner == null)
            return;
        VehicleBaseStats potVehicleBase = null;
        foreach (VehicleBaseStats _vehBases in possibleVehiclesBases)
        {
            if (_vehBases.vehicleBaseName == basesDropDown.options[_index].text)
                potVehicleBase = _vehBases;
        }
        if (potVehicleBase == null)
        {
            Debug.Log("No such base");
            return;
        }
        currentVehicleSpawner.vehicleBaseSO = potVehicleBase;
        RefreshCabs();
        RefreshBodies();
        TrySpawnVehicle();
    }
    public void ChangeCab(Int32 _index)
    {
        //Debug.Log("Returning Cab index is " + _index);

        if (currentVehicleSpawner == null)
            return;
        VehiclePartStats potVehicleCab = null;
        foreach (VehiclePartStats _vehCab in possibleVehiclesCabs)
        {
            if (_vehCab.partName == cabsDropDown.options[_index].text)
                potVehicleCab = _vehCab;
        }
        if (potVehicleCab == null)
        {
            Debug.Log("No such Cab");
            return;
        }
        currentVehicleSpawner.vehicleCabPartSO = potVehicleCab;

        currentVehicleSpawner.ChangeCab();

        if (Application.isPlaying)
        {
            RefreshCabSlots();
            //RefreshCabSlots_AfterFrame();
        }
    }
    public void ChangeBody(Int32 _index)
    {
        //Debug.Log("Returning Body index is " + _index);

        if (currentVehicleSpawner == null)
            return;
        if (bodiesDropDown.options.Count <= 0)
            currentVehicleSpawner.vehicleBodyPartSO = null;
        VehiclePartStats potVehicleBody = null;
        foreach (VehiclePartStats _vehBody in possibleVehiclesBodies)
        {
            if (_vehBody.partName == bodiesDropDown.options[_index].text)
                potVehicleBody = _vehBody;
        }
        if (potVehicleBody == null)
        {
            Debug.Log("No such Body");
            return;
        }
        currentVehicleSpawner.vehicleBodyPartSO = potVehicleBody;

        //currentVehicleSpawner.ChangeBody();
    }
    [SerializeField]
    RectTransform ui_cabSlotsMenu = null;
    [SerializeField]
    VerticalLayoutGroup ui_cabSlotsHolder;
    public List<TurretSlotBehaviour> ui_cabSlots = new List<TurretSlotBehaviour>();
    [SerializeField]
    UI_TurretSlot ui_weaponSlot;
    [SerializeField]
    GameObject ui_itemList;
    [SerializeField]
    TurretStats nullTurret;
    [ContextMenu("Refresh Cab slots")]
    void RefreshCabSlots()
    {
        Debug.Log("Ent RefreshCabSlots");
        if (ui_cabSlotsHolder == null || ui_weaponSlot == null || ui_cabSlotsMenu == null)
            return;

        if (currentVehicleSpawner.currentVehicle == null)
        {
            StartCoroutine(RefreshCabSlots_AfterFrame());
            return;
        }


        ui_cabSlots.Clear();

        for (int i = 0; i < ui_cabSlotsHolder.transform.childCount; i++)
            Destroy(ui_cabSlotsHolder.transform.GetChild(i).gameObject);        

        //ui_cabSlots.AddRange(currentVehicleSpawner.vehicleCabPartSO.partPrefab.GetComponentsInChildren<TurretSlotBehaviour>());
        ui_cabSlots.AddRange(currentVehicleSpawner.currentVehicle.GetComponent<VehicleBehaviour>().currentVehicleCab.GetComponentsInChildren<TurretSlotBehaviour>());

        if (ui_cabSlots.Count > 0)
        {
            if (ui_cabSlotsMenu.gameObject.activeSelf == false)
                ui_cabSlotsMenu.gameObject.SetActive(true);

            foreach (TurretSlotBehaviour slot in ui_cabSlots)
            {
                UI_TurretSlot buffSlot = Instantiate(ui_weaponSlot, ui_cabSlotsHolder.transform);
                buffSlot.currentslotBeh = slot;
                buffSlot.turretSlotSizeText.text = slot.slotTurretSize.ToString();
                if (slot.currentTurretStats == null)
                {
                    slot.currentTurretStats = nullTurret;
                    //buffSlot.currentItemInfo = nullTurret;
                    //buffSlot.turretName.text = "Empty";
                }
                //else
                    buffSlot.RefreshUI(slot.currentTurretStats.turretName);

                buffSlot.onTurretSlotClicked.AddListener(ShowItemList);
                buffSlot.onTurretSlotChanged.AddListener(ChangeSlotItem);
            }
        }
        else
        {
            if (ui_cabSlotsMenu.gameObject.activeSelf == true)
                ui_cabSlotsMenu.gameObject.SetActive(false);
        }
    }
    public void ChangeSlotItem(TurretStats _turret, UI_TurretSlot _ui_slot)
    {
        if (_ui_slot == null || _turret == null || _ui_slot.currentslotBeh == null)
            return;
        //UI change
        _ui_slot.currentTurretStats = _turret;
        _ui_slot.RefreshUI(_turret.turretName);

        //OnModel change
        _ui_slot.currentslotBeh.SpawnTurretInSlot(_turret);
    }
    [SerializeField]
    List<TurretStats> turretsInStock;
    [SerializeField]
    UI_TurretHolder itemHolder;
    public void ShowItemList(UI_TurretSlot _uiSlot)
    {
        //Debug.Log("Entered SHOWITEMLIST");
        if (ui_itemList == null)
            return;
        TurretSlotBehaviour _slotBeh = _uiSlot.currentslotBeh;
        if (ui_itemList.activeSelf == true)
        {
            UI_TurretHolder[] _buffList = ui_itemList.GetComponentsInChildren<UI_TurretHolder>();
            foreach (UI_TurretHolder _buff in _buffList)
            {
                Destroy(_buff.gameObject);
            }
            ui_itemList.SetActive(false);
        }
        else
        {
            ui_itemList.SetActive(true);
            if (itemHolder == null)
                return;
            Transform _list = ui_itemList.transform.GetChild(0);

            UI_TurretHolder _nullItem = Instantiate(itemHolder, _list);
            _nullItem.ui_SlotRef = _uiSlot;


            foreach (TurretStats _turretStats in turretsInStock)
            {
                if (_turretStats.TurretSize == _slotBeh.slotTurretSize)
                {
                    UI_TurretHolder _buffItem = Instantiate(itemHolder, _list);
                    _buffItem.ChangeHoldItem(_turretStats);
                    _buffItem.ui_SlotRef = _uiSlot;
                }
            }
        }
    }

    [ContextMenu("SpawnVehicle")]
    public void TrySpawnVehicle()
    {
        if (currentVehicleSpawner == null)
            return;

        currentVehicleSpawner.AssemblyVehicle();
    }
    IEnumerator RefreshCabSlots_AfterFrame()
    {
        yield return new WaitForFixedUpdate();
        RefreshCabSlots_AfterFrame();
    }
}
