using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public static BossSpawner instance;

    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    // 🔥 카드 선택이 끝난 뒤 보스 인트로를 실행시키기 위한 플래그
    public bool bossSpawned = false;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnBoss()
    {
        // 이미 보스가 존재하면 스폰 X
        if (GameObject.FindGameObjectWithTag("Boss") != null)
            return;

        // 🔥 적 제거
        KillAllEnemies();

        // 🔥 EnemySpawner 멈추기
        EnemySpawner.instance?.StopSpawning();

        // ❗ 인트로는 여기서 실행하지 않는다!
        // BossIntroController.instance?.PlayBossIntro();  ← 삭제됨

        // 🔥 스폰 준비 체크
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("❌ BossSpawner: bossPrefab 또는 bossSpawnPoint가 null입니다.");
            return;
        }

        // 🔥 보스 생성
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        boss.tag = "Boss";

        // 🔥 "인트로 예약" 플래그 켜기
        bossSpawned = true;

        Debug.Log("🔥 보스 등장! (카드 선택 후 인트로 재생 예정)");
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
