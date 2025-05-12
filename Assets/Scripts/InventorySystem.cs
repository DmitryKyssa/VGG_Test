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
        yield return new WaitForSeconds(.5f);
        LoadInventory();
        InventoryUIController.Instance.Initialize();
    }

    public void Initialize()
    {
        inventoryUI = InventoryUIController.Instance.gameObject;
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
        }
    }

    public void ToggleInventoryUI()
    {
        inventoryUI = InventoryUIController.Instance.gameObject;
        if (inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(false);
            PlayerMovementController.Instance.PlayerInput.actions["Look"].Enable();
        }
        else
        {
            inventoryUI.SetActive(true);
            PlayerMovementController.Instance.PlayerInput.actions["Look"].Disable();
        }
    }

    public void AddWeapon(WeaponType weaponType)
    {
        if (!inventoryData.weaponTypes[weaponType])
        {
            inventoryData.weaponTypes[weaponType] = true;
        }
    }

    public void AddPatron(PatronType patronType)
    {
        if (!inventoryData.patronTypes[patronType])
        {
            inventoryData.patronTypes[patronType] = true;
        }
    }

    public void AddHealer(int amount)
    {
        inventoryData.healers[amount]++;
    }

    public void AddMagazine(int amount)
    {
        inventoryData.magazines[amount]++;
    }

    public void RemoveHealer(int amount)
    {
        if (inventoryData.healers[amount] > 0)
        {
            inventoryData.healers[amount]--;
            PlayerHealthController.Instance.Heal(amount);
        }
    }

    public void RemoveMagazine(int amount)
    {
        if (inventoryData.magazines[amount] > 0)
        {
            inventoryData.magazines[amount]--;
            WeaponController.Instance.Weapon.TakeMagazines(amount);
        }
    }

    public void SaveInventory()
    {
        string path = Path.Combine(Application.streamingAssetsPath, SAVED_INVENTORY_FILE);

        if (!File.Exists(path))
        {
            SetDefaultData();
        }

        string json = JsonConvert.SerializeObject(inventoryData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    private void SetDefaultData()
    {
        inventoryData.weaponTypes.Add(WeaponType.Red, true); //default weapon
        inventoryData.weaponTypes.Add(WeaponType.Blue, false);
        inventoryData.weaponTypes.Add(WeaponType.Black, false);

        inventoryData.patronTypes.Add(PatronType.Blue, true); //default patron
        inventoryData.patronTypes.Add(PatronType.Green, false);
        inventoryData.patronTypes.Add(PatronType.Pink, false);

        var pickableDatas = Resources.LoadAll<PickableData>("PickablesDatas");

        int[] healers = pickableDatas.Where(p => p.pickableType == PickableType.Health)
            .Select(p => p.pickableValue).ToArray();
        foreach (int healer in healers)
        {
            inventoryData.healers.Add(healer, 0);
        }

        int[] magazines = pickableDatas.Where(p => p.pickableType == PickableType.Magazines)
            .Select(p => p.pickableValue).ToArray();
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
}