//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
[SerializeField]
public enum FireRateType
{
    Automatic = 0,
    Burst = 1
}
public class BasicTurretBehaviour : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField]
    public TurretStats turretStats;
    [SerializeField]
    protected AmmoStats ammoStats;
    [Header("Vars")]
    [SerializeField]
    protected GameObject ghostHorRotator = null;
    [SerializeField]
    protected GameObject ghostVertRotator = null;
    public bool limitVertRotation = false;
    [SerializeField]
    protected GameObject HorTurret = null;
    [SerializeField]
    protected GameObject VertTurret = null;
    public bool canFire = false;
    bool reloading = false;
    [SerializeField]
    FireRateType fireRateType = FireRateType.Automatic;
    public int burstMaxSize = 3;
    int burstCurrentSize;
    [SerializeField]
    Vector3 currentSpread = Vector3.zero;
    Coroutine checkingTime = null;
    public Transform targetTrans = null;
    [HideInInspector]
    public Rigidbody bodyRb;
    public List<Transform> shootPoints = new List<Transform>();
    [HideInInspector]
    public Transform currentShootPoint;
    [HideInInspector]
    public Transform lastShootPoint = null;

    UnityAction MGShoot;
    UnityAction SShoot;
    UnityAction LShoot;

    private void OnValidate()
    {
        //Update things
        if (shootPoints.Count < 1)
        {
            Debug.LogWarning("No shootpoints at " + this.name);
            currentShootPoint = transform;
            //currentShootPoint.position = new Vector3(currentShootPoint.position.x, currentShootPoint.position.y + 0.2f, currentShootPoint.position.z);
        }
        else
            currentShootPoint = shootPoints[0];
    }
    private void OnEnable()
    {
        currentSpread = turretStats.minSpreadAmount;
        if (shootPoints.Count < 1)
        {
            Debug.LogWarning("No shootpoints at " + this.name);
            currentShootPoint = transform;
            currentShootPoint.position = new Vector3(currentShootPoint.position.x, currentShootPoint.position.y + 0.2f, currentShootPoint.position.z);
        }
        else
            currentShootPoint = shootPoints[0];

        MGShoot += ShootBullet;
        SShoot += ShootShell;
        LShoot += ShootLaser;
    }
    private void OnDisable()
    {
        MGShoot -= ShootBullet;
        SShoot -= ShootShell;
        LShoot -= ShootLaser;        
    }
    private void Update()
    {
        ResetSpread();
    }
    private void FixedUpdate()
    {
        //FindTarget();
        TryRotateTurret();
        Fire();
    }
    public void ChangeTargetTo(GameObject _newTarget)
    {
        targetTrans = _newTarget.transform;
    }
    public void ChangeTargetTo(Transform _newTargetTrans)
    {
        targetTrans = _newTargetTrans;
    }
    public void ClearTarget() //If changing name -> change next func too
    {
        targetTrans = null;
    }
    public void ClearTarget(float time)
    {
        Invoke("ClearTarget", time);
    }
    [SerializeField]
    Transform centerOfArea = null;
    public virtual void FindTarget()
    {
        if (centerOfArea == null)
        {
            centerOfArea = transform.root.transform;            
        }
        if (targetTrans != null)
        {
            if (Vector3.Distance(centerOfArea.position, targetTrans.position) > turretStats.maxRange)
                ClearTarget();
            return;
        }
        Collider[] potTargets = Physics.OverlapSphere(centerOfArea.position, turretStats.maxRange * 1.1f);
        foreach (Collider potTarget in potTargets)
        {
            if (potTarget.isTrigger == false)
            {
                ChangeTargetTo(potTarget.transform);
                break;
            }
        }
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
        if(HorTurret == null)
            return;

        Vector3 _lookDirection = (targetTrans.position - ghostHorRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostHorRotator.transform.rotation = Quaternion.RotateTowards(ghostHorRotator.transform.rotation, _lookRotation, turretStats.horizontalSpeed);
        HorTurret.transform.localEulerAngles = new Vector3(0.0f, ghostHorRotator.transform.localEulerAngles.y, 0.0f);

    }
    protected void RotateVert()
    {
        if (VertTurret == null)
            return;

        Vector3 _lookDirection = (targetTrans.position - ghostVertRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostVertRotator.transform.rotation = Quaternion.RotateTowards(ghostVertRotator.transform.rotation, _lookRotation, turretStats.verticalSpeed);
        float targetAngle = ghostVertRotator.transform.localEulerAngles.x;
        //if (limitVertRotation == true)
        //{
            Debug.Log("Start targetAngel = " + targetAngle);

            float diff = 180 - targetAngle;
            if (diff < 0)
            {
                if (targetAngle < 360 - turretStats.verticalMaxAngle_pos)
                {
                    Debug.Log("Smaller");
                    targetAngle = 360 - turretStats.verticalMaxAngle_pos;
                }
            }
            else
            {
                if (targetAngle > -turretStats.verticalMaxAngle_neg)
                {
                    Debug.Log("Smaller");
                    targetAngle = -turretStats.verticalMaxAngle_neg;
                }
            }
            Debug.Log("After targetAngel = " + targetAngle);
        //}
        VertTurret.transform.localEulerAngles = new Vector3(targetAngle, 0.0f, 0.0f);
    }

    void Fire()
    {
        if (canFire != true || reloading == true)
            return;
        TryToShoot();
        StartCoroutine(Reload());
    }
    [ContextMenu("Shoot")]
    public virtual void TryToShoot()
    {
        Debug.Log(this.name + " gun tried to shoot");
        if (fireRateType == FireRateType.Burst)
            burstCurrentSize = burstMaxSize;
        switch (ammoStats.ammoType)
        {
            case AmmoType.Bullet:
                ShootBullet();
                break;
            case AmmoType.CannonShell:
                ShootShell();
                break;
            case AmmoType.LaserBatteries:
                ShootLaser();
                break;
            default:
                Debug.Log(this.name + " gun dont have ammo type");
                break;
        }
    }
    void ChangeShootPoint()
    {
        if (shootPoints.Count <= 1)
            return;
        lastShootPoint = currentShootPoint;
        currentShootPoint = shootPoints[Random.Range(0, shootPoints.Count)];
        if (currentShootPoint == lastShootPoint)
            ChangeShootPoint();
    }
    void ShootBullet() 
    {
        GameObject currentBulletTrail;
        currentBulletTrail = Instantiate(ammoStats.bulletTrailVfx_Prefab, currentShootPoint.position, currentShootPoint.rotation);
        currentBulletTrail.GetComponent<BulletTrail>().lineLifetime = ammoStats.trailLifetime;
        currentBulletTrail.GetComponent<LineRenderer>().SetPosition(0, currentShootPoint.position);
        currentBulletTrail.GetComponent<BulletTrail>().StartTimer();
        currentBulletTrail.SetActive(true);

        if (checkingTime == null)
        {
            checkingTime = StartCoroutine(CheckSpreadTimer());
        }
        else
        {
            StopCoroutine(checkingTime);
            checkingTime = StartCoroutine(CheckSpreadTimer());
        }

        ActivateRecoil();

        Vector3 shootDir = currentShootPoint.forward + new Vector3(Random.Range(-currentSpread.x, currentSpread.x),
                                                        Random.Range(-currentSpread.y, currentSpread.y),
                                                        Random.Range(-currentSpread.z, currentSpread.z));
        shootDir.Normalize();
        if (Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, turretStats.maxRange * 2f))
        {
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, hit.point);

            //hit.collider.GetComponentInParent<IDestroyable>()?.ChangeHealth(-gunDamage);

            //if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //{
            //    hit.collider.GetComponentInParent<CharacterStats>().ChangeHealth(-gunDamage);
            //}

            //if (hit.collider.GetComponentInParent<Rigidbody>() != null)
            //{
            //    Vector3 forceVector = (hit.point - currentShootPoint.position);
            //    if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //        forceVector.y = 0;
            //    forceVector = forceVector.normalized;
            //    hit.collider.GetComponentInParent<Rigidbody>().AddForce(forceVector * impulsePower, ForceMode.Impulse);
            //}
        }
        else
        {
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, currentShootPoint.position + (shootDir * turretStats.maxRange * 2f));
        }
        currentBulletTrail = null;
        if (fireRateType == FireRateType.Burst)
        {
            burstCurrentSize -= 1;
            if (burstCurrentSize > 0)
                StartCoroutine(BurstFireReload(MGShoot));
        }
        ChangeShootPoint(); //Change after shoot
    }
    void ShootShell()
    {
        Debug.Log("Tried to shoot Shell");

        Vector3 shootDir = currentShootPoint.forward + new Vector3(Random.Range(-currentSpread.x, currentSpread.x),
                                                         Random.Range(-currentSpread.y, currentSpread.y),
                                                         Random.Range(-currentSpread.z, currentSpread.z));
        shootDir.Normalize();

        GameObject currentShell = Instantiate(ammoStats.shellPrefab, currentShootPoint.position, currentShootPoint.rotation);
        currentShell.transform.rotation = Quaternion.LookRotation(shootDir);
        currentShell.GetComponent<ProjectileBehaviour>().impulsePower = ammoStats.onHitImpulsePower;
        currentShell.GetComponent<Rigidbody>().AddForce(currentShell.transform.forward * ammoStats.shellSpeed);
        currentShell = null;

        ActivateRecoil();

        if (fireRateType == FireRateType.Burst)
        {
            burstCurrentSize -= 1;
            if (burstCurrentSize > 0)
                StartCoroutine(BurstFireReload(SShoot));
        }
        ChangeShootPoint(); //Change after shoot
    }
    void ShootLaser()
    {
        GameObject currentLaserTrail;
        currentLaserTrail = Instantiate(ammoStats.laserTrailVfx_Prefab, currentShootPoint.position, currentShootPoint.rotation);
        currentLaserTrail.GetComponent<BulletTrail>().lineLifetime = ammoStats.trailLifetime;
        currentLaserTrail.GetComponent<LineRenderer>().SetPosition(0, currentShootPoint.position);
        currentLaserTrail.GetComponent<BulletTrail>().StartTimer();
        currentLaserTrail.SetActive(true);

        if (checkingTime == null)
        {
            checkingTime = StartCoroutine(CheckSpreadTimer());
        }
        else
        {
            StopCoroutine(checkingTime);
            checkingTime = StartCoroutine(CheckSpreadTimer());
        }

        ActivateRecoil();

        Vector3 shootDir = currentShootPoint.forward + new Vector3(Random.Range(-currentSpread.x, currentSpread.x),
                                                        Random.Range(-currentSpread.y, currentSpread.y),
                                                        Random.Range(-currentSpread.z, currentSpread.z));
        shootDir.Normalize();
        if (Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, turretStats.maxRange * 2f))
        {
            currentLaserTrail.GetComponent<LineRenderer>().SetPosition(1, hit.point);

            //hit.collider.GetComponentInParent<IDestroyable>()?.ChangeHealth(-gunDamage);

            //if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //{
            //    hit.collider.GetComponentInParent<CharacterStats>().ChangeHealth(-gunDamage);
            //}

            //if (hit.collider.GetComponentInParent<Rigidbody>() != null)
            //{
            //    Vector3 forceVector = (hit.point - currentShootPoint.position);
            //    if (hit.collider.GetComponentInParent<CharacterStats>() != null)
            //        forceVector.y = 0;
            //    forceVector = forceVector.normalized;
            //    hit.collider.GetComponentInParent<Rigidbody>().AddForce(forceVector * impulsePower, ForceMode.Impulse);
            //}
        }
        else
        {
            currentLaserTrail.GetComponent<LineRenderer>().SetPosition(1, currentShootPoint.position + (shootDir * turretStats.maxRange * 2f));
        }
        if (fireRateType == FireRateType.Burst)
        {
            burstCurrentSize -= 1;
            if (burstCurrentSize > 0)
                StartCoroutine(BurstFireReload(LShoot));
        }
        ChangeShootPoint(); //Change after shoot
    }
    public virtual void ActivateRecoil()
    {
        if (bodyRb == true)
        {
            Vector3 forceVector = (bodyRb.position - currentShootPoint.position);
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
    IEnumerator CheckSpreadTimer()
    {
        currentSpread.x = Mathf.Lerp(currentSpread.x, turretStats.maxSpreadAmount.x, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.y = Mathf.Lerp(currentSpread.y, turretStats.maxSpreadAmount.y, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.z = Mathf.Lerp(currentSpread.z, turretStats.maxSpreadAmount.z, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        yield return new WaitForSeconds(turretStats.spreadWaitTime);
        StopCoroutine(checkingTime);
        checkingTime = null;
        yield break;
    }
    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(turretStats.reloadSpeed);
        reloading = false;
    }
    IEnumerator BurstFireReload(UnityAction shootAction)
    {
        yield return new WaitForSeconds(turretStats.reloadSpeed * 0.2f);
        shootAction.Invoke();
    }
}
