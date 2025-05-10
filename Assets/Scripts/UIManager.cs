using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI magazinesText;
    [SerializeField] private TextMeshProUGUI patronesText;
    [SerializeField] private TextMeshProUGUI enemiesCountText;
    [SerializeField] private TextMeshProUGUI messageText;

    private const string HEALTH_TEXT_FORMAT = "Health: {0}";
    private const string MAGAZINES_TEXT_FORMAT = "Magazines: {0}";
    private const string PATRONES_TEXT_FORMAT = "Patrones: {0}";
    private const string ENEMIES_COUNT_TEXT_FORMAT = "Enemies: {0}";

    public const string GAME_START_MESSAGE = "Game Start!";
    public const string RELOAD_MESSAGE = "Reload!";
    public const string ENEMY_KILLED_MESSAGE = "Enemy Killed!";
    public const string ENEMY_SPAWNED_MESSAGE = "Enemy Spawned!";
    public const string PATRONS_CANCELED_MESSAGE = "Patrons Cancelled!";

    private readonly Queue<string> messageQueue = new Queue<string>();
    private readonly float delay = 1f;

    private void Awake()
    {
        messageText.gameObject.SetActive(false);
        messageText.text = string.Empty;
    }

    private void Start()
    {
        ShowMessage(GAME_START_MESSAGE);
    }

    public void UpdateHealth(int health)
    {
        healthText.text = string.Format(HEALTH_TEXT_FORMAT, health);
    }

    public void UpdateMagazines(int magazines)
    {
        magazinesText.text = string.Format(MAGAZINES_TEXT_FORMAT, magazines);
    }

    public void UpdatePatrons(int patrones)
    {
        patronesText.text = string.Format(PATRONES_TEXT_FORMAT, patrones);
    }

    public void UpdateEnemiesCount(int count)
    {
        enemiesCountText.text = string.Format(ENEMIES_COUNT_TEXT_FORMAT, count);
    }

    public void ShowMessage(string message)
    {
        if (messageText.text != string.Empty)
        {
            messageQueue.Enqueue(message);
            return;
        }

        messageText.text = message;
        messageText.gameObject.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(delay));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        messageText.gameObject.SetActive(false);
        messageText.text = string.Empty;

        if (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            ShowMessage(nextMessage);
        }
    }
}