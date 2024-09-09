using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BaseItemHolder : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public IItemInfo currentItemInfo = null;

    [SerializeField]
    TMP_Text itemTextHolder;
    [SerializeField]
    Image itemImageHolder;
    [SerializeField]
    UI_ItemInfoPanel itemInfoPanel;
    [SerializeField]
    UI_ItemInfoPanel currentInfoPanel;
    public void ChangeHoldItemInfo(IItemInfo _itemInfo)
    {
        currentItemInfo = _itemInfo;
        itemTextHolder.text = currentItemInfo.ItemName;
        //change item icon
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Create turret in slot
        if (currentItemInfo == null)
            return;

        Debug.Log("Item = " + currentItemInfo.ItemName);
        //ui_SlotRef.changeWeaponSlot.Invoke(currentItemInfo, ui_SlotRef);
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        //Debug.Log("Mouse on item " + this.name + " " + currentItemInfo.turretName);
        if (itemInfoPanel == null)
            return;
        currentInfoPanel = Instantiate(itemInfoPanel, transform.root);
        _eventData.position = Input.mousePosition;
        currentInfoPanel.itemName.text = currentItemInfo.ItemName;
        currentInfoPanel.itemType.text = currentItemInfo.ItemType.ToString();
        currentInfoPanel.itemAddInfo.text = (currentItemInfo.ItemValue + " $");
    }
    public void OnPointerExit(PointerEventData _eventData)
    {
        //Debug.Log("Mouse off item " + this.name + " " + currentItemInfo.turretName);
        if (itemInfoPanel == null || currentInfoPanel == null)
            return;
        currentInfoPanel.gameObject.SetActive(false);
        Destroy(currentInfoPanel.gameObject);
        currentInfoPanel = null;
    }

    public void OnPointerMove(PointerEventData _eventData)
    {
        if (itemInfoPanel == null || currentInfoPanel == null)
            return;
        currentInfoPanel.transform.position = _eventData.position;
    }
}
