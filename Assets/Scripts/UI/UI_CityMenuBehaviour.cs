using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


public class UI_CityMenuBehaviour : MonoBehaviour
{
    public enum CityPart
    {
        Shop = 0,
        Garage = 1,
        Map = 2
    }
    public Camera cityCamera;
    public Canvas cityCanvas;
    public PlayerManager playerManager;
    CityPart currentCityPart = CityPart.Shop;
    public RectTransform currentUIPanel;
    public RectTransform shopUIPanel;
    public RectTransform garageUIPanel;
    public RectTransform mapUIPanel;
    void MoveCameraTo(Transform _targetTransform)
    {
        if (_targetTransform == null || cityCamera.transform.position == _targetTransform.position)
            return;
        cityCamera.transform.position = (_targetTransform.position);
        cityCamera.transform.rotation = (_targetTransform.rotation);
    }
    private void Start()
    {
        currentCityPart = CityPart.Shop;
        currentUIPanel = shopUIPanel;
    }
    private void OnEnable()
    {
        playerManager = PlayerManager.instance;
        if(currentUIPanel == shopUIPanel)
        {
            RefreshShopUIList(playerInvItemList, playerManager.playerSave.playerInventory);
            RefreshShopUIList(shopItemList, shopStock);
        }

        //Test
        SwitchVehicle(0);
    }
    private void OnDisable()
    {
        playerManager = null;
    }

    public void ChangeCityPart(int _partNum)
    {
        if (((int)currentCityPart) == _partNum)
            return;
        else
            currentCityPart = (CityPart)_partNum;
        //Debug.Log("curr city part is " + currentCityPart.ToString());
        switch (currentCityPart)
        {
            case CityPart.Shop:
                MoveCameraTo(shopCameraPos);
                ChangeUIPanel(shopUIPanel);
                RefreshShopUIList(playerInvItemList, playerManager.playerSave.playerInventory);
                RefreshShopUIList(shopItemList, shopStock);
                break;
            case CityPart.Garage:
                MoveCameraTo(garageCameraPos);
                ChangeUIPanel(garageUIPanel);
                break;
            case CityPart.Map:
                MoveCameraTo(mapCameraPos);
                ChangeUIPanel(mapUIPanel);
                break;
            default:
                break;
        }
    }
    public void ChangeUIPanel(RectTransform _UIPanel)
    {
        if (currentUIPanel == _UIPanel)
            return;
        if (currentUIPanel != null)
            currentUIPanel.gameObject.SetActive(false);
        currentUIPanel = _UIPanel;
        currentUIPanel.gameObject.SetActive(true);
    }

    #region Shop
    [Space(10)]
    public Transform shopCameraPos;
    public RectTransform shopItemList;
    public List<ScriptableObject> shopStock = new List<ScriptableObject>();
    public RectTransform playerInvItemList;
    [SerializeField]
    UI_BaseItemHolder baseItemHolder;

    //public List<Tuple<GameObject, ScriptableObject>> tempPlayerItemsList = new List<Tuple<GameObject, ScriptableObject>>(); 
    void RefreshShopUIList(RectTransform _listRectTransform, List<ScriptableObject> _listItems)
    {
        for (int i = 0; i < _listRectTransform.transform.childCount; i++)
            Destroy(_listRectTransform.transform.GetChild(i).gameObject);

        foreach (ScriptableObject item in _listItems)
        {
            var buffInfo = Instantiate(baseItemHolder, _listRectTransform);
            buffInfo.ChangeHoldItemInfo(item);
            if (buffInfo.currentItemStats == null)
                Destroy(buffInfo.gameObject);
            else
            {
                buffInfo.itemHolderClicked.AddListener(HighlightItem);
            }
        }
        playerMoney.text = playerManager.playerSave.playerMoney.ToString();
    }
    List<UI_BaseItemHolder> highlightedItems = new List<UI_BaseItemHolder>();
    public void HighlightItem(UI_BaseItemHolder _baseItemHolder)
    {
        if (highlightedItems.Contains(_baseItemHolder) == true)
        {
            highlightedItems.Remove(_baseItemHolder);
            _baseItemHolder.GetComponentInChildren<TMP_Text>().color = Color.white;
        }
        else
        {
            highlightedItems.Add(_baseItemHolder);
            _baseItemHolder.GetComponentInChildren<TMP_Text>().color = Color.green;
        }
        RefreshTradeCost();
    }
    public TMP_Text tradeCostText;
    int tradeCost = 0;
    public TMP_Text playerMoney;
    void RefreshTradeCost()
    {
        int potCost = 0;
        foreach (UI_BaseItemHolder item in highlightedItems)
        {
            if (item.transform.parent == playerInvItemList)
                potCost += ((IItemInfo)item.currentItemStats).ItemValue;
            else if (item.transform.parent == shopItemList)
                potCost -= ((IItemInfo)item.currentItemStats).ItemValue;
            else
            {
                Debug.LogWarning("Cannot find parent list for " + item.name);
            }
        }
        tradeCost = potCost;
        tradeCostText.text = tradeCost.ToString();
    }

