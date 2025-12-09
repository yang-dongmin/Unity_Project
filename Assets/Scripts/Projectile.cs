using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 1.5f;

    public GameObject hitFlash;

    private LineRenderer lr;
    private Vector3 lastPos;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        lastPos = transform.position;

        // 일정 시간 지나면 삭제
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void Update()
    {
        // 이동 계산
        Vector3 newPos = transform.position + transform.forward * speed * Time.deltaTime;

        // 레이저 궤적 갱신
        if (lr != null)
        {
            lr.SetPosition(0, lastPos);
            lr.SetPosition(1, newPos);
        }

        // 총알 이동
        transform.position = newPos;

        // 다음 프레임을 위한 기록
        lastPos = newPos;
    }

    void OnTriggerEnter(Collider other)
    {
        // 일반 적 처리
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            HitEffect();
            return;
        }

        // 🔥 보스 처리
        BossHealth boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            HitEffect();
            return;
        }
    }

    void HitEffect()
    {
        if (hitFlash != null)
            Instantiate(hitFlash, transform.position, Quaternion.identity);

        DestroySelf();
    }


    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
