using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionRadius = 3f;
    public int damage = 5;
    public GameObject explosionEffect;

    void Start()
    {
        // 혹시 에디터에서 안 맞게 세팅해놔도 여기서 강제로 맞춰버림
        var col = GetComponent<Collider>();
        col.isTrigger = true;  // ★ 트리거 방식으로 감지

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // 우리가 transform으로 직접 움직일 거라 kinematic
    }

    void Update()
    {
        // 계속 앞으로 이동
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Rocket hit: " + other.name);

        // 일반 적 데미지
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        // 🔥 보스 데미지 추가
        BossHealth boss = other.GetComponent<BossHealth>();
        if (boss != null)
            boss.TakeDamage(damage);

        Explode();
    }


    void Explode()
    {
        Debug.Log("Rocket Explode!");

        // 폭발 이펙트
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // 주변 적에게 데미지
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var hp = hit.GetComponent<EnemyHealth>();
                if (hp != null)
                {
                    hp.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }
}
