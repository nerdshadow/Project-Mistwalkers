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
public enum TargetDirection
{
    Right = 0,
    Left = 1,
    Front = 2,
    Back = 3
}
public class TurretBehaviour : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField]
    public TurretStats turretStats;
    [SerializeField]
    protected AmmoStats ammoStats;
    [Space(10)]
    [Header("Rotation")]
    public bool canAim = true;
    public float minReqAngle = 5f;
    bool targetInAngle = false;
    [SerializeField]
    protected GameObject ghostHorRotator = null;
    [SerializeField]
    protected GameObject ghostVertRotator = null;
    public bool limitVertRotation = false;
    [SerializeField]
    bool rotateByShort = true;
    [SerializeField]
    protected GameObject HorTurret = null;
    [SerializeField]
    protected GameObject VertTurret = null;
    [SerializeField]
    Collider[] triggerColls;
    [Space(10)]
    [Header("Shooting")]
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
        FindTarget();
        TryRotateTurret();
        TryToFire();
    }
    public void ChangeTargetTo(GameObject _newTarget)
    {
        targetTrans = _newTarget.transform;
        rotateByShort = true;
        toggleViewEye = false;
    }
    public void ChangeTargetTo(Transform _newTargetTrans)
    {
        targetTrans = _newTargetTrans;
        rotateByShort = true;
        toggleViewEye = false;
    }
    public void ClearTarget() //If changing name -> change next func too
    {
        targetTrans = null;
        rotateByShort = true;
        toggleViewEye = false;
    }
    public void ClearTarget(float time)
    {
        Invoke("ClearTarget", time);
        rotateByShort = true;
        toggleViewEye = false;
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
    protected void TryRotateTurret()// go in FixedUpdate
    {
        if (targetTrans == null || canAim == false)
            return;

        //is our eye "active"
        if (toggleViewEye == true)
        {
            //if our eye detected that target reached other side of view
            if (ViewEyeCheckTarget(!wasRotatingRight) == true)
            {
                toggleViewEye = false;
                rotateByShort = true;
            }
        }
        
        if (rotateByShort == true)
            RotateHorShort();
        else
            RotateHorLong();

        RotateVert();
        float a = Vector3.Angle(VertTurret.transform.forward, (targetTrans.position - VertTurret.transform.position));
        if (a <= minReqAngle)
        {
            targetInAngle = true;
        }
        else
            targetInAngle = false;
        //Debug.Log("Is in angle: " + a + " and " + targetInAngle);
    }

    [SerializeField]
    Transform viewEye; //object that will detecting objects position change
    [SerializeField]
    bool toggleViewEye = false;
    [SerializeField]
    bool wasRotatingRight = false; //in what way turret was rotating
    bool isObstacleToLeft()
    {
        // check if no colliders in left | if yes change rotation
        RaycastHit hit;
        foreach (Transform shootPoint in shootPoints)
        {
            float distInZ = shootPoint.localPosition.z;
            float d = distInZ * 0.25f;

            //Transform nextTrans = shootPoint;
            Vector3 nextPos = shootPoint.position;
            for (int i = 0; i < 4; i++)
            {
                float mod = i * d;

                Debug.DrawRay(shootPoint.position + (-shootPoint.forward * mod), -shootPoint.right * 0.1f, Color.red);
                if (Physics.Raycast(shootPoint.position + (-shootPoint.forward * mod), -shootPoint.right, out hit, 0.1f))
                {
                    Debug.Log("Hit in left");
                    return true;
                }
            }
        }

        return false;
    }
    bool isObstacleToRight()
    {
        // check if no colliders in right | if yes change rotation
        RaycastHit hit;
        foreach (Transform shootPoint in shootPoints)
        {
            float distInZ = shootPoint.localPosition.z;
            float d = distInZ * 0.25f;

            //Transform nextTrans = shootPoint;
            Vector3 nextPos = shootPoint.position;
            for (int i = 0; i < 4; i++)
            {
                float mod = i * d;

                Debug.DrawRay(shootPoint.position + (-shootPoint.forward * mod), shootPoint.right * 0.1f, Color.blue);
                if (Physics.Raycast(shootPoint.position + (-shootPoint.forward * mod), shootPoint.right, out hit, 0.1f))
                {
                    Debug.Log("Hit in right");
                    return true;
                }
            }
        }

        return false;
    }
    void ChangeViewEyeTrans()
    {
        //change rotation to where obstacle 
        viewEye.transform.rotation = VertTurret.transform.rotation;
        Debug.Log("Rotated eye to " + viewEye.rotation);
    }
    //check if target moved to the side of eye
    bool ViewEyeCheckTarget(bool changesToRight)
    {
        Vector3 right = viewEye.transform.TransformDirection(transform.right);
        Vector3 toTarget = Vector3.Normalize(targetTrans.position - viewEye.position);
        float dot = Vector3.Dot(right, toTarget);
        //Debug.Log("Dot of turret " + dot);

        if (dot > 0)
        {
            if (changesToRight == true)
            {
                Debug.Log("Target moved RIGHT of eye, changing rotation");
                return true;
            }
            else
                return false;
        }
        else
        {
            if (changesToRight == false)
            {
                Debug.Log("Target moved LEFT of eye, changing rotation");
                return true;
            }
            else
                return false;
        }    

    }
    void ObstacleCheck()
    {
        if (isObstacleToLeft() == true /*&& toggleViewEye == false */)
        {
            //enable view eye
            Debug.Log("Path to target blocked, changing rotation");
            ChangeViewEyeTrans();
            rotateByShort = false;
            toggleViewEye = true;
            wasRotatingRight = false;
        }

        if (isObstacleToRight() == true /*&& toggleViewEye == false*/)
        {
            //enable view eye
            Debug.Log("Path to target blocked, changing rotation");
            ChangeViewEyeTrans();
            rotateByShort = false;
            toggleViewEye = true;
            wasRotatingRight = true;
        }
    }
    // rotate by short way
    TargetDirection targetDirection = TargetDirection.Left;
    TargetDirection lastRotationDirection = TargetDirection.Front;
    void RotateHorShort()
    {
        if (HorTurret == null)
            return;
        targetDirection = GetTargetDirection();
        Debug.Log("Turrets target is " + targetDirection);

        ObstacleCheck();

        switch (targetDirection)
        {
            case TargetDirection.Right:
                HorTurret.transform.localRotation *= Quaternion.AngleAxis(turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                lastRotationDirection = TargetDirection.Right;
                break;
            case TargetDirection.Left:
                HorTurret.transform.localRotation *= Quaternion.AngleAxis(-turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                lastRotationDirection = TargetDirection.Left;
                break;
            case TargetDirection.Front:
                toggleViewEye = false;
                break;
            case TargetDirection.Back:
                if (lastRotationDirection == TargetDirection.Right)
                    HorTurret.transform.localRotation *= Quaternion.AngleAxis(-turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                if (lastRotationDirection == TargetDirection.Left)
                    HorTurret.transform.localRotation *= Quaternion.AngleAxis(turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                break;
            default:
                Debug.LogWarning("Cannot find direction to target!");
                break;
        }
    }
    // rotate by long way
    void RotateHorLong() 
    {
        if (HorTurret == null)
            return;

        TargetDirection buffTargetDir = GetTargetDirection();
        //if we g0 after mid point change long rotation to short
        if (targetDirection != buffTargetDir)
        {
            targetDirection = GetTargetDirection();
            rotateByShort = true;
            return;
        }
        Debug.Log("Turrets target is " + targetDirection);

        ObstacleCheck();
        switch (targetDirection)
        {
            case TargetDirection.Right:
                HorTurret.transform.localRotation *= Quaternion.AngleAxis(-turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                lastRotationDirection = TargetDirection.Right;
                break;
            case TargetDirection.Left:
                HorTurret.transform.localRotation *= Quaternion.AngleAxis(turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                lastRotationDirection = TargetDirection.Left;
                break;
            case TargetDirection.Front:
                toggleViewEye = false;
                break;
            case TargetDirection.Back:
                //helps to unstuck turrent from facing target by back
                if(lastRotationDirection == TargetDirection.Right)
                    HorTurret.transform.localRotation *= Quaternion.AngleAxis(turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                if(lastRotationDirection == TargetDirection.Left)
                    HorTurret.transform.localRotation *= Quaternion.AngleAxis(-turretStats.horizontalSpeed * 40 * Time.deltaTime, HorTurret.transform.up);
                break;
            default:
                Debug.LogWarning("Cannot find direction to target!");
                break;
        }
        
    }
    //Check direction to our target 
    TargetDirection GetTargetDirection()
    {
        Vector3 right = HorTurret.transform.TransformDirection(transform.right);
        Vector3 toTarget = Vector3.Normalize(targetTrans.position - transform.position);
        float dot = Vector3.Dot(right, toTarget);
        //Debug.Log("Dot of turret " + dot);

        if (dot > 0.01)
            return TargetDirection.Right;
        else if(dot < -0.01)
            return TargetDirection.Left;

        Vector3 forward = HorTurret.transform.TransformDirection(transform.forward);
        dot = Vector3.Dot(forward, toTarget);
        if (dot > 0)
            return TargetDirection.Front;
        else
            return TargetDirection.Back;
    }
    void RotateHor()
    {
        if (HorTurret == null)
            return;

        Vector3 _lookDirection = (targetTrans.position - ghostHorRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostHorRotator.transform.rotation = Quaternion.RotateTowards(ghostHorRotator.transform.rotation, _lookRotation, turretStats.horizontalSpeed);
        HorTurret.transform.localEulerAngles = new Vector3(0.0f, ghostHorRotator.transform.localEulerAngles.y, 0.0f);

    }
    void RotateVert()
    {
        if (VertTurret == null)
            return;

        Vector3 _lookDirection = (targetTrans.position - ghostVertRotator.transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_lookDirection);

        ghostVertRotator.transform.rotation = Quaternion.RotateTowards(ghostVertRotator.transform.rotation, _lookRotation, turretStats.verticalSpeed);
        float targetAngle = ghostVertRotator.transform.localEulerAngles.x;
        //if (limitVertRotation == true)
        //{
        //Debug.Log("Start targetAngel = " + targetAngle);

        float diff = 180 - targetAngle;
        if (diff < 0)
        {
            if (targetAngle < 360 - turretStats.verticalMaxAngle_pos)
            {
                //Debug.Log("Smaller");
                targetAngle = 360 - turretStats.verticalMaxAngle_pos;
            }
        }
        else
        {
            if (targetAngle > -turretStats.verticalMaxAngle_neg)
            {
                //Debug.Log("Smaller");
                targetAngle = -turretStats.verticalMaxAngle_neg;
            }
        }
        //Debug.Log("After targetAngel = " + targetAngle);
        //}
        VertTurret.transform.localEulerAngles = new Vector3(targetAngle, 0.0f, 0.0f);
    }
    bool SomethingInTheWay()
    {
        //Collider[] colls = Physics.OverlapCapsule(VertTurret.transform.position, shootPoints[0].transform.position, 0.046f);
        //foreach (Collider coll in colls)
        //{
        //    Debug.Log("Colliding with " + coll.gameObject.name);
        //}
        //if(colls.Length > 0)
        //    return true;
        //else return false;
        return Physics.CheckCapsule(HorTurret.transform.position, shootPoints[0].transform.position, 0.046f);
    }
    void TryToFire()
    {
        if (canFire == false 
            || targetInAngle == false 
            || reloading == true 
            || targetTrans == null
            /*|| SomethingInTheWay() == true*/)
            return;
        Shoot();
        StartCoroutine(Reload());
    }
    [ContextMenu("Shoot")]
    public virtual void Shoot()
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
    int loopCount = 0;
    void ChangeShootPoint()
    {
        if (shootPoints.Count <= 1)
            return;
        lastShootPoint = currentShootPoint;
        currentShootPoint = shootPoints[Random.Range(0, shootPoints.Count)];
        if (currentShootPoint == lastShootPoint)
        {
            if (loopCount >= 64)
                return;

            loopCount++;
            ChangeShootPoint();
        }
        else
        {
            loopCount = 0;
        }
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

            IDamageable potTarget = hit.collider.GetComponent<IDamageable>();
            if (potTarget != null)
            {
                potTarget.DoDamage(ammoStats.ammoDamage);
            }
            else
            {
                potTarget = hit.collider.GetComponentInParent<IDamageable>();
                if (potTarget != null)
                {
                    potTarget.DoDamage(ammoStats.ammoDamage);
                }
            }

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
        currentShell.GetComponent<ProjectileBehaviour>().projectileDamage = ammoStats.ammoDamage;
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

            IDamageable potTarget = hit.collider.GetComponent<IDamageable>();
            if (potTarget != null)
            {
                potTarget.DoDamage(ammoStats.ammoDamage);
            }
            else
            {
                potTarget = hit.collider.GetComponentInParent<IDamageable>();
                if (potTarget != null)
                {
                    potTarget.DoDamage(ammoStats.ammoDamage);
                }
            }

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
        if (currentSpread.x == turretStats.minSpreadAmount.x
            && currentSpread.y == turretStats.minSpreadAmount.y
            && currentSpread.z == turretStats.minSpreadAmount.z)
            return;
        if (checkingTime == null)
        {
            if (currentSpread.x != turretStats.minSpreadAmount.x)
            {
                currentSpread.x = Mathf.Lerp(currentSpread.x, turretStats.minSpreadAmount.x, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
                if (Mathf.Abs(currentSpread.x - turretStats.minSpreadAmount.x) <= 0.001)
                    currentSpread.x = turretStats.minSpreadAmount.x;
            }
            if (currentSpread.y != turretStats.minSpreadAmount.y)
            {
                currentSpread.y = Mathf.Lerp(currentSpread.y, turretStats.minSpreadAmount.y, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
                if (Mathf.Abs(currentSpread.y - turretStats.minSpreadAmount.y) <= 0.001)
                    currentSpread.y = turretStats.minSpreadAmount.y;
            }
            if (currentSpread.z != turretStats.minSpreadAmount.z)
            {
                currentSpread.z = Mathf.Lerp(currentSpread.z, turretStats.minSpreadAmount.z, Time.deltaTime * turretStats.spreadEnlargingSpeed * 1.2f);
                if (Mathf.Abs(currentSpread.z - turretStats.minSpreadAmount.z) <= 0.001)
                    currentSpread.z = turretStats.minSpreadAmount.z;
            }
        }
    }
    IEnumerator CheckSpreadTimer()
    {
        currentSpread.x = Mathf.Lerp(currentSpread.x, turretStats.maxSpreadAmount.x, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.y = Mathf.Lerp(currentSpread.y, turretStats.maxSpreadAmount.y, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        currentSpread.z = Mathf.Lerp(currentSpread.z, turretStats.maxSpreadAmount.z, Time.deltaTime * turretStats.spreadEnlargingSpeed);
        yield return new WaitForSeconds(turretStats.spreadWaitTime);
        DisableCheckingTime();
        yield break;
    }
    void DisableCheckingTime()
    {
        StopCoroutine(checkingTime);
        checkingTime = null;
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