    public void Trade()
    {
        if (playerManager.playerSave.playerMoney + tradeCost < 0)
        {
            Debug.Log("no money(");
            return;
        }
        playerManager.playerSave.playerMoney += tradeCost;
        foreach (UI_BaseItemHolder item in highlightedItems)
        {
            if (item.transform.parent == playerInvItemList)
            {
                playerManager.RemoveItemFromInv(item.currentItemStats);
                shopStock.Add(item.currentItemStats);
            }
            else if (item.transform.parent == shopItemList)
            {
                shopStock.Remove(item.currentItemStats);
                playerManager.AddItemToInv(item.currentItemStats);
            }
        }
        highlightedItems = new List<UI_BaseItemHolder>();
        RefreshShopUIList(playerInvItemList, playerManager.playerSave.playerInventory);
        RefreshShopUIList(shopItemList, shopStock);
        RefreshTradeCost();
    }
    #endregion Shop

    #region Garage
    [Space(10)]
    public Transform garageCameraPos;
    public Transform vehicleSpawnPoint;
    public int currentVehicleIndex = -1;
    public TMP_Text currentVehiclePlace;
    public GameObject currentVehicle;

    public UI_VehicleCompHolder UIvehicleBaseHolder;
    public UI_VehicleCompHolder UIvehicleCabHolder;
    public UI_VehicleCompHolder UIvehicleBodyHolder;

