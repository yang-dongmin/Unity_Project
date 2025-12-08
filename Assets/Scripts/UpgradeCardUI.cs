using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;

    private UpgradeData upgrade;
    private System.Action<UpgradeData> onSelect;

    public void Setup(UpgradeData data, System.Action<UpgradeData> callback)
    {
        upgrade = data;
        onSelect = callback;

        titleText.text = data.upgradeName;
        descriptionText.text = data.description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect(data));
    }
}
