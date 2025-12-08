using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;

    public enum UpgradeType { MoveSpeed, AttackDamage, AttackSpeed, ExplosionRadius, ExplosionDamage, MaxHealthIncrease, Heal }
    public UpgradeType type;

    public float value; // 증가량
}
