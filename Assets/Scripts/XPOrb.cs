using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpAmount = 10;

    Transform player;
    public float pickupDistance = 1.2f;   // 먹히는 거리
    public float attractDistance = 4f;    // 빨려오는 거리
    public float attractSpeed = 6f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // 플레이어 가까이 오면 빨려오기
        if (dist < attractDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                attractSpeed * Time.deltaTime
            );
        }

        // 완전 가까우면 먹기
        if (dist < pickupDistance)
        {
            PlayerXP xp = player.GetComponent<PlayerXP>();
            if (xp != null)
                xp.AddXP(xpAmount);

            Destroy(gameObject);
        }
    }
}
