using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance;

    public GameObject levelUpPanel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    
    public bool introPlayed = false;


    public UpgradeData[] allUpgrades;

    // 🔥 추가: 경고 패널
    public GameObject warningPanel;

    private void Awake()
    {
        instance = this;
    }

    public void OpenLevelUpUI()
    {
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowRandomCards();
    }

    void ShowRandomCards()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < 3; i++)
        {
            var upgrade = allUpgrades[Random.Range(0, allUpgrades.Length)];

            var cardObj = Instantiate(cardPrefab, cardContainer);
            var ui = cardObj.GetComponent<UpgradeCardUI>();

            ui.Setup(upgrade, ApplyUpgrade);
        }
    }

    void ApplyUpgrade(UpgradeData upgrade)
    {
        PlayerStats.instance.ApplyUpgrade(upgrade);
        Debug.Log("업그레이드 적용됨 : " + upgrade.upgradeName);

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CheckBossIntro();
    }

    void CheckBossIntro()
    {
        Debug.Log("CheckBossIntro 호출됨. BossSpawner.spawnedBoss = " + BossSpawner.spawnedBoss);

        if (BossSpawner.spawnedBoss == null)
        {
            Debug.Log("CheckBossIntro: 아직 스폰된 보스가 없음.");
            return;
        }
        if (!introPlayed)
        {
        // 🔥 1) 카드 선택이 끝났으니까 WarningPanel 켜기 (반짝반짝 시작)
        if (warningPanel != null){
            introPlayed = true;
            warningPanel.SetActive(true);
}
        // 🔥 2) 인트로는 조금 딜레이 후 실행하는게 자연스러움
        StartCoroutine(StartIntroAfterDelay(BossSpawner.spawnedBoss));
    }

    System.Collections.IEnumerator StartIntroAfterDelay(GameObject boss)
    {
        // 🔥 경고 잠깐 보여줄 시간 (1초)
        yield return new WaitForSecondsRealtime(3f);

        // 🔥 인트로 실제 시작 → BossIntroController 안에서 WarningPanel 자동 OFF 예정
        BossIntroController.instance.PlayIntro(boss);
        }
    }
}
