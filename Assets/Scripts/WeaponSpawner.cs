using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public static WeaponSpawner instance;
    public WeaponData[] weaponList;

    public float spawnInterval = 5f;
    public Vector3 spawnAreaSize = new Vector3(20, 0, 20);

    [HideInInspector]
    public GameObject currentWeaponPickup;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnWeapon), 2f, spawnInterval);
    }

    void SpawnWeapon()
    {
        if (currentWeaponPickup != null)
            return;

        if (weaponList.Length == 0)
        {
            Debug.LogWarning("WeaponSpawner: weaponList 비어있음!");
            return;
        }

        WeaponData randomWeapon = weaponList[Random.Range(0, weaponList.Length)];

        if (randomWeapon.pickupPrefab == null)
        {
            Debug.LogError("WeaponData에 pickupPrefab이 비어있음 → " + randomWeapon.name);
            return;
        }

        currentWeaponPickup = Instantiate(
            randomWeapon.pickupPrefab,
            GetRandomPosition(),
            Quaternion.identity
        );
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnAreaSize.x, spawnAreaSize.x);
        float z = Random.Range(-spawnAreaSize.z, spawnAreaSize.z);

        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));

        return new Vector3(x, y + 0.5f, z);
    }

}
