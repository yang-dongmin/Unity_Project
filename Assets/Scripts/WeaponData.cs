using UnityEngine;

public enum WeaponType
{
    Normal,
    RocketLauncher
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("프리팹들")]
    public GameObject weaponModelPrefab;   // 손에 들 무기 모델
    public GameObject pickupPrefab;        // 맵에 떨어지는 무기
    public GameObject projectilePrefab;    // 총알 또는 로켓

    [Header("공통 정보")]
    public string weaponName;
    public WeaponType weaponType = WeaponType.Normal;
    public float fireCooldown = 0.2f;
    public bool isAutoFire = false;


    [Header("머즐 플래시")]
    public ParticleSystem muzzleFlashPrefab;

    [Header("로켓런처 옵션")]
    public float rocketSpeed = 10f;
    public float explosionRadius = 3f;
    public int explosionDamage = 5;
    public GameObject explosionEffectPrefab;

    [Header("무기 모델 회전 보정값")]
    public Vector3 modelRotationOffset = Vector3.zero;

    [Header("반동 기울기")]
    public float tiltAmount = 10f;
    public float tiltSpeed = 15f;

    [Header("사운드")]
    public AudioClip fireSound;
    public float fireVolume = 1f;
    public AudioClip autoLoopSound;  // 자동사격용 루프 사운드
    public float loopVolume = 1f;


}
