using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIController : Singleton<GameUIController>
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI magazinesText;
    [SerializeField] private TextMeshProUGUI patronesText;
    [SerializeField] private TextMeshProUGUI enemiesCountText;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private GameObject finishScreen;
    [SerializeField] private TextMeshProUGUI finishText;

    private const string HEALTH_TEXT_FORMAT = "Health: {0}/{1}";
    private const string MAGAZINES_TEXT_FORMAT = "Magazines: {0}/{1}";
    private const string PATRONES_TEXT_FORMAT = "Patrones: {0}/{1}";
    private const string ENEMIES_COUNT_TEXT_FORMAT = "Enemies: {0}/{1}";

    public const string GAME_START_MESSAGE = "Game Start!";
    public const string RELOAD_MESSAGE = "Reload!";
    public const string ENEMY_KILLED_MESSAGE = "Enemy Killed!";
    public const string ENEMY_SPAWNED_MESSAGE = "Enemy Spawned!";
    public const string PATRONS_CANCELED_MESSAGE = "Patrons Cancelled!";

    private const string WIN_MESSAGE = "You Win!";
    private const string LOSE_MESSAGE = "You Lose!";

    private readonly Queue<string> messageQueue = new Queue<string>();
    private readonly float delay = 1f;

    private int maxHealth;
    private int maxEnemiesCount;

    private void Awake()
    {
        messageText.gameObject.SetActive(false);
        messageText.text = string.Empty;
        maxHealth = PlayerHealthController.Instance.MaxHealth;
        maxEnemiesCount = EnemiesController.Instance.EnemyCount;

        UpdateEnemiesCount(maxEnemiesCount);
        UpdateHealth(maxHealth);
    }

    private void Start()
    {
        ShowMessage(GAME_START_MESSAGE);
    }

    public void UpdateHealth(int health)
    {
        healthText.text = string.Format(HEALTH_TEXT_FORMAT, health, maxHealth);
    }

    public void UpdateMagazines(int currentMagazines, int maxMagazinesForCurrentWeapon)
    {
        magazinesText.text = string.Format(MAGAZINES_TEXT_FORMAT, currentMagazines, maxMagazinesForCurrentWeapon);
    }

    public void UpdatePatrons(int currentPatrones, int maxPatronesForCurrentWeapon)
    {
        patronesText.text = string.Format(PATRONES_TEXT_FORMAT, currentPatrones, maxPatronesForCurrentWeapon);
    }

    public void UpdateEnemiesCount(int count)
    {
        enemiesCountText.text = string.Format(ENEMIES_COUNT_TEXT_FORMAT, count, maxEnemiesCount);
    }

    public void ShowFinishScreen(bool isWin)
    {
        finishScreen.SetActive(true);
        finishText.text = isWin ? WIN_MESSAGE : LOSE_MESSAGE;
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