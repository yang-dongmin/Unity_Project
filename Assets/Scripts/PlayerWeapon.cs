using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData currentWeapon;

    public Transform firePoint;
    public Transform weaponHolder;
    private GameObject currentWeaponModel;

    private float lastFireTime;
    private InputSystem_Actions input;

    private ParticleSystem muzzleFlashInstance;
    private Quaternion originalRot;
    private Quaternion tiltedRot;
    private bool isTilting = false;

    bool isFiring = false;

    // =======================
    // 🔊 오디오
    // =======================
    private AudioSource singleShotSource;
    private AudioSource loopSource;
    private AudioSource[] shotPool;
    private int shotIndex = 0;


    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void Start()
    {
        ApplyWeaponData(currentWeapon);
        originalRot = weaponHolder.localRotation;

        singleShotSource = gameObject.AddComponent<AudioSource>();
        singleShotSource.spatialBlend = 0f;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.spatialBlend = 0f;

        shotPool = new AudioSource[10];
        for (int i = 0; i < 10; i++)
        {
            shotPool[i] = gameObject.AddComponent<AudioSource>();
            shotPool[i].spatialBlend = 0f;
        }
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Attack.started += OnAttackStart;
        input.Player.Attack.canceled += OnAttackStop;
    }

    void OnDisable()
    {
        input.Player.Attack.started -= OnAttackStart;
        input.Player.Attack.canceled -= OnAttackStop;
        input.Disable();
    }


    // ----------------------------------------------------
    // 🔥 핵심: 모든 발사는 Update()에서 쿨다운 검사 후 실행
    // ----------------------------------------------------
    void Update()
    {
        if (!isFiring) return;
        if (currentWeapon == null) return;

        TryShoot();
    }


    // ----------------------------------------------------
    // 🔥 입력 처리
    // ----------------------------------------------------
    void OnAttackStart(InputAction.CallbackContext ctx)
    {

        if (!currentWeapon.isAutoFire)
        {
            // 단발은 누르고 있는 동안 자동반복 안 되게 isFiring = false 유지
            TryShoot();
            return; // ← 중요: 자동 발사 루틴으로 넘어가지 않음
        }
        isFiring = true;

        // 자동 사격 루프 시작
        if (currentWeapon.isAutoFire && currentWeapon.autoLoopSound != null)
        {
            loopSource.clip = currentWeapon.autoLoopSound;
            loopSource.volume = currentWeapon.loopVolume;
            loopSource.Play();
        }
    }

    void OnAttackStop(InputAction.CallbackContext ctx)
    {
        isFiring = false;

        if (currentWeapon.isAutoFire)
        {
            loopSource.Stop();
        }
    }


    // ----------------------------------------------------
    // 🔥 쿨다운 체크 + 발사
    // ----------------------------------------------------
    void TryShoot()
    {
        float baseCooldown = currentWeapon.fireCooldown;
        float bonus = PlayerStats.instance != null ? PlayerStats.instance.attackSpeed : 0f;

        float rpgScale = 0.15f;
        float effectiveBonus = (currentWeapon.weaponType == WeaponType.RocketLauncher)
                               ? bonus * rpgScale
                               : bonus;

        float cooldown = baseCooldown / (1f + effectiveBonus);


        if (Time.time < lastFireTime + cooldown)
            return;

        Shoot();
        lastFireTime = Time.time;
    }


    // ----------------------------------------------------
    // 🔥 총알 생성
    // ----------------------------------------------------
    void Shoot()
    {
        if (firePoint == null)
        {
            Debug.LogError("firePoint 없음!");
            return;
        }

        if (currentWeapon.weaponType == WeaponType.Normal)
        {
            GameObject proj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            // 플레이어와 충돌 무시
            Collider projCol = proj.GetComponent<Collider>();
            Collider playerCol = GetComponentInParent<Collider>();
            if (projCol && playerCol)
                Physics.IgnoreCollision(projCol, playerCol);

            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.damage += Mathf.RoundToInt(PlayerStats.instance.attackDamage);

            if (!currentWeapon.isAutoFire)
                PlaySingleShot();
        }
        else if (currentWeapon.weaponType == WeaponType.RocketLauncher)
        {
            GameObject obj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            Collider projCol = obj.GetComponent<Collider>();
            Collider playerCol = GetComponentInParent<Collider>();
            if (projCol && playerCol)
                Physics.IgnoreCollision(projCol, playerCol);

            RocketProjectile rp = obj.GetComponent<RocketProjectile>();
            if (rp != null)
            {
                rp.damage = currentWeapon.explosionDamage + Mathf.RoundToInt(PlayerStats.instance.explosionDamage);
                rp.explosionRadius = currentWeapon.explosionRadius + PlayerStats.instance.explosionRadius;
                rp.speed = currentWeapon.rocketSpeed;
                rp.explosionEffect = currentWeapon.explosionEffectPrefab;
            }

            if (!currentWeapon.isAutoFire)
                PlaySingleShot();
        }

        if (currentWeapon.isAutoFire)
            PlayAutoShot();

        muzzleFlashInstance?.Play();
        StartCoroutine(WeaponTilt());
    }


    // 🔊 단발 사운드
    void PlaySingleShot()
    {
        if (currentWeapon.fireSound == null) return;
        singleShotSource.volume = currentWeapon.fireVolume;
        singleShotSource.PlayOneShot(currentWeapon.fireSound);
    }

    // 🔊 자동사격 매발 사운드
    void PlayAutoShot()
    {
        if (currentWeapon.autoShotSound == null) return;
        var src = shotPool[shotIndex];
        src.volume = currentWeapon.shotVolume;
        src.PlayOneShot(currentWeapon.autoShotSound);
        shotIndex = (shotIndex + 1) % shotPool.Length;
    }


    // ----------------------------------------------------
    // 🔧 무기 교체
    // ----------------------------------------------------
    public void ApplyWeaponData(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        if (currentWeaponModel != null)
            Destroy(currentWeaponModel);

        if (newWeapon.weaponModelPrefab != null)
        {
            currentWeaponModel = Instantiate(newWeapon.weaponModelPrefab, weaponHolder);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation =
                Quaternion.Euler(newWeapon.modelRotationOffset);

            Transform fp = currentWeaponModel.transform.Find("firepoint");
            if (fp != null)
                firePoint = fp;
        }

        if (muzzleFlashInstance != null)
            Destroy(muzzleFlashInstance.gameObject);

        if (newWeapon.muzzleFlashPrefab != null)
        {
            muzzleFlashInstance = Instantiate(newWeapon.muzzleFlashPrefab, firePoint);
            muzzleFlashInstance.Stop();
        }
    }


    // ----------------------------------------------------
    // 🔥 반동
    // ----------------------------------------------------
    IEnumerator WeaponTilt()
    {
        if (isTilting) yield break;
        isTilting = true;

        tiltedRot = Quaternion.Euler(-currentWeapon.tiltAmount, 0, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * currentWeapon.tiltSpeed;
            weaponHolder.localRotation = Quaternion.Lerp(originalRot, tiltedRot, t);
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * currentWeapon.tiltSpeed;
            weaponHolder.localRotation = Quaternion.Lerp(tiltedRot, originalRot, t);
            yield return null;
        }

        isTilting = false;
    }
}
