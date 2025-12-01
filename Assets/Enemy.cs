using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    public float speed = 2f;

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

        // 플레이어 방향으로 이동
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
}
