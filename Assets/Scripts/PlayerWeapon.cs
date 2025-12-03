using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;   // 총알 프리팹
    public Transform firePoint;           // 총알이 나가는 위치
    public float fireCooldown = 0.2f;     // 연사 속도 제한

    private float lastFireTime;
    private InputSystem_Actions input;

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
        // 쿨타임 체크
        if (Time.time < lastFireTime + fireCooldown) return;

        Shoot();
        lastFireTime = Time.time;
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

    }
}
