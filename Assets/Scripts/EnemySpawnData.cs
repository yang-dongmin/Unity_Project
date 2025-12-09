using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public int levelRequired;      // 이 레벨 이상일 때부터 등장
    public float spawnWeight = 1f; // 가중치 (확률 비율)
}
