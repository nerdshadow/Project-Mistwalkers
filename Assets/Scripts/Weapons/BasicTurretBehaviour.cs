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
    public bool canFire = false;
    bool reloading = false;
    [SerializeField]
    public enum FireRateType
    {
        Automatic = 0,
        Burst = 1
    }
    [SerializeField]
    FireRateType fireRateType = FireRateType.Automatic;
    [SerializeField]
    Vector3 currentSpread = Vector3.zero;
    Coroutine checkingTime = null;
    public Transform targetTrans = null;
    public Rigidbody bodyRb;
    public Transform shootPoint;
    private void OnValidate()
    {
        //Update things

    }
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
        //Old
        //Debug.Log("Hor Rotating");
        //ghostHorRotator.transform.LookAt(targetTrans, transform.up);

        //HorTurret.transform.rotation = Quaternion.RotateTowards(HorTurret.transform.rotation, ghostHorRotator.transform.rotation, turretStats.horizontalSpeed);
        //HorTurret.transform.localEulerAngles = new Vector3(0.0f, HorTurret.transform.eulerAngles.y, 0.0f);

        //New
        Vector3 _lookDirection = (targetTrans.position - ghostHorRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostHorRotator.transform.rotation = Quaternion.RotateTowards(ghostHorRotator.transform.rotation, _lookRotation, turretStats.horizontalSpeed);
        HorTurret.transform.localEulerAngles = new Vector3(0.0f, ghostHorRotator.transform.localEulerAngles.y, 0.0f);

    }
    protected void RotateVert()
    {
        if (VertTurret == null)
            return;
        //Debug.Log("Vert Rotating");
        //OLD
        //ghostVertRotator.transform.LookAt(targetTrans, transform.up);

        //VertTurret.transform.rotation = Quaternion.RotateTowards(VertTurret.transform.rotation, ghostVertRotator.transform.rotation, turretStats.verticalSpeed);
        //VertTurret.transform.localEulerAngles = new Vector3(VertTurret.transform.eulerAngles.x, 0.0f, 0.0f);

        Vector3 _lookDirection = (targetTrans.position - ghostVertRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostVertRotator.transform.rotation = Quaternion.RotateTowards(ghostVertRotator.transform.rotation, _lookRotation, turretStats.verticalSpeed);
        VertTurret.transform.localEulerAngles = new Vector3(ghostVertRotator.transform.localEulerAngles.x, 0.0f, 0.0f);
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
        GameObject currentBulletTrail;
        currentBulletTrail = Instantiate(ammoStats.bulletTrailVfx_Prefab, shootPoint.position, shootPoint.rotation);
        currentBulletTrail.GetComponent<BulletTrail>().lineLifetime = ammoStats.trailLifetime;
        currentBulletTrail.GetComponent<LineRenderer>().SetPosition(0, shootPoint.position);
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

        Vector3 shootDir = shootPoint.forward + new Vector3(Random.Range(-currentSpread.x, currentSpread.x),
                                                        Random.Range(-currentSpread.y, currentSpread.y),
                                                        Random.Range(-currentSpread.z, currentSpread.z));
        shootDir.Normalize();
        if (Physics.Raycast(shootPoint.position, shootDir, out RaycastHit hit, turretStats.maxRange * 2f))
        {
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, hit.point);

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
            currentBulletTrail.GetComponent<LineRenderer>().SetPosition(1, shootPoint.position + (shootDir * turretStats.maxRange));
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
}
