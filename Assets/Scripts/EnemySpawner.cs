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
            return;

        int level = playerXP.level;
        GameObject enemyToSpawn = GetEnemyForLevel(level);

        if (enemyToSpawn == null)
            return;

        // 🔥 플레이어 주변 spawnDistance 반경에서 스폰
        Vector3 dir = Random.onUnitSphere;
        dir.y = 0;
        dir.Normalize();

        Vector3 pos = playerXP.transform.position + dir * spawnDistance;

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
