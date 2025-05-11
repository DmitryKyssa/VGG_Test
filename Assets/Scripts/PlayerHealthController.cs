using UnityEngine;

public class PlayerHealthController : Singleton<PlayerHealthController>
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        GameUIController.Instance.UpdateHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
        GameUIController.Instance.ShowFinishScreen(GameUIController.LOSE_MESSAGE, isWin: false, isLastLevel: false);
        Time.timeScale = 0;
        PlayerMovementController.Instance.enabled = false;
    }
}