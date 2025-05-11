using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        StartCoroutine(AsyncLoadInventory());
    }

    private IEnumerator AsyncLoadInventory()
    {
        yield return new WaitForSeconds(1f);
        LoadInventory();
    }

    public void ActivateInput()
    {
        toggleInventoryUI = PlayerMovementController.Instance.PlayerInput.actions["Inventory"];
        toggleInventoryUI.performed += ctx => ToggleInventoryUI();
        toggleInventoryUI.Enable();
    }

    public void Initialize()
    {
        inventoryUI = FindFirstObjectByType<InventoryUIController>(FindObjectsInactive.Include).gameObject;
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
            SetDefaultData();
        }

        string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    private void SetDefaultData()
    {
        inventoryData.weaponTypes.Add(WeaponType.Red, true); //default weapon
        inventoryData.weaponTypes.Add(WeaponType.Black, false);
        inventoryData.weaponTypes.Add(WeaponType.Blue, false);

        inventoryData.patronTypes.Add(PatronType.Blue, true); //default patron
        inventoryData.patronTypes.Add(PatronType.Pink, false);
        inventoryData.patronTypes.Add(PatronType.Green, false);

        var pickableDatas = Resources.LoadAll<PickableData>("PickablesDatas"); 

        int[] healers = pickableDatas.Select(p => int.Parse(p.name.Replace("Health", ""))).ToArray();
        foreach (int healer in healers)
        {
            inventoryData.healers.Add(healer, 0);
        }

        int[] magazines = pickableDatas.Select(p => int.Parse(p.name.Replace("Magazines", ""))).ToArray();
        foreach (int magazine in magazines)
        {
            inventoryData.magazines.Add(magazine, 0);
        }
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
            Debug.LogWarning("Inventory file not found! Creating new!");

            SetDefaultData();
            string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }

    private void OnDisable()
    {
        toggleInventoryUI.performed -= ctx => ToggleInventoryUI();
        toggleInventoryUI.Disable();
    }
}