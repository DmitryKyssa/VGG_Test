using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private GameObject inventoryUI;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        LevelLoader.Instance.LoadNextLevel();
    }

    private void OnExitButtonClicked()
    {
        InventorySystem.Instance.SaveInventory();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnInventoryButtonClicked()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}