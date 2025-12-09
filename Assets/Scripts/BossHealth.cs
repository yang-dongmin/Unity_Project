using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHp = 500;
    int currentHp;


    public System.Action<int, int> OnHealthChanged;   // HP UI 갱신용

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int dmg)
    {
        currentHp -= dmg;
        Debug.Log("Boss HP: " + currentHp + "/" + maxHp);
        OnHealthChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated !");

        FindFirstObjectByType<GameManager>().GameClear();

        Destroy(gameObject);
    }
}
