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

    private AudioSource audioSource;

    private AudioSource[] gunAudioPool;
    private int audioIndex = 0;


    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void Start()
    {
        ApplyWeaponData(currentWeapon);
        originalRot = weaponHolder.localRotation;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;

        gunAudioPool = new AudioSource[10];
        for (int i = 0; i < gunAudioPool.Length; i++)
        {
            gunAudioPool[i] = gameObject.AddComponent<AudioSource>();
            gunAudioPool[i].spatialBlend = 1f;
            gunAudioPool[i].playOnAwake = false;
        }
    }

    void Update()
    {
        if (isFiring && currentWeapon != null && currentWeapon.isAutoFire)
        {
            TryShoot();
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

    bool isFiring = false;

    void OnAttackStart(InputAction.CallbackContext ctx)
    {
        isFiring = true;

        if (currentWeapon.isAutoFire)
        {
            audioSource.loop = true;
            audioSource.clip = currentWeapon.fireSound;
            audioSource.time = 0f;
            audioSource.Play();
        }
        else
        {
            TryShoot();
        }
    }

    void OnAttackStop(InputAction.CallbackContext ctx)
    {
        isFiring = false;

        if (currentWeapon.isAutoFire)
        {
            audioSource.Stop();
        }
    }


    void TryShoot()
    {
        if (currentWeapon == null) return;

        float baseCooldown = currentWeapon.fireCooldown;

        // 🔥 PlayerStats가 없으면 공격속도 보정 0으로 처리
        float attackSpeedBonus = 0f;
        if (PlayerStats.instance != null)
        {
            attackSpeedBonus = PlayerStats.instance.attackSpeed;   // ex) 4
        }

        // 🔥 공격속도 = 쿨다운 감소 개념으로 사용
        float cooldown = baseCooldown - attackSpeedBonus;
        cooldown = Mathf.Max(0.05f, cooldown);    // 최소 0.05초

        // 🔥 여기서는 반드시 cooldown을 써야 강화가 적용됨
        if (Time.time < lastFireTime + cooldown)
            return;

        Shoot();
        lastFireTime = Time.time;
    }




    void Shoot()
    {
        // ----------------------
        //  🔥 Normal Weapon
        // ----------------------
        if (currentWeapon.weaponType == WeaponType.Normal)
        {
            GameObject proj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            // 🔥 Projectile damage 강화 적용
            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
            {
                p.damage += Mathf.RoundToInt(PlayerStats.instance.attackDamage);
            }

            if (!currentWeapon.isAutoFire)
            {
                PlayFireSound();
            }
        }
        // ----------------------
        //  🔥 Rocket Launcher
        // ----------------------
        else if (currentWeapon.weaponType == WeaponType.RocketLauncher)
        {
            GameObject obj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            RocketProjectile rp = obj.GetComponent<RocketProjectile>();
            if (rp != null)
            {
                // 🔥 로켓 강화 적용 (더 명확하게 통일됨)
                rp.damage = currentWeapon.explosionDamage + Mathf.RoundToInt(PlayerStats.instance.explosionDamage);
                rp.explosionRadius = currentWeapon.explosionRadius + PlayerStats.instance.explosionRadius;
                rp.speed = currentWeapon.rocketSpeed;
                rp.explosionEffect = currentWeapon.explosionEffectPrefab;
            }

            if (!currentWeapon.isAutoFire)
            {
                PlayFireSound();
            }
        }

        if (muzzleFlashInstance != null)
            muzzleFlashInstance.Play();

        StartCoroutine(WeaponTilt());
    }


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


    public void ApplyWeaponData(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        if (muzzleFlashInstance != null)
            Destroy(muzzleFlashInstance.gameObject);

        if (currentWeaponModel != null)
            Destroy(currentWeaponModel);

        if (currentWeapon.weaponModelPrefab != null)
        {
            currentWeaponModel = Instantiate(currentWeapon.weaponModelPrefab, weaponHolder);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
            currentWeaponModel.transform.localRotation =
                Quaternion.Euler(currentWeapon.modelRotationOffset);

            Transform fp = currentWeaponModel.transform.Find("firepoint");
            if (fp != null)
                firePoint = fp;
        }

        if (currentWeapon.muzzleFlashPrefab != null)
        {
            muzzleFlashInstance = Instantiate(currentWeapon.muzzleFlashPrefab, firePoint);
            muzzleFlashInstance.Stop();
        }

        if (muzzleFlashInstance != null)
        {
            muzzleFlashInstance.transform.SetParent(firePoint);
            muzzleFlashInstance.transform.localPosition = Vector3.zero;
            muzzleFlashInstance.transform.localRotation = Quaternion.identity;
        }
        
    }


    void PlayFireSound()
    {
        if (currentWeapon.fireSound == null)
            return;

        AudioSource src = gunAudioPool[audioIndex];
        src.volume = currentWeapon.fireVolume;
        src.PlayOneShot(currentWeapon.fireSound, currentWeapon.fireVolume * 0.6f);

        audioIndex = (audioIndex + 1) % gunAudioPool.Length;
    }
}
