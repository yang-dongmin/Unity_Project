using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public float attackCooldown = 3f;
    float lastAttackTime;

    public GameObject laserPrefab;   // 레이저 이펙트
    public Transform firePoint;      // 발사 위치

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
        if (firePoint == null)
        {
            Debug.LogError("🔥 Boss firePoint가 없습니다!");
            return;
        }

        if (laserPrefab == null)
        {
            Debug.LogError("🔥 Boss laserPrefab이 없습니다!");
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        if (dist < 15f)
        {
            StartCoroutine(LaserAttack());
            lastAttackTime = Time.time;
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

}
