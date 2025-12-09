using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemySpawnData> enemyTypes = new List<EnemySpawnData>();

    public static EnemySpawner instance;

    public float spawnInterval = 1f;
    public float spawnDistance = 10f;

    private PlayerXP playerXP;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // 🔥 PlayerXP 직접 찾아오기
        playerXP = FindAnyObjectByType<PlayerXP>();

        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (playerXP == null)
        {
            Debug.LogWarning("PlayerXP를 찾을 수 없음!");
            return;
        }

        int level = playerXP.level;  // 🔥 PlayerXP에서 레벨 직접 가져오기

        GameObject enemyToSpawn = GetEnemyForLevel(level);

        if (enemyToSpawn == null)
        {
            Debug.LogWarning("스폰 가능한 몬스터가 없음!");
            return;
        }

        Vector3 pos = Random.onUnitSphere;
        pos.y = 0;
        pos = pos.normalized * spawnDistance;

        Instantiate(enemyToSpawn, pos, Quaternion.identity);
    }

    GameObject GetEnemyForLevel(int level)
    {
        List<EnemySpawnData> candidates = new List<EnemySpawnData>();

        foreach (var data in enemyTypes)
        {
            if (level >= data.levelRequired)
                candidates.Add(data);
        }

        if (candidates.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var c in candidates)
            totalWeight += c.spawnWeight;

        float randomValue = Random.value * totalWeight;

        foreach (var c in candidates)
        {
            if (randomValue <= c.spawnWeight)
                return c.enemyPrefab;

            randomValue -= c.spawnWeight;
        }

        return candidates[0].enemyPrefab;
    }
    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnEnemy));
    }

}
