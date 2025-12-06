using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class PlayerWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;   // 총알 프리팹
    public Transform firePoint;           // 총알이 나가는 위치
    public float fireCooldown = 0.2f;     // 연사 속도 제한

    private float lastFireTime;
    private InputSystem_Actions input;

    public ParticleSystem muzzleFlash;

    public Transform weaponHolder;
    public float tiltAmount = 10f;      
    public float tiltSpeed = 15f;       

    private Quaternion originalRot;
    private Quaternion tiltedRot;
    private bool isTilting = false;


    void Start()
    {
        originalRot = weaponHolder.localRotation;
    }


    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        input.Player.Attack.performed -= OnAttack;
        input.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (GameManager.instance != null && GameManager.instance.isOptionOpen)
            return;
            
        if (Time.time < lastFireTime + fireCooldown) return;

        Shoot();
        lastFireTime = Time.time;
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (muzzleFlash != null)
            muzzleFlash.Play();
        StartCoroutine(WeaponTilt());
    }

    IEnumerator WeaponTilt()
    {
        if (isTilting) yield break; 
        isTilting = true;

        // 살짝 아래로 혹은 뒤로 기울이기 (Z축 또는 X축)
        tiltedRot = Quaternion.Euler(-tiltAmount, 0, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * tiltSpeed;
            weaponHolder.localRotation = Quaternion.Lerp(originalRot, tiltedRot, t);
            yield return null;
        }

        // 다시 원래 위치로 복귀
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * tiltSpeed;
            weaponHolder.localRotation = Quaternion.Lerp(tiltedRot, originalRot, t);
            yield return null;
        }

        isTilting = false;
    }

}
