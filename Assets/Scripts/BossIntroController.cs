using UnityEngine;
using TMPro;
using System.Collections;

public class BossIntroController : MonoBehaviour
{
    public static BossIntroController instance;

    public GameObject introPanel;
    public TextMeshProUGUI bossNameText;

    void Awake()
    {
        instance = this;
    }

    public void PlayBossIntro()
    {
        Debug.Log("Boss Intro 실행됨!");

        introPanel.SetActive(true);
        bossNameText.text = "WARNING";

        // 게임 전체 멈춤
        Time.timeScale = 0f;

        // TimeScale 영향 안받는 타이머
        StartCoroutine(CloseIntroRoutine());
    }

    IEnumerator CloseIntroRoutine()
    {
        // Realtime → TimeScale 영향 없음
        yield return new WaitForSecondsRealtime(1.5f);

        introPanel.SetActive(false);

        // 다시 게임 재개
        Time.timeScale = 1f;
    }
}
