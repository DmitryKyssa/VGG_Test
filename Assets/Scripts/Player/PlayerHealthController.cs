using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthController : Singleton<PlayerHealthController>
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private Volume volume;

    public int MaxHealth => maxHealth;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
        DontDestroyOnLoad(gameObject);
        volume = FindFirstObjectByType<Volume>();
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashHitEffect());
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        GameUIController.Instance.UpdateHealth(currentHealth);
    }

    private IEnumerator FlashHitEffect()
    {
        if (volume == null)
        {
            volume = FindFirstObjectByType<Volume>();
        }
        volume.profile.TryGet(out Vignette vignette);
        vignette.intensity.Override(0.5f);
        vignette.active = true;
        yield return new WaitForSeconds(0.5f);
        vignette.intensity.Override(0f);
        vignette.active = false;
    }

    public void Heal(int amount)
    {
        if (currentHealth == maxHealth)
        {
            InventorySystem.Instance.AddHealer(amount);
            InventoryUIController.Instance.SetHealersCount(amount, 1);
            return;
        }

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        GameUIController.Instance.UpdateHealth(currentHealth);
    }

    private void Die()
    {
        Time.timeScale = 0;
        GameUIController.Instance.ShowFinishScreen(GameUIController.LOSE_MESSAGE, isWin: false, LevelLoader.Instance.IsLastLevel);
    }
}