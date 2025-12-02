using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 1;

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Debug.Log("Player Dead");
            gameObject.SetActive(false);
        }
    }
}
