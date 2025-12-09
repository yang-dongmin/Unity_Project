using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("기본 스탯")]
    public float moveSpeed = 5f;

    [Header("공격 스탯")]
    public float attackDamage = 10f;
    public float attackSpeed = 1f; // 발사 간격(작을수록 빠름)

    [Header("로켓런처 전용")]
    public float explosionRadius = 3f;
    public float explosionDamage = 5f;

    [Header("체력")]
    public int maxHealth = 5;
    public int currentHealth = 5;

    private void Awake()
    {
        instance = this;
    }

    public void ApplyUpgrade(UpgradeData up)
    {
        switch (up.type)
        {
            case UpgradeData.UpgradeType.MoveSpeed:
                moveSpeed += up.value;
                break;

            case UpgradeData.UpgradeType.AttackDamage:
                attackDamage += up.value;
                break;

            case UpgradeData.UpgradeType.AttackSpeed:
                attackSpeed += up.value;
                break;

            case UpgradeData.UpgradeType.ExplosionRadius:
                explosionRadius += up.value;
                break;

            case UpgradeData.UpgradeType.ExplosionDamage:
                explosionDamage += up.value;
                break;

            case UpgradeData.UpgradeType.MaxHealthIncrease:
                {
                    int increase = Mathf.RoundToInt(up.value);

                    // 🔥 최대 체력 증가
                    maxHealth += increase;

                    // 🔥 현재 체력도 증가량만큼 회복
                    currentHealth += increase;

                    // 🔥 현재 체력이 최대를 넘지 않도록 제한
                    currentHealth = Mathf.Min(currentHealth, maxHealth);
                }
                break;


            case UpgradeData.UpgradeType.Heal:
                currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.RoundToInt(up.value));
                break;
        }

    
        Debug.Log("업그레이드 적용됨 : " + up.upgradeName);
    }
}
