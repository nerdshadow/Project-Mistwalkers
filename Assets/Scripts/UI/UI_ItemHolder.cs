using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemHolder : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TurretStats currentItemStats = null;
    public UI_WeaponSlot ui_SlotRef = null;
    [SerializeField]
    TMP_Text itemTextHolder;
    [SerializeField]
    Image itemImageHolder;
    [SerializeField]
    UI_ItemInfoPanel itemInfoPanel;
    [SerializeField]
    UI_ItemInfoPanel currentInfoPanel;
    public void ChangeHoldItem(TurretStats _turretStat)
    {
        currentItemStats = _turretStat;
        itemTextHolder.text = _turretStat.turretName;
        //change item icon
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Create turret in slot
        if(currentItemStats == null)
            return;
        
        Debug.Log("Item = " + currentItemStats.turretName);
        ui_SlotRef.changeWeaponSlot.Invoke(currentItemStats, ui_SlotRef);
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        //Debug.Log("Mouse on item " + this.name + " " + currentItemStats.turretName);
        if(itemInfoPanel == null)
            return;
        currentInfoPanel = Instantiate(itemInfoPanel, transform.root);
        _eventData.position = Input.mousePosition;
        currentInfoPanel.itemName.text = currentItemStats.turretName;
        currentInfoPanel.itemType.text = currentItemStats.TurretType.ToString();
        currentInfoPanel.itemSize.text = currentItemStats.TurretSize.ToString();
    }
    public void OnPointerExit(PointerEventData _eventData)
    {
        //Debug.Log("Mouse off item " + this.name + " " + currentItemStats.turretName);
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
