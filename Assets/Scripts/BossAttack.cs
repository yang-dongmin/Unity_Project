using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public float attackCooldown = 3f;
    float lastAttackTime;

    public GameObject laserPrefab;   // 레이저 이펙트
    public Transform firePoint;      // 발사 위치

    public GameObject aoePrefab;   // 장판 프리팹 (Particle 포함)
    public float aoeRadius = 3f;
    public int aoeDamage = 30;


    public int damage = 20;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (laserPrefab == null)
        {
            Debug.LogError("🔥 laserPrefab이 Inspector에 연결되지 않았습니다!");
        }

    }


    
    public void TryAttack(float dist)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        // 패턴 선택 (레이저, 돌진, 탄막, 장판)
        int pattern = Random.Range(0, 3);

        switch (pattern)
        {
            case 0:
                StartCoroutine(LaserAttack());
                break;
            case 1:
                StartCoroutine(AoeAttack());
                break;
            case 2:
                StartCoroutine(DashAttack());
                break;
        }
    }

    IEnumerator LaserAttack()
    {
        Debug.Log("Boss → Laser Charge!");

        Vector3 start = firePoint.position;
        Vector3 dir = (player.position - firePoint.position).normalized;

        // ① 경고 레이저 표시 (얇은 빨간선)
        LineRenderer warn = Instantiate(laserPrefab).GetComponent<LineRenderer>();
        warn.positionCount = 2;
        warn.SetPosition(0, start);
        warn.SetPosition(1, start + dir * 30f);
        warn.startWidth = 0.05f;
        warn.endWidth = 0.05f;
        warn.material.color = Color.red * 0.5f;

        // 1초 동안 조준선 유지
        yield return new WaitForSeconds(1f);
        Destroy(warn.gameObject);

        // ② 발사 (굵은 강한 레이저)
        GameObject laser = Instantiate(laserPrefab);
        LineRenderer lr = laser.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, start + dir * 30f);
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;

        // 데미지 판정
        if (Physics.Raycast(start, dir, out RaycastHit hit, 30f))
        {
            if (hit.collider.CompareTag("Player"))
                hit.collider.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(laser);
    }

    IEnumerator AoeAttack()
    {
        Debug.Log("Boss → AOE Attack!");

        // 1) 플레이어 현재 위치 저장
        Vector3 targetPos = player.position;
        targetPos.y = 0; // 바닥에 붙이기

        // 2) AOE 장판 생성
        GameObject aoe = Instantiate(aoePrefab, targetPos, Quaternion.identity);

        // 3) 1초간 대기 (경고)
        float delay = 1f;
        yield return new WaitForSeconds(delay);

        // 4) 폭발 데미지 판정
        float dist = Vector3.Distance(player.position, targetPos);

        if (dist < aoeRadius)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(aoeDamage);
        }

        // 5) 폭발 파티클 재생 (있다면)
        ParticleSystem ps = aoe.GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Play();

        // 6) 1초 뒤 장판 삭제
        Destroy(aoe, 1.5f);
    }

    IEnumerator DashAttack()
    {
        Debug.Log("Boss → Dash Charge...");

        // -------------------------------
        // 1) 플레이어 위치 기반으로 방향만 저장
        // -------------------------------
        Vector3 startPos = transform.position;
        Vector3 playerPos = player.position;

        playerPos.y = startPos.y; // 높이 고정
        Vector3 dir = (playerPos - startPos).normalized;

        // -------------------------------
        // 2) 보스 바라보게 하고 멈춤(차지)
        // -------------------------------
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        float chargeTime = 1.5f;
        yield return new WaitForSeconds(chargeTime);

        Debug.Log("Boss → DASH!!!");

        // -------------------------------
        // 3) 돌진 (시간 기반 이동)
        // -------------------------------
        float dashSpeed = 60f;
        float dashDuration = 0.6f;  // 플레이어 위치를 충분히 넘어가기 위한 시간

        float t = 0f;

        while (t < dashDuration)
        {
            t += Time.deltaTime;

            // 직선 방향으로 밀어붙이기
            transform.position += dir * dashSpeed * Time.deltaTime;

            // 지나가면서 데미지 판정
            if (Vector3.Distance(transform.position, player.position) < 2f)
            {
                player.GetComponent<PlayerHealth>()?.TakeDamage(20);
            }

            yield return null;
        }

        Debug.Log("Boss Dash Finished!");
    }





}
