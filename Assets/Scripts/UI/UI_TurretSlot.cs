using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TurretSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public WeaponSlotBehaviour slotBeh;
    public TurretStats currentTurretStats;
    public TurretSize currentTurretSize = TurretSize.Small;
    public TMP_Text turretSlotSizeText;
    public TMP_Text turretName;
    public Image turretImage;
    public UnityEvent<UI_TurretSlot> onTurretSlotClicked = new UnityEvent<UI_TurretSlot>();
    public UnityEvent<TurretStats, UI_TurretSlot> changeTurretSlot = new UnityEvent<TurretStats, UI_TurretSlot>()
        ;
    public void RefreshUI(string _weaponName)
    {
        turretName.text = _weaponName;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse clicked slot " + this.name);
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
    public void ChangeSlotTurret(WeaponSlotBehaviour _slotBehaviour)
    {
        currentTurretSize = _slotBehaviour.slotTurretSize;
        turretSlotSizeText.text = currentTurretSize.ToString();
        if (_slotBehaviour.currentWeaponStats == null)
        {
            turretName.text = "None";
            turretImage.sprite = null;
            currentTurretStats = null;
        }
        else
        {
            currentTurretStats = _slotBehaviour.currentWeaponStats;
            turretName.text = currentTurretStats.ItemName;
            turretImage.sprite = null;
        }
    }
    private void OnDestroy()
    {
        onTurretSlotClicked.RemoveAllListeners();
        changeTurretSlot.RemoveAllListeners();
    }
    private void OnDisable()
    {
        onTurretSlotClicked.RemoveAllListeners();
        changeTurretSlot.RemoveAllListeners();
    }
}
