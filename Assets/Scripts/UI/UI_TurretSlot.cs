using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TurretSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TurretSlotBehaviour slotBeh;
    public TurretStats currentTurretStats;
    public TurretSize currentTurretSize = TurretSize.Small;
    public TMP_Text turretSlotSizeText;
    public TMP_Text turretName;
    public Image turretImage;
    public UnityEvent<UI_TurretSlot> onTurretSlotClicked = new UnityEvent<UI_TurretSlot>();
    public UnityEvent<TurretStats, UI_TurretSlot> changeTurretSlot = new UnityEvent<TurretStats, UI_TurretSlot>();
    public UnityEvent<TurretStats> changeTurretStats = new UnityEvent<TurretStats>();
    public void RefreshUI(string _weaponName)
    {
        turretName.text = _weaponName;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked slot " + this.name + " with " + turretName.text);
        onTurretSlotClicked.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse on slot " + this.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse off slot " + this.name);        
    }
    public void ChangeSlotTurret(TurretSlotBehaviour _slotBehaviour)
    {
        currentTurretSize = _slotBehaviour.slotTurretSize;
        turretSlotSizeText.text = currentTurretSize.ToString();
        if (_slotBehaviour.currentTurretStats == null)
        {
            turretName.text = "None";
            turretImage.sprite = null;
            currentTurretStats = null;
        }
        else
        {
            currentTurretStats = _slotBehaviour.currentTurretStats;
            turretName.text = currentTurretStats.ItemName;
            turretImage.sprite = null;
        }
    }
    public void ChangeSlotTurret(UI_BaseItemHolder _potTurr)
    {
        currentTurretSize = ((TurretStats)_potTurr.currentItemStats).TurretSize;
        turretSlotSizeText.text = currentTurretSize.ToString();
        if ((TurretStats)_potTurr.currentItemStats == null)
        {
            turretName.text = "None";
            turretImage.sprite = null;
            currentTurretStats = null;
        }
        else
        {
            currentTurretStats = (TurretStats)_potTurr.currentItemStats;
            turretName.text = currentTurretStats.ItemName;
            turretImage.sprite = null;
        }
        changeTurretStats.Invoke(currentTurretStats);
    }
    private void OnDestroy()
    {
        onTurretSlotClicked.RemoveAllListeners();
        changeTurretSlot.RemoveAllListeners();
        changeTurretStats.RemoveAllListeners();
    }
    private void OnDisable()
    {
        onTurretSlotClicked.RemoveAllListeners();
        changeTurretSlot.RemoveAllListeners();
        changeTurretStats.RemoveAllListeners();
    }
}
