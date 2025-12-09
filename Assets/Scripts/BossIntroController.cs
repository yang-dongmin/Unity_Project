using UnityEngine;
using Cinemachine;
using System.Collections;

public class BossIntroController : MonoBehaviour
{
    public static BossIntroController instance;

    public CinemachineVirtualCamera introCam;

    public Transform bossSpawnPoint;

    void Awake()
    {
        instance = this;
    }

    public void PlayIntro(GameObject boss)
    {
        StartCoroutine(IntroRoutine(boss));
    }

    IEnumerator IntroRoutine(GameObject boss)
    {
        Time.timeScale = 0f;

        // 인트로 카메라 우선순위 높임
        introCam.Priority = 100;

        Vector3 startPos = bossSpawnPoint.position + Vector3.down * 3f;
        Vector3 endPos = bossSpawnPoint.position;

        boss.transform.position = startPos;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 0.5f;
            boss.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        introCam.Priority = 0;

        Time.timeScale = 1f;
    }
}
