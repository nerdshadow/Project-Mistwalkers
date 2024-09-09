using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using static UnityEngine.GraphicsBuffer;

public class UI_CityMenuBehaviour : MonoBehaviour
{
    public Camera cityCamera;
    public Canvas cityCanvas;
    void MoveCameraTo(Transform _targetTransform)
    {
        if(_targetTransform == null || cityCamera.transform.position == _targetTransform.position)
            return;
        cityCamera.transform.position = (_targetTransform.position);
        cityCamera.transform.rotation = (_targetTransform.rotation);
    }

    private void OnEnable()
    {
        
    }

    #region Shop
    public Transform shopCameraPos;
    public RectTransform shopItemList;

    public RectTransform playerInvItemList;
    void RefreshShop()
    {
        //for (int i = 0; i < ui_cabSlotsHolder.transform.childCount; i++)
        //    Destroy(ui_cabSlotsHolder.transform.GetChild(i).gameObject);
    }
    void RefreshPlayerInvUI()
    {
        
    }

    #endregion Shop

    #region Garage
    public Transform garageCameraPos;
    public Transform turretTestTransform;
    //public RectTransform slotTestPos;
    //public UILineRenderer lineTestRenderer;
    public RectTransform panelTestRectTranform;
    //[ContextMenu("DrawLine")]
    //void DrawLineOnCanvas()
    //{
    //    var pos1 = new Vector2(Camera.main.WorldToViewportPoint(turretTestTransform.position).x, Camera.main.WorldToViewportPoint(turretTestTransform.position).y);
    //    Debug.Log(pos1);
    //    var pos2 = new Vector2(slotTestPos.position.x, slotTestPos.position.y);
    //    Debug.Log(pos2);
    //    lineTestRenderer.Points[0] = pos1;
    //    lineTestRenderer.Points[1] = pos2;
    //}
    void MoveUIElemToWorldPos(Transform _worldObjPos, RectTransform _UIObjToMove)
    {
        if (_worldObjPos == null || _UIObjToMove == null)
        {
            Debug.LogWarning("There is no ref provided");
            return;
        }
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_worldObjPos.position);        
        //float h = Screen.height;
        //float w = Screen.width;
        //float x = screenPos.x - (w / 2);
        //float y = screenPos.y - (h / 2);
        float s = cityCanvas.scaleFactor;
        //panelTestRectTranform.anchoredPosition = new Vector2(x, y) / s;
        _UIObjToMove.anchoredPosition = new Vector2(screenPos.x, screenPos.y) / s; //Kinda works
    }
    [ContextMenu("Move infoPanel")]
    void MoveToWorldToUI()
    {
        MoveUIElemToWorldPos(turretTestTransform, panelTestRectTranform);
    }
    #endregion Garage

    #region Map
    public Transform mapCameraPos;
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
