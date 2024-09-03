using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CityMenuBehaviour : MonoBehaviour
{
    public Camera cityCamera;
    void MoveCameraTo(Transform _targetTransform)
    {
        if(_targetTransform == null || cityCamera.transform.position == _targetTransform.position)
            return;
        cityCamera.transform.position = (_targetTransform.position);
        cityCamera.transform.rotation = (_targetTransform.rotation);
    }
    
    #region Shop
    public Transform shopCameraPos;
    #endregion Shop

    #region Garage
    public Transform garageCameraPos;
    #endregion Garage

    #region Map
    public Transform mapCameraPos;
    #endregion Map

    #region Test
    [ContextMenu("Move to garage")]
    void MoveCameraToGarage()
    {
        MoveCameraTo(garageCameraPos);
    }
    #endregion Test
}
