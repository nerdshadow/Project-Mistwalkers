using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BaseItemHolder : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ScriptableObject currentItemStats = null;
    [SerializeReference]
    public IItemInfo iitemInfo;
    [SerializeField]
    TMP_Text itemTextHolder;
    [SerializeField]
    Image itemImageHolder;
    [SerializeField]
    TMP_Text itemValue;
    [SerializeField]
    UI_ItemInfoPanel itemInfoPanel;
    [SerializeField]
    UI_ItemInfoPanel currentInfoPanel;
    public UnityEvent<UI_BaseItemHolder> itemHolderClicked = new UnityEvent<UI_BaseItemHolder>();
    private void OnEnable()
    {
        if(itemHolderClicked == null)
            itemHolderClicked = new UnityEvent<UI_BaseItemHolder> ();
        ChangeHoldItemInfo(currentItemStats);
    }
    private void OnDisable()
    {
        itemHolderClicked = null;
    }
    public void ChangeHoldItemInfo(ScriptableObject _itemStats)
    {
        if (_itemStats != null && _itemStats is IItemInfo)
        {
            currentItemStats = _itemStats;
        }
        else
        {
            Debug.LogWarning("ItemStats empty or wrong cast");
            currentItemStats = null;
            return;
        }
        
        itemTextHolder.text = ((IItemInfo)currentItemStats).ItemName;
        //change item icon
        itemValue.text = ((IItemInfo)currentItemStats).ItemValue.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Create turret in slot
        if (currentItemStats == null)
            return;

        Debug.Log("Item = " + ((IItemInfo)currentItemStats).ItemName + " in " + transform.parent.name);
        itemHolderClicked.Invoke(this);
        //ui_SlotRef.changeWeaponSlot.Invoke(currentItemInfo, ui_SlotRef);
    }

    public void OnPointerEnter(PointerEventData _eventData)
    {
        //Debug.Log("Mouse on item " + this.name + " " + currentItemInfo.turretName);
        if (itemInfoPanel == null || ((IItemInfo)currentItemStats) == null)
            return;
        currentInfoPanel = Instantiate(itemInfoPanel, transform.root);
        _eventData.position = Input.mousePosition;
        currentInfoPanel.itemName.text = ((IItemInfo)currentItemStats).ItemName;
        currentInfoPanel.itemType.text = ((IItemInfo)currentItemStats).ItemType.ToString();
        currentInfoPanel.itemSize.text = (((IItemInfo)currentItemStats).ItemSize.ToString());
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
