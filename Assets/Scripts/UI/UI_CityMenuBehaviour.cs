using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
        if(_targetTransform == null || cityCamera.transform.position == _targetTransform.position)
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
        RefreshUIList(playerInvItemList, playerManager.playerSave.playerInventory);
        RefreshUIList(shopItemList, shopStock);

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
                RefreshUIList(playerInvItemList, playerManager.playerSave.playerInventory);
                RefreshUIList(shopItemList, shopStock);
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
        if(currentUIPanel != null)
            currentUIPanel.gameObject.SetActive(false);
        currentUIPanel = _UIPanel;
        currentUIPanel.gameObject.SetActive(true);
    }

    #region Shop
    [Space(10)]
    public Transform shopCameraPos;
    public RectTransform shopItemList;
    public List< ScriptableObject> shopStock = new List<ScriptableObject>();
    public RectTransform playerInvItemList;
    [SerializeField]
    UI_BaseItemHolder UI_BaseItemHolder;

    //public List<Tuple<GameObject, ScriptableObject>> tempPlayerItemsList = new List<Tuple<GameObject, ScriptableObject>>(); 
    void RefreshUIList(RectTransform _listRectTransform, List<ScriptableObject> _listItems)
    {
        for (int i = 0; i < _listRectTransform.transform.childCount; i++)
            Destroy(_listRectTransform.transform.GetChild(i).gameObject);

        foreach (ScriptableObject item in _listItems)
        {
            var buffInfo = Instantiate(UI_BaseItemHolder, _listRectTransform);
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
        RefreshUIList(playerInvItemList, playerManager.playerSave.playerInventory);
        RefreshUIList(shopItemList, shopStock);
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
    public UI_VehicleCompHolder vehicleBaseHolder;
    public UI_VehicleCompHolder vehicleCabHolder;
    public UI_VehicleCompHolder vehicleBodyHolder;
    void SpawnVehicle(int _indexOfVehicle)
    {
        if(_indexOfVehicle > playerManager.playerCurrentVehicleVars.Length || _indexOfVehicle  < 0)
        {
            Debug.LogWarning("index out of array size");
            return; 
        }
        if (currentVehicle != null)
            Destroy(currentVehicle);

        if (playerManager.playerCurrentVehicleVars[_indexOfVehicle].vehicleBaseStats == null)
        {
            Debug.Log("No vehicle data at index");
            return ;
        }

        currentVehicle = Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].vehicleBaseStats.vehicleBasePrefab, vehicleSpawnPoint.position, vehicleSpawnPoint.rotation);
        //currentVehicle.SetActive(false);
        VehicleBehaviour vehicleBehaviour = currentVehicle.GetComponent<VehicleBehaviour>();

        Destroy(vehicleBehaviour.currentVehicleCab);
        Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].cabStats.partPrefab, vehicleBehaviour.cabHolder.transform);

        if (playerManager.playerCurrentVehicleVars[_indexOfVehicle].bodyStats.partPrefab != null)
        {
            Destroy(vehicleBehaviour.currentVehicleBody);
            Instantiate(playerManager.playerCurrentVehicleVars[_indexOfVehicle].bodyStats.partPrefab, vehicleBehaviour.bodyHolder.transform);
        }

        StartCoroutine(AssebleNextFrame());
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
    }
    public void SwitchVehicle(int _toIndex)
    {
        if (_toIndex >= 0 && _toIndex < playerManager.playerCurrentVehicleVars.Length)
        {
            DespawnVehicle();
            currentVehicleIndex = _toIndex;
            currentVehiclePlace.text = "Vehicle #" + (currentVehicleIndex + 1);
            SpawnVehicle(currentVehicleIndex);
        }
        else Debug.Log("Index out of range");
    }
    void DespawnVehicle()
    {
        if (currentVehicle == null)
        {
            Debug.LogWarning("No current vehicle");
            return;
        }
        Destroy(currentVehicle);
        
        currentVehicle = null;
    }

    void CreateListOfParts()
    {
        
    }
    void CreateListOfTurrets()
    {
    
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
