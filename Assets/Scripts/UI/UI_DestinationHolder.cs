using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_DestinationHolder : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string destinationName = "Basic Dest";
    public MapType destinationType = MapType.Sand;
    public SceneField destinationSceneField = null;
    [SerializeField]
    UI_ItemInfoPanel destinationInfoPanel = null;
    public UI_ItemInfoPanel currentDestinationInfoPanel = null;
    public UnityEvent onDestinationClicked = new UnityEvent();
    private void OnEnable()
    {
        GetComponentInChildren<TMP_Text>().text = destinationName;
        if(onDestinationClicked == null)
            onDestinationClicked = new UnityEvent();
    }
    private void OnDisable()
    {
        onDestinationClicked = null;
    }
    public void ChangeDestination(string _name, MapType _mapType, SceneField _sceneField)
    {
        destinationName = _name;
        GetComponentInChildren<TMP_Text>().text = destinationName;
        destinationType = _mapType;
        destinationSceneField = _sceneField;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
        onDestinationClicked.Invoke();
        //ui_SlotRef.onTurretSlotChanged.Invoke(currentItemInfo, ui_SlotRef);
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        //Debug.Log("Mouse on item " + this.name + " " + currentItemInfo.turretName);
        if (destinationInfoPanel == null)
            return;

        currentDestinationInfoPanel = Instantiate(destinationInfoPanel, transform.root);
        _eventData.position = Input.mousePosition;
        currentDestinationInfoPanel.itemName.text = destinationName;
        currentDestinationInfoPanel.itemType.text = destinationType.ToString();
        currentDestinationInfoPanel.itemSize.text = "1";
    }
    public void OnPointerExit(PointerEventData _eventData)
    {
        //Debug.Log("Mouse off item " + this.name + " " + currentItemInfo.turretName);
        if (destinationInfoPanel == null || currentDestinationInfoPanel == null)
            return;
        currentDestinationInfoPanel.gameObject.SetActive(false);
        Destroy(currentDestinationInfoPanel.gameObject);
        currentDestinationInfoPanel = null;
    }

    public void OnPointerMove(PointerEventData _eventData)
    {
        if (destinationInfoPanel == null || currentDestinationInfoPanel == null)
            return;
        currentDestinationInfoPanel.transform.position = _eventData.position;
    }
}
