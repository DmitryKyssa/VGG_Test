using UnityEngine;

public class PlayerHealthController : Singleton<PlayerHealthController>
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int MaxHealth => maxHealth;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
        DontDestroyOnLoad(gameObject);
    }

    public void Reload()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        GameUIController.Instance.UpdateHealth(currentHealth);
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
        Time.timeScale = 0;
        GameUIController.Instance.ShowFinishScreen(GameUIController.LOSE_MESSAGE, isWin: false, LevelLoader.Instance.IsLastLevel);
    }
}