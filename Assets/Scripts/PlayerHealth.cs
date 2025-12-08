using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;

    private void Start()
    {
        stats = PlayerStats.instance;
    }

    public void TakeDamage(int amount)
    {
        stats.currentHealth -= amount;

        if (stats.currentHealth <= 0)
        {
            Debug.Log("Player Dead");
            gameObject.SetActive(false);
        }
    }
}