    public List<TurretSlotBehaviour> TESTSLOTLIST = new List<TurretSlotBehaviour>();
    void SpawnVehicle(int _indexOfVehicle)
    {
        if (_indexOfVehicle > playerManager.playerCurrentVehicleVars.Length || _indexOfVehicle < 0)
        {
            Debug.LogWarning("index out of array size");
            return;
        }
        if (currentVehicle != null)
            Destroy(currentVehicle);

        if (playerManager.playerCurrentVehicleVars[_indexOfVehicle].vehicleBaseStats == null)
        {
            Debug.Log("No vehicle data at index");
            currentVehicle = null;
            return;
        }

        currentVehicle = Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].vehicleBaseStats.vehicleBasePrefab, vehicleSpawnPoint.position, vehicleSpawnPoint.rotation);
        //currentVehicle.SetActive(false);
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();

        //Destroy(vehicleBehaviour.currentVehicleCab.gameObject);
        DestroyImmediate(vehicleBehaviour.currentVehicleCab, false); //A bit of danger to use
        Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].cabStats.partPrefab, vehicleBehaviour.cabHolder.transform);
        List<TurretSlotBehaviour> cabSlots = new List<TurretSlotBehaviour>();
        cabSlots.AddRange(vehicleBehaviour.cabHolder.GetComponentsInChildren<TurretSlotBehaviour>(false));
        for (int i = 0; i < cabSlots.Count; i++)
        {
            cabSlots[i].SpawnTurretInSlot(playerManager.playerCurrentVehicleVars[_indexOfVehicle].cabTurrets[i]);
        }

        if ( playerManager.playerCurrentVehicleVars[_indexOfVehicle].bodyStats != null)
        {
            DestroyImmediate(vehicleBehaviour.currentVehicleBody, false);
            Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].bodyStats.partPrefab, vehicleBehaviour.bodyHolder.transform);
            List<TurretSlotBehaviour> bodySlots = new List<TurretSlotBehaviour>();
            bodySlots.AddRange(vehicleBehaviour.bodyHolder.GetComponentsInChildren<TurretSlotBehaviour>(false));
            for (int i = 0; i < bodySlots.Count; i++)
            {
                bodySlots[i].SpawnTurretInSlot(playerManager.playerCurrentVehicleVars[_indexOfVehicle].bodyTurrets[i]);
            }
        }

        StartCoroutine(AssebleNextFrame());
    }
    void SpawnVehicle(VehicleBaseStats _VehicleBase)
    {
        if (_VehicleBase == null)
        {
            Debug.LogWarning("vehicle is null");
            return;
        }
        if (currentVehicle != null)
            Destroy(currentVehicle);


        currentVehicle = Instantiate(_VehicleBase.vehicleBasePrefab, vehicleSpawnPoint.position, vehicleSpawnPoint.rotation);
        //currentVehicle.SetActive(false);
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();        

        StartCoroutine(AssebleNextFrame());
    }
    public void ChangeTurret(TurretSlotBehaviour _turretSlot, TurretStats _turretStats)
    {
        Debug.Log("Tried to spawn turret");
        _turretSlot.SpawnTurretInSlot(_turretStats);
    }
    IEnumerator AssebleNextFrame()
    {
        yield return new WaitForFixedUpdate();
        //currentVehicle.GetComponent<VehicleBehaviour>().SerializeVehicle();
        currentVehicle.SetActive(true);
        StartCoroutine(UpdateWheels());
        currentVehicle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
    IEnumerator UpdateWheels()
    {
        yield return new WaitForFixedUpdate();
        List<WheelBehaviour> wBehs = new List<WheelBehaviour>();
        wBehs.AddRange(currentVehicle.GetComponentsInChildren<WheelBehaviour>());
        foreach (WheelBehaviour wheelBehaviour in wBehs)
        {
            wheelBehaviour.ReManageWheelColliders();
        }
    }
    public void SwitchVehicle(bool _dir)
    {
        int potIndex = 0;
        if (_dir == true)
            potIndex = currentVehicleIndex + 1;
        else
            potIndex = currentVehicleIndex - 1;
        if (potIndex >= playerManager.playerCurrentVehicleVars.Length)
            potIndex = 0;
        if (potIndex < 0)
            potIndex = playerManager.playerCurrentVehicleVars.Length - 1;

        DespawnVehicle();
        currentVehicleIndex = potIndex;
        currentVehiclePlace.text = "Vehicle #" + (potIndex + 1);
        SpawnVehicle(potIndex);
        StartCoroutine(RefreshVehicleCompAfterFrame());
    }
    public void SwitchVehicle(int _toIndex)
    {
        if (_toIndex >= 0 && _toIndex < playerManager.playerCurrentVehicleVars.Length)
        {
            DespawnVehicle();
            currentVehicleIndex = _toIndex;
            currentVehiclePlace.text = "Vehicle #" + (currentVehicleIndex + 1);
            SpawnVehicle(currentVehicleIndex);
            StartCoroutine(RefreshVehicleCompAfterFrame());
        }
        else Debug.Log("Index out of range");
    }
    public void ResetVehicle()
    {
        SwitchVehicle(currentVehicleIndex);
    }
    void DespawnVehicle()
    {
        if (currentVehicle == null)
        {
            Debug.Log("No current vehicle");
            return;
        }
        Destroy(currentVehicle);

        currentVehicle = null;
    }
    [ContextMenu("RefreshVehicleComp")]
    void RefreshCurrentVehicleUIComp()
    {
        if (currentVehicle == null)
        {
            Debug.Log("No vehicle");

            UIvehicleBaseHolder.ChangeComp(null);
            UIvehicleCabHolder.ChangeComp(null);
            UIvehicleBodyHolder.ChangeComp(null);
            return ;
        }
        VehicleBehaviour buffVehBeh = currentVehicle.GetComponent<VehicleBehaviour>();
        VehiclePartBehaviour buffCabBeh = buffVehBeh.cabHolder.GetComponentInChildren<VehiclePartBehaviour>();
        VehiclePartBehaviour buffBodyBeh = null;
        if (buffVehBeh.bodyHolder != null)
        {
            buffBodyBeh = buffVehBeh.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>();
        }

        UIvehicleBaseHolder.ChangeComp(buffVehBeh);
        UIvehicleBaseHolder.compHolder.itemHolderClicked.AddListener(CreateListOfBases);

        UIvehicleCabHolder.ChangeComp(buffCabBeh);
        UIvehicleCabHolder.compHolder.itemHolderClicked.AddListener(CreateListOfCabs);

        UIvehicleBodyHolder.ChangeComp(buffBodyBeh);
        if(buffBodyBeh != null)
            UIvehicleBodyHolder.compHolder.itemHolderClicked.AddListener(CreateListOfBodies);
    }
    public RectTransform bufferList;    
    void CreateListOf(ItemType _itemType)
    {    
        switch (_itemType)
        {
            case ItemType.Turret:
                break;
            case ItemType.VehicleBase:                
                break;
            case ItemType.VehiclePart:
                break;
            default:
                break;
        }
    }
    ScriptableObject lastListType = null;
    public void ManageList(ScriptableObject _scriptable)
    {
        if (bufferList == null)
        { Debug.LogWarning("no list"); return; }
        if (bufferList.gameObject.activeInHierarchy == true && _scriptable == lastListType)
        {
            bufferList.transform.parent.gameObject.SetActive(false);
            return;
        }
        else
        {
            bufferList.transform.parent.gameObject.SetActive(true);
            lastListType = _scriptable;
        }
        //Clear list
        for (int i = 0; i < bufferList.transform.childCount; i++)
            Destroy(bufferList.transform.GetChild(i).gameObject);
    }
    [ContextMenu("Refresh List of Bases")]
    public void CreateListOfBases(UI_BaseItemHolder _uI_BaseItemHolder)
    {
        ManageList(_uI_BaseItemHolder.currentItemStats);

        //UI_BaseItemHolder buffNullItem = Instantiate(baseItemHolder, bufferList);
        //buffNullItem.ChangeHoldItemInfo(null);
        //buffNullItem.itemHolderClicked.AddListener(ChangeBase);

        foreach (ScriptableObject item in shopStock)
        {
            //Debug.Log(item + " - " + (item is VehicleBaseStats));
            if (item is VehicleBaseStats == true)
            {
                UI_BaseItemHolder buffItem = Instantiate(baseItemHolder, bufferList);
                buffItem.ChangeHoldItemInfo((VehicleBaseStats)item);
                buffItem.itemHolderClicked.AddListener(ChangeBase);
            }
        }
    }
    [ContextMenu("Refresh List of Cabs")]
    public void CreateListOfCabs(UI_BaseItemHolder _uI_BaseItemHolder)
    {
        ManageList(_uI_BaseItemHolder.currentItemStats);

        foreach (ScriptableObject item in shopStock)
        {
            if (item is VehiclePartStats == true 
                && ((VehiclePartStats)item).partType == PartType.Cab
                && ((VehiclePartStats)item).relatedBase == ((VehicleBaseStats)UIvehicleBaseHolder.compHolder.currentItemStats))
            {
                UI_BaseItemHolder buffItem = Instantiate(baseItemHolder, bufferList);
                buffItem.ChangeHoldItemInfo((VehiclePartStats)item);
                buffItem.itemHolderClicked.AddListener(ChangePart);
                //buffItem.itemHolderClicked.AddListener(ChangeBase);
            }
        }
    }
    [ContextMenu("Refresh List of Bodies")]
    public void CreateListOfBodies(UI_BaseItemHolder _uI_BaseItemHolder)
    {
        ManageList(_uI_BaseItemHolder.currentItemStats);

        foreach (ScriptableObject item in shopStock)
        {
            if (item is VehiclePartStats == true 
                && ((VehiclePartStats)item).partType == PartType.Body
                && ((VehiclePartStats)item).relatedBase == ((VehicleBaseStats)UIvehicleBaseHolder.compHolder.currentItemStats))
            {
                UI_BaseItemHolder buffItem = Instantiate(baseItemHolder, bufferList);
                buffItem.ChangeHoldItemInfo((VehiclePartStats)item);
                buffItem.itemHolderClicked.AddListener(ChangePart);
                //buffItem.itemHolderClicked.AddListener(ChangeBase);
            }
        }
    }
    public void CreateListOfTurrets(UI_TurretSlot _uI_TurretSlot)
    {
        ManageList(_uI_TurretSlot.currentTurretStats);

        foreach (ScriptableObject item in shopStock)
        {
            if (item is TurretStats == true
                && ((TurretStats)item).TurretSize == _uI_TurretSlot.currentTurretSize)
            {
                UI_BaseItemHolder buffItem = Instantiate(baseItemHolder, bufferList);
                buffItem.ChangeHoldItemInfo((TurretStats)item);
                buffItem.itemHolderClicked.AddListener(_uI_TurretSlot.ChangeSlotTurret);
                //_uI_TurretSlot.onTurretStatsChanged.AddListener(ChangeTurret);
            }
        }
    }
    void ChangeBase(UI_BaseItemHolder _potBase)
    {
        VehicleBaseStats _potBaseStats = (VehicleBaseStats)_potBase.currentItemStats;
        if (_potBaseStats == null)
        {
            Debug.LogWarning("there is no base");
            return;
        }
        SpawnVehicle(_potBaseStats);
        StartCoroutine(RefreshVehicleCompAfterFrame());
    }
    void ChangePart(UI_BaseItemHolder _potPart)
    {
        VehiclePartStats vehiclePartStats = (VehiclePartStats)_potPart.currentItemStats;
        if (vehiclePartStats == null)
        {
            Debug.LogWarning("part is null");
            return;
        }
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();
        if (vehiclePartStats.partType == PartType.Cab)
        {
            Destroy(vehicleBehaviour.currentVehicleCab);
            Instantiate(vehiclePartStats.partPrefab, vehicleBehaviour.cabHolder.transform);
        }
        else if (vehiclePartStats.partType == PartType.Body)
        {
            Destroy(vehicleBehaviour.currentVehicleBody);
            Instantiate(vehiclePartStats.partPrefab, vehicleBehaviour.bodyHolder.transform);
        }
        StartCoroutine(UpdateWheels());
        StartCoroutine(RefreshVehicleCompAfterFrame());
    }
    IEnumerator RefreshVehicleCompAfterFrame()
    {
        yield return new WaitForFixedUpdate();
        RefreshCurrentVehicleUIComp();
    }
    public void SaveCurrentVehicle()
    {
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();
        VehicleBaseStats baseVehicleStats = vehicleBehaviour.currentVehicleStats;
        VehiclePartStats cabStats = vehicleBehaviour.cabHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats;
        List<TurretSlotBehaviour> buffTurretSlots = new List<TurretSlotBehaviour>();
        buffTurretSlots.AddRange(vehicleBehaviour.cabHolder.GetComponentsInChildren<TurretSlotBehaviour>());
        List<TurretStats> cabTurrets = new List<TurretStats>();
        foreach (TurretSlotBehaviour cabTurrSlot in buffTurretSlots)
        {
            cabTurrets.Add(cabTurrSlot.currentTurretStats);
        }
        VehiclePartStats bodyStats = null;
        List<TurretStats> bodyTurrets = new List<TurretStats>();
        if (vehicleBehaviour.bodyHolder != null)
        {
            bodyStats = vehicleBehaviour.bodyHolder.GetComponentInChildren<VehiclePartBehaviour>().partStats;
            buffTurretSlots = new List<TurretSlotBehaviour>();
            buffTurretSlots.AddRange(vehicleBehaviour.bodyHolder.GetComponentsInChildren<TurretSlotBehaviour>());
            foreach (TurretSlotBehaviour bodyTurrSlot in buffTurretSlots)
            {
                bodyTurrets.Add(bodyTurrSlot.currentTurretStats);
            }
        }

        playerManager.playerCurrentVehicleVars[currentVehicleIndex] = new VehicleSaveVar(baseVehicleStats, cabStats, cabTurrets, bodyStats, bodyTurrets);
        playerManager.SavePlayerData();
    }
    public void DeleteCurrentVehicle()
    {
        playerManager.playerCurrentVehicleVars[currentVehicleIndex] = new VehicleSaveVar();
        ResetVehicle();
        playerManager.SavePlayerData();
    }

    #endregion Garage

    #region Map
    [Space(10)]
    public Transform mapCameraPos;
    public RectTransform acceptMapMoveWindow;
    public List<Button> posDestinations;
    void ChooseDestination()
    {
        
    }
    void SpawnAcceptWindow()
    {
        Instantiate(acceptMapMoveWindow, mapUIPanel);
    }
    #endregion Map

    #region Test
    [ContextMenu("Move to shop")]
    void MoveCameraToShop()
    {
        MoveCameraTo(shopCameraPos);
    }
    [ContextMenu("Move to garage")]
    void MoveCameraToGarage()
    {
        MoveCameraTo(garageCameraPos);
    }
    [ContextMenu("Move to a map")]
    void MoveCameraToMap()
    {
        MoveCameraTo(mapCameraPos);
    }
    #endregion Test
}
