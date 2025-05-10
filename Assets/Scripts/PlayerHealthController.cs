using UnityEngine;

public class PlayerHealthController : Singleton<PlayerHealthController>
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth - 20;
        GameUIController.Instance.UpdateHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameUIController.Instance.UpdateHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        GameUIController.Instance.UpdateHealth(currentHealth);
        Debug.Log($"Player healed by {amount}. Current health: {currentHealth}");
    }

    private void Die()
    {
        // Handle player death (e.g., respawn, game over, etc.)
        Debug.Log("Player has died.");
    }
}