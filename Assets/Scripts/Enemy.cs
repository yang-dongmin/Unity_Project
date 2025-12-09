using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    public float speed = 2f;
    public float heightOffset = 0.5f; // 땅 위로 살짝 띄우기용

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        other.GetComponent<PlayerHealth>()?.TakeDamage(1);
    }
}

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
{
    if (player == null) return;

        StickToGround();
        Vector3 direction = player.position - transform.position;
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z);

        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(flatDir);
            transform.rotation = lookRot;
        }

        transform.position += transform.forward * speed * Time.deltaTime;

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
    void KillPlayer()
    {
        Transform player = GetComponent<Collider>().transform;

        Object.FindFirstObjectByType<GameManager>().GameOver(player);

        Destroy(player.gameObject); // 또는 SetActive(false)
    }

}
