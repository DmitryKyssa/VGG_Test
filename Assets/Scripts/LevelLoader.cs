using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    private string[] levelNames;
    private int currentLevelIndex = 0;

    public bool IsLastLevel => currentLevelIndex == levelNames.Length - 1;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        levelNames = new string[SceneManager.sceneCountInBuildSettings];
        for (int i = 0; i < levelNames.Length; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            levelNames[i] = Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"Level {i}: {levelNames[i]}");
        }

#if UNITY_EDITOR
        // This is just for debugging purposes. By clicking on Tab next level will be loaded.
        InputAction debugAction = new InputAction(type: InputActionType.Button);
        debugAction.AddCompositeBinding("ButtonWithOneModifier")
            .With("Modifier", "<Keyboard>/tab")
            .With("Button", "<Keyboard>/tab");
        debugAction.performed += ctx => LoadNextLevel();
        debugAction.Enable();
#endif
    }

    public void GetCurrentLevelIndex()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadCurrentLevel()
    {
        Time.timeScale = 1;
        string levelName = levelNames[currentLevelIndex];
        SceneManager.LoadScene(levelName);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levelNames.Length)
        {
            return;
        }

        ClearScene();
        GameUIController.Instance.HideFinishScreen();
        LoadCurrentLevel();
    }

    private void ClearScene()
    {
        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj == gameObject || obj.name == "[Debug Updater]")
                continue;

            if (obj.scene == SceneManager.GetActiveScene())
            {
                Destroy(obj);
            }
        }
    }

    public void ReloadCurrentLevel()
    {
        ClearScene();
        PlayerHealthController.Instance.Initialize();
        EnemiesController.Instance.Iniztialize();
        InventorySystem.Instance.Initialize();
        GameUIController.Instance.Initialize();
        LoadCurrentLevel();
    }
}