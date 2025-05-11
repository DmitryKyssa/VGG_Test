using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : Singleton<InventorySystem>
{
    private const string PATRON_TYPE_KEY = "PatronType";
    private const string WEAPON_TYPE_KEY = "WeaponType";

    private InputAction toggleInventoryUI;
    private GameObject inventoryUI;

    public PatronType PatronType
    {
        get
        {
            if (PlayerPrefs.HasKey(PATRON_TYPE_KEY))
            {
                return (PatronType)PlayerPrefs.GetInt(PATRON_TYPE_KEY);
            }
            else
            {
                return PatronType.Blue; // Default value
            }
        }
        private set
        {
            PlayerPrefs.SetInt(PATRON_TYPE_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

    public WeaponType WeaponType
    {
        get
        {
            if (PlayerPrefs.HasKey(WEAPON_TYPE_KEY))
            {
                return (WeaponType)PlayerPrefs.GetInt(WEAPON_TYPE_KEY);
            }
            else
            {
                return WeaponType.Red; // Default value
            }
        }
        private set
        {
            PlayerPrefs.SetInt(WEAPON_TYPE_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

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
            PlayerMovementController.Instance.PlayerInput.actions["Look"].Enable();
        }
        else
        {
            Time.timeScale = 0;
            inventoryUI.SetActive(true);
            PlayerMovementController.Instance.PlayerInput.actions["Look"].Disable();
        }
    }

    private void OnDestroy()
    {
        toggleInventoryUI.performed -= ctx => ToggleInventoryUI();
        toggleInventoryUI.Disable();
    }
}