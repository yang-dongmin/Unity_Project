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

    private void OnTriggerEnter(Collider other)
    {
        // 디버그용 로그
        Debug.Log($"Rocket hit: {other.name}");

        // 플레이어 총구나 자기 자신 같은 건 무시하고 싶으면 여기에 조건 추가 가능
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
