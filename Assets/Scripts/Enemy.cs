using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    public float speed = 2f;
    public float heightOffset = 0.5f; // 땅 위로 살짝 띄우기용

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found! Tag the player as 'Player'");
    }

    void Update()
    {
        if (player == null) return;

        // 1) 플레이어 쪽 방향 (XZ 평면에서만)
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dir = toPlayer.normalized;

        // 2) XZ로만 따라가기
        transform.position += dir * speed * Time.deltaTime;

        // 3) 지형에 붙이기 (Terrain 기준)
        StickToGround();
    }

    void StickToGround()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        Vector3 pos = transform.position;

        // Terrain의 실제 월드 기준 높이
        float terrainY = terrain.SampleHeight(pos) + terrain.GetPosition().y;

        pos.y = terrainY + heightOffset;
        transform.position = pos;
    }
}
