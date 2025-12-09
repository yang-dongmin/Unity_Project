using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public static BossSpawner instance;

    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    // ✅ 실제로 스폰된 보스를 저장하는 static 변수
    public static GameObject spawnedBoss = null;

    private void Awake()
    {
        instance = this;
        Debug.Log("BossSpawner Awake, spawnedBoss = " + spawnedBoss);
    }

    public void SpawnBoss()
    {
        // 이미 스폰된 보스가 있으면 그냥 그걸 사용
        if (spawnedBoss != null)
        {
            Debug.Log("BossSpawner: 이미 보스가 존재함: " + spawnedBoss.name);
            return;
        }

        // 🔥 적 제거
        KillAllEnemies();

        // 🔥 EnemySpawner 멈추기
        EnemySpawner.instance?.StopSpawning();

        // 🔥 스폰 준비 체크
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("❌ BossSpawner: bossPrefab 또는 bossSpawnPoint가 null입니다.");
            return;
        }

        // 🔥 보스 생성 + static 변수에 저장
        spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        spawnedBoss.tag = "Boss";

        Debug.Log("🔥 보스 스폰 완료! spawnedBoss = " + spawnedBoss.name);
    }

    void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject e in enemies)
        {
            Destroy(e);
        }

        Debug.Log($"🔥 {enemies.Length} 마리의 적 제거됨");
    }
}
