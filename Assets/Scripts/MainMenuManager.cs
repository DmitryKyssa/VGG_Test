using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager>
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
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

    private void OnContinueButtonClicked()
    {
        //save logic
    }
}