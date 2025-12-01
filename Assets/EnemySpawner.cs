using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    public float spawnDistance = 10f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 pos = Random.onUnitSphere;   // 랜덤한 방향
        pos.y = 0;
        pos = pos.normalized * spawnDistance;

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
