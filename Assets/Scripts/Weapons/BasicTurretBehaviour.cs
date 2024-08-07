//using System;
using System.Collections;
using UnityEngine;

public class BasicTurretBehaviour : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField]
    protected TurretStats turretStats;
    [SerializeField]
    protected AmmoStats ammoStats;
    [Header("Vars")]
    [SerializeField]
    protected GameObject ghostHorRotator = null;
    [SerializeField]
    protected GameObject ghostVertRotator = null;
    [SerializeField]
    protected GameObject HorTurret = null;
    [SerializeField]
    protected GameObject VertTurret = null;
    [SerializeField]
    Vector3 currentSpread = Vector3.zero;
    Coroutine checkingTime = null;
    public Transform targetTrans = null;
    public Rigidbody bodyRb;
    public Transform shootPoint;
    private void OnEnable()
    {
        currentSpread = turretStats.minSpreadAmount;
    }
    private void Update()
    {
        ResetSpread();
    }
    private void FixedUpdate()
    {
        FindTarget();
        TryRotateTurret();   
    }
    public virtual void FindTarget()
    {
        if (targetTrans != null)
            return;
    }
    protected void TryRotateTurret()
    {
        if(targetTrans == null)
            return ;
        RotateHor();
        RotateVert();
    }
    protected void RotateHor()
    {
        if(VertTurret == null)
            return;
        //Debug.Log("Hor Rotating");
        ghostHorRotator.transform.LookAt(targetTrans, transform.up);

        HorTurret.transform.rotation = Quaternion.RotateTowards(HorTurret.transform.rotation, ghostHorRotator.transform.rotation, turretStats.horizontalSpeed);
        HorTurret.transform.localEulerAngles = new Vector3(0.0f, HorTurret.transform.eulerAngles.y, 0.0f);

        //Quaternion rotate = Quaternion.FromToRotation(HorTurret.transform.forward, targetTrans.position - HorTurret.transform.position);
        //HorTurret.transform.localRotation = Quaternion.RotateTowards(HorTurret.transform.localRotation, rotate, horizontalSpeed);
        //HorTurret.transform.localEulerAngles = new Vector3(0.0f, HorTurret.transform.localEulerAngles.y, 0.0f);

    }
    protected void RotateVert()
    {
        if (VertTurret == null)
            return;

        //Debug.Log("Vert Rotating");
        ghostVertRotator.transform.LookAt(targetTrans, transform.up);

        VertTurret.transform.rotation = Quaternion.RotateTowards(VertTurret.transform.rotation, ghostVertRotator.transform.rotation, turretStats.verticalSpeed);
        VertTurret.transform.localEulerAngles = new Vector3(VertTurret.transform.eulerAngles.x, 0.0f, 0.0f);

    }
    public virtual void TryToShoot()
    {
        Debug.Log(this.name + " gun tried to shoot");
        switch (ammoStats.ammoType)
        {
            case AmmoStats.AmmoType.Bullet:
                ShootBullet();
                break;
            case AmmoStats.AmmoType.CannonShell:
                ShootSheel();
                break;
            case AmmoStats.AmmoType.LaserBatteries:
                ShootLaser();
                break;
            default:
                Debug.Log(this.name + " gun dont have ammo type");
                break;
        }
    }
    void ShootBullet() 
    {
        if (checkingTime == null)
        {
            checkingTime = StartCoroutine(CheckTimer());
        }
        else
        {
            StopCoroutine(checkingTime);
            checkingTime = StartCoroutine(CheckTimer());
        }

        ActivateRecoil();

        Vector3 shootDir = shootPoint.forward + new Vector3(Random.Range(-currentSpread.x, currentSpread.x),
                                                        Random.Range(-currentSpread.y, currentSpread.y),
                                                        Random.Range(-currentSpread.z, currentSpread.z));
        shootDir.Normalize();
        if (Physics.Raycast(shootPoint.position, shootDir, out RaycastHit hit, turretStats.maxRange * 2f))
        {
            //currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, hit.point);

            //hit.collider.GetComponentInParent<IDestroyable>()?.ChangeHealth(-gunDamage);

            //if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //{
            //    hit.collider.GetComponentInParent<CharacterStats>().ChangeHealth(-gunDamage);
            //}

            //if (hit.collider.GetComponentInParent<Rigidbody>() != null)
            //{
            //    Vector3 forceVector = (hit.point - shootPoint.position);
            //    if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //        forceVector.y = 0;
            //    forceVector = forceVector.normalized;
            //    hit.collider.GetComponentInParent<Rigidbody>().AddForce(forceVector * impulsePower, ForceMode.Impulse);
            //}
        }
        else
        {
            //currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, shootPoint.position + (shootDir * gunRange));
        }
    }
    void ShootSheel()
    {
    }
    void ShootLaser()
    {
    }
    public virtual void ActivateRecoil()
    {
        if (bodyRb == true)
        {
            Vector3 forceVector = (bodyRb.position - shootPoint.position);
            //forceVector.y = 0;
            forceVector = forceVector.normalized;
            bodyRb.AddForce(forceVector * ammoStats.recoilImpulsePower, ForceMode.Impulse);
        }
    }
    void ResetSpread()
    {
        if (checkingTime == null)
        {
            if (currentSpread.x != turretStats.minSpreadAmount.x)
                currentSpread.x = Mathf.Lerp(currentSpread.x, turretStats.minSpreadAmount.x, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
            if (currentSpread.y != turretStats.minSpreadAmount.y)
                currentSpread.y = Mathf.Lerp(currentSpread.y, turretStats.minSpreadAmount.y, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
            if (currentSpread.z != turretStats.minSpreadAmount.z)
                currentSpread.z = Mathf.Lerp(currentSpread.z, turretStats.minSpreadAmount.z, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
        }
    }
    IEnumerator CheckTimer()
    {
        currentSpread.x = Mathf.Lerp(currentSpread.x, turretStats.maxSpreadAmount.x, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.y = Mathf.Lerp(currentSpread.y, turretStats.maxSpreadAmount.y, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.z = Mathf.Lerp(currentSpread.z, turretStats.maxSpreadAmount.z, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        yield return new WaitForSeconds(turretStats.spreadWaitTime);
        StopCoroutine(checkingTime);
        checkingTime = null;
        yield break;
    }
}
