using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryData
{
    public Dictionary<WeaponType, bool> weaponTypes = new Dictionary<WeaponType, bool>();
    public Dictionary<PatronType, bool> patronTypes = new Dictionary<PatronType, bool>();
    public Dictionary<int, int> healers = new Dictionary<int, int>(); //key: amount in name, value: count. Ex: Healer10: 2 collected pickables - each gives 10 health
    public Dictionary<int, int> magazines = new Dictionary<int, int>(); //key: amount in name, value: count
}

public class InventorySystem : Singleton<InventorySystem>
{
    private const string PATRON_TYPE_KEY = "PatronType";
    private const string WEAPON_TYPE_KEY = "WeaponType";

    private const string SAVED_INVENTORY_FILE = "SavedInventory.json";

    private InputAction toggleInventoryUI;
    private GameObject inventoryUI;

    private InventoryData inventoryData = new InventoryData();
    public InventoryData InventoryData => inventoryData;

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
        set
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
        set
        {
            PlayerPrefs.SetInt(WEAPON_TYPE_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void ActivateInput()
    {
        toggleInventoryUI = PlayerMovementController.Instance.PlayerInput.actions["Inventory"];
        toggleInventoryUI.performed += ctx => ToggleInventoryUI();
        toggleInventoryUI.Enable();
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

    public void SaveInventory()
    {
        string path = Path.Combine(Application.streamingAssetsPath, SAVED_INVENTORY_FILE);

        if(!File.Exists(path))
        {
            inventoryData.weaponTypes.Add(WeaponType.Red, true); //default weapon
            inventoryData.patronTypes.Add(PatronType.Blue, true); //default patron
        }

        string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void LoadInventory()
    {
        string path = Path.Combine(Application.streamingAssetsPath, SAVED_INVENTORY_FILE);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            inventoryData = JsonConvert.DeserializeObject<InventoryData>(json);
        }
        else
        {
            Debug.LogError("Inventory file not found!");
        }
    }

    private void OnDisable()
    {
        toggleInventoryUI.performed -= ctx => ToggleInventoryUI();
        toggleInventoryUI.Disable();
    }
}