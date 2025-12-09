using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stopDistance = 4f;

    Transform player;
    BossAttack bossAttack;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        bossAttack = GetComponent<BossAttack>();
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어 방향 보기
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        // 일정 거리까지만 이동
        float dist = dir.magnitude;
        if (dist > stopDistance)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        // 공격 시도
        bossAttack.TryAttack(dist);
    }

    void LateUpdate()
    {
        StickToGround();
    }

    void StickToGround()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        Vector3 pos = transform.position;
        float terrainY = terrain.SampleHeight(pos) + terrain.GetPosition().y;

        pos.y = terrainY + 0.3f;
        transform.position = pos;
    }

}
