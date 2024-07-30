using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject ghostHorRotator = null;
    [SerializeField]
    GameObject ghostVertRotator = null;
    [SerializeField]
    GameObject HorTurret = null;
    [SerializeField]
    GameObject VertTurret = null;
    [SerializeField]
    float horizontalSpeed = 1f;
    [SerializeField]
    float verticalSpeed = 1f;
    public Transform targetTrans = null;

    void Start()
    {

    }
    private void FixedUpdate()
    {
        //FindTarget();
        TryRotateTurret();   
    }
    void Update()
    {
        
    }
    void FindTarget()
    {
        if (targetTrans != null)
            return;
    }
    void TryRotateTurret()
    {
        RotateHor();
        RotateVert();
    }
    private void RotateHor()
    {
        if(VertTurret == null)
            return;
        Debug.Log("Hor Rotating");
        ghostHorRotator.transform.LookAt(targetTrans, transform.up);

        HorTurret.transform.rotation = Quaternion.RotateTowards(HorTurret.transform.rotation, ghostHorRotator.transform.rotation, horizontalSpeed);
        HorTurret.transform.localEulerAngles = new Vector3(0.0f, HorTurret.transform.eulerAngles.y, 0.0f);

        //Quaternion rotate = Quaternion.FromToRotation(HorTurret.transform.forward, targetTrans.position - HorTurret.transform.position);
        //HorTurret.transform.localRotation = Quaternion.RotateTowards(HorTurret.transform.localRotation, rotate, horizontalSpeed);
        //HorTurret.transform.localEulerAngles = new Vector3(0.0f, HorTurret.transform.localEulerAngles.y, 0.0f);

    }

    private void RotateVert()
    {
        if (VertTurret == null)
            return;

        Debug.Log("Vert Rotating");
        ghostVertRotator.transform.LookAt(targetTrans, transform.up);

        VertTurret.transform.rotation = Quaternion.RotateTowards(VertTurret.transform.rotation, ghostVertRotator.transform.rotation, verticalSpeed);
        VertTurret.transform.localEulerAngles = new Vector3(VertTurret.transform.eulerAngles.x, 0.0f, 0.0f);

    }

}
