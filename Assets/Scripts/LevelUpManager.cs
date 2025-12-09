using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance;

    public GameObject levelUpPanel;
    public Transform cardContainer;
    public GameObject cardPrefab;

    public UpgradeData[] allUpgrades;  // SO로 만든 업그레이드들

    private void Awake()
    {
        instance = this;
    }

    public void OpenLevelUpUI()
    {
        Time.timeScale = 0f; // 게임 일시정지
        levelUpPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowRandomCards();
    }

    void ShowRandomCards()
    {
        // 기존 카드 제거
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        // 랜덤 3개 선택
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

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CheckBossIntro();
    }
    void CheckBossIntro()
    {
        if (BossSpawner.instance.bossSpawned)
        {
            BossIntroController.instance.PlayBossIntro();
            BossSpawner.instance.bossSpawned = false; // 중복 실행 방지
        }
    }

}
