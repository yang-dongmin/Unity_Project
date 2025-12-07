using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 50;

    public Slider xpSlider;   // ⭐ 슬라이더 참조
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    void Start()
    {
        if (xpSlider != null)
        {
            xpSlider.minValue = 0;
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }

        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        level++;
        xpToNextLevel += 20;

        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
        }

        Debug.Log("레벨업! 현재 레벨: " + level);
    }

    void UpdateUI()
    {
        if (xpSlider != null)
        {
            xpSlider.value = currentXP;   // ⭐ 현재 경험치 반영
        }

        if (levelText != null)
        {
            levelText.text = "Lv " + level;
        }

        if (xpText != null)
        {
            xpText.text = currentXP + " / " + xpToNextLevel;
        }
    }
}
