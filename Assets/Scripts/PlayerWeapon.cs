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
        gunAudioPool = new AudioSource[10]; // 5개면 충분
        for (int i = 0; i < gunAudioPool.Length; i++)
        {
            gunAudioPool[i] = gameObject.AddComponent<AudioSource>();
            gunAudioPool[i].spatialBlend = 1f;
            gunAudioPool[i].playOnAwake = false;
        }


        ApplyWeaponData(currentWeapon);
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
            TryShoot(); // 단발용
        }
    }

    void OnAttackStop(InputAction.CallbackContext ctx)
    {
        isFiring = false;

        if (currentWeapon.isAutoFire)
        {
            audioSource.Stop(); // ← 떼면 끄기
        }
    }




    void TryShoot()
    {
        if (Time.time < lastFireTime + currentWeapon.fireCooldown)
            return;

        Shoot();
        lastFireTime = Time.time;
    }


    void Shoot()
    {
        if (currentWeapon.weaponType == WeaponType.Normal)
        {
            Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            // 단발 무기만 발사 사운드
            if (!currentWeapon.isAutoFire)
            {
                PlayFireSound();
            }
        }
        else if (currentWeapon.weaponType == WeaponType.RocketLauncher)
        {
            GameObject obj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

            RocketProjectile rp = obj.GetComponent<RocketProjectile>();
            if (rp != null)
            {
                rp.damage = currentWeapon.explosionDamage;
                rp.speed = currentWeapon.rocketSpeed;
                rp.explosionRadius = currentWeapon.explosionRadius;
                rp.explosionEffect = currentWeapon.explosionEffectPrefab;
            }

            // 로켓런처도 단발이므로 PlayOneShot 가능
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

        // 기존 muzzleFlash 삭제
        if (muzzleFlashInstance != null)
            Destroy(muzzleFlashInstance.gameObject);

        // 무기 모델 교체
        if (currentWeaponModel != null)
            Destroy(currentWeaponModel);

        if (currentWeapon.weaponModelPrefab != null)
        {
            currentWeaponModel = Instantiate(currentWeapon.weaponModelPrefab, weaponHolder);

            currentWeaponModel.transform.localPosition = Vector3.zero;

            // ★ 무기 기본 회전 강제 초기화
            currentWeaponModel.transform.localRotation = Quaternion.identity;

            // ★ 무기별 보정 회전 적용
            currentWeaponModel.transform.localRotation = 
                Quaternion.Euler(currentWeapon.modelRotationOffset);

            // firepoint 다시 찾아 재할당
            Transform fp = currentWeaponModel.transform.Find("firepoint");
            if (fp != null)
                firePoint = fp;
        }


        // muzzle flash 재생성
        if (currentWeapon.muzzleFlashPrefab != null)
        {
            muzzleFlashInstance = Instantiate(currentWeapon.muzzleFlashPrefab, firePoint);
            muzzleFlashInstance.Stop();
        }

        // ★ 새 firePoint로 muzzle flash 위치 강제 동기화
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

        src.volume = currentWeapon.fireVolume;  // 🔥 볼륨 복구
        src.PlayOneShot(currentWeapon.fireSound, currentWeapon.fireVolume * 0.6f);

        audioIndex = (audioIndex + 1) % gunAudioPool.Length;
    }




}
