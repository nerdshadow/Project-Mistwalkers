using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text weaponSlotSize;
    public TMP_Text weaponName;
    public Image weaponImage;
    public UnityEvent<string> onWeaponSlotClicked = new UnityEvent<string>();
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked slot " + this.name);
        onWeaponSlotClicked.Invoke(weaponSlotSize.text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse on slot " + this.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse off slot " + this.name);        
    }
    private void OnDestroy()
    {
        onWeaponSlotClicked.RemoveAllListeners();
    }
    private void OnDisable()
    {
        onWeaponSlotClicked.RemoveAllListeners();
    }
}
