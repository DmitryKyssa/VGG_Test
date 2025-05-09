using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI magazinesText;
    [SerializeField] private TextMeshProUGUI patronesText;
    [SerializeField] private TextMeshProUGUI enemiesCountText;
    [SerializeField] private TextMeshProUGUI messageText;

    private const string HealthTextFormat = "Health: {0}";
    private const string MagazinesTextFormat = "Magazines: {0}";
    private const string PatronesTextFormat = "Patrones: {0}";
    private const string EnemiesCountTextFormat = "Enemies: {0}";

    public const string GameStartMessage = "Game Start!";
    public const string GameOverMessage = "Game Over!";
    public const string WinMessage = "You Win!";
    public const string LoseMessage = "You Lose!";
    public const string ReloadMessage = "Reload!";
    public const string EnemyKilledMessage = "Enemy Killed!";

    public void UpdateHealth(int health)
    {
        healthText.text = string.Format(HealthTextFormat, health);
    }

    public void UpdateMagazines(int magazines)
    {
        magazinesText.text = string.Format(MagazinesTextFormat, magazines);
    }

    public void UpdatePatrones(int patrones)
    {
        patronesText.text = string.Format(PatronesTextFormat, patrones);
    }

    public void UpdateEnemiesCount(int count)
    {
        enemiesCountText.text = string.Format(EnemiesCountTextFormat, count);
    }

    public void ShowMessage(string message, float delay)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(delay));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }
}