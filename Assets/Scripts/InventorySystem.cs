using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : Singleton<InventorySystem>
{
    private InputAction toggleInventoryUI;
    private GameObject inventoryUI;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        toggleInventoryUI = new InputAction("ToggleInventoryUI", binding: "<Keyboard>/i");
        toggleInventoryUI.performed += ctx => ToggleInventoryUI();
        toggleInventoryUI.Enable();
        Initialize();
    }

    public void Initialize()
    {
        inventoryUI = InventoryUIController.Instance.gameObject;
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
    }

    private void ToggleInventoryUI()
    {
        if (inventoryUI.activeSelf)
        {
            Time.timeScale = 1;
            inventoryUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            inventoryUI.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        toggleInventoryUI.performed -= ctx => ToggleInventoryUI();
        toggleInventoryUI.Disable();
    }
}