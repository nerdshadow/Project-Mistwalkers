using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TurretHolder : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TurretStats currentTurretStats = null;
    public UI_WeaponSlot ui_SlotRef = null;
    [SerializeField]
    TMP_Text turretTextHolder;
    [SerializeField]
    Image turretImageHolder;
    [SerializeField]
    UI_ItemInfoPanel turretInfoPanel;
    [SerializeField]
    UI_ItemInfoPanel currentInfoPanel;
    public void ChangeHoldItem(TurretStats _turretStat)
    {
        currentTurretStats = _turretStat;
        turretTextHolder.text = _turretStat.turretName;
        //change item icon
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Create turret in slot
        if(currentTurretStats == null)
            return;
        
        Debug.Log("Item = " + currentTurretStats.turretName);
        ui_SlotRef.changeWeaponSlot.Invoke(currentTurretStats, ui_SlotRef);
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        //Debug.Log("Mouse on item " + this.name + " " + currentItemInfo.turretName);
        if(turretInfoPanel == null)
            return;
        currentInfoPanel = Instantiate(turretInfoPanel, transform.root);
        _eventData.position = Input.mousePosition;
        currentInfoPanel.itemName.text = currentTurretStats.turretName;
        currentInfoPanel.itemType.text = currentTurretStats.TurretType.ToString();
        currentInfoPanel.itemSize.text = currentTurretStats.TurretSize.ToString();
    }
    public void OnPointerExit(PointerEventData _eventData)
    {
        //Debug.Log("Mouse off item " + this.name + " " + currentItemInfo.turretName);
        if (turretInfoPanel == null || currentInfoPanel == null)
            return;
        currentInfoPanel.gameObject.SetActive(false);
        Destroy(currentInfoPanel.gameObject);
        currentInfoPanel = null;
    }

    public void OnPointerMove(PointerEventData _eventData)
    {
        if (turretInfoPanel == null || currentInfoPanel == null)
            return;
        currentInfoPanel.transform.position = _eventData.position;
    }
}
