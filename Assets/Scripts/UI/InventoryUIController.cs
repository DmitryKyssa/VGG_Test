using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct WeaponButton
{
    public Button button;
    public WeaponType weaponType;
}

[Serializable]
public struct PatronButton
{
    public Button button;
    public PatronType patronType;
}

[Serializable]
public struct ValueCountButton
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI valueText;
    public Button button;
}

public class InventoryUIController : Singleton<InventoryUIController>
{
    [Header("Buttons")]
    [SerializeField] private WeaponButton[] weaponsButtons = new WeaponButton[3];
    [SerializeField] private ValueCountButton[] magazinesButtons = new ValueCountButton[3];
    [SerializeField] private ValueCountButton[] healersButtons = new ValueCountButton[3];
    [SerializeField] private PatronButton[] patronsButtons = new PatronButton[3];

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI saveSelectionText;

    [Header("Buttons")]
    [SerializeField] private Button applyButton;
    [SerializeField] private Button exitButton;

    private InventoryData localInventoryData;
    private int selectedWeaponIndex = -1;
    private int selectedPatronIndex = -1;
    private int selectedHealerIndex = -1;
    private int selectedMagazineIndex = -1;

    private int SceneIndex => SceneManager.GetActiveScene().buildIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        localInventoryData = InventorySystem.Instance.InventoryData;
        InitializeButtons();
    }

    public void SetWeaponActive(WeaponType weaponType)
    {
        for (int i = 0; i < weaponsButtons.Length; i++)
        {
            if (weaponsButtons[i].weaponType == weaponType)
            {
                weaponsButtons[i].button.interactable = true;
                break;
            }
        }
    }

    public void SetPatronActive(PatronType patronType)
    {
        for (int i = 0; i < patronsButtons.Length; i++)
        {
            if (patronsButtons[i].patronType == patronType)
            {
                patronsButtons[i].button.interactable = true;
                break;
            }
        }
    }

    public void SetMagazinesCount(int key, int value)
    {
        for (int i = 0; i < magazinesButtons.Length; i++)
        {
            if (magazinesButtons[i].valueText.text == key.ToString())
            {
                int parsed = int.Parse(magazinesButtons[i].countText.text);
                magazinesButtons[i].countText.text = (parsed + value).ToString();
                break;
            }
        }
    }

    public void SetHealersCount(int key, int value)
    {
        for (int i = 0; i < healersButtons.Length; i++)
        {
            if (healersButtons[i].valueText.text == key.ToString())
            {
                int parsed = int.Parse(healersButtons[i].countText.text); 
                healersButtons[i].countText.text = (parsed + value).ToString();
                break;
            }
        }
    }

    private void InitializeButtons()
    {
        applyButton.onClick.AddListener(OnApplyButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        for (int i = 0; i < weaponsButtons.Length; i++)
        {
            int index = i;
            weaponsButtons[i].button.onClick.AddListener(() => OnWeaponButtonClicked(index, weaponsButtons[index].weaponType));
            weaponsButtons[i].button.interactable = localInventoryData.weaponTypes[weaponsButtons[i].weaponType];
        }

        int j = 0;
        foreach (var invMag in localInventoryData.magazines)
        {
            int index = j;
            magazinesButtons[j].valueText.text = invMag.Key.ToString();
            magazinesButtons[j].countText.text = invMag.Value.ToString();
            magazinesButtons[j].button.onClick.AddListener(() => OnMagazineButtonClicked(index, invMag.Value.ToString(), invMag.Key.ToString()));
            magazinesButtons[j].button.interactable = SceneIndex != 0;
            j++;
        }

        j = 0;
        foreach (var invHealer in localInventoryData.healers)
        {
            int index = j;
            healersButtons[j].valueText.text = invHealer.Key.ToString();
            healersButtons[j].countText.text = invHealer.Value.ToString();
            healersButtons[j].button.onClick.AddListener(() => OnHealerButtonClicked(index, invHealer.Value.ToString(), invHealer.Key.ToString()));
            healersButtons[j].button.interactable = SceneIndex != 0;
            j++;
        }

        for (int i = 0; i < patronsButtons.Length; i++)
        {
            int index = i;
            patronsButtons[i].button.onClick.AddListener(() => OnPatronButtonClicked(index, patronsButtons[index].patronType));
            patronsButtons[i].button.interactable = localInventoryData.patronTypes[patronsButtons[i].patronType];
        }
    }

    private void OnWeaponButtonClicked(int index, WeaponType weaponType)
    {
        if (selectedWeaponIndex == index)
        {
            selectedWeaponIndex = -1;
            descriptionText.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        selectedWeaponIndex = index;
        descriptionText.text = weaponType.GetWeaponData().Stringify();
    }

    private void OnMagazineButtonClicked(int index, string count, string value)
    {
        if (selectedMagazineIndex == index)
        {
            selectedMagazineIndex = -1;
            descriptionText.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        applyButton.interactable = SceneIndex != 0;

        selectedMagazineIndex = index;
        descriptionText.text = $"{count} packs with {value} magazines";
    }

    private void OnHealerButtonClicked(int index, string count, string value)
    {
        if (selectedHealerIndex == index)
        {
            selectedHealerIndex = -1;
            descriptionText.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        applyButton.interactable = SceneIndex != 0;

        selectedHealerIndex = index;
        descriptionText.text = $"{count} packs, each has {value} healer' points";
    }

    private void OnPatronButtonClicked(int index, PatronType patronType)
    {
        if (selectedPatronIndex == index)
        {
            selectedPatronIndex = -1;
            descriptionText.text = string.Empty;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        selectedPatronIndex = index;
        descriptionText.text = patronType.GetPatronData().Stringify();
    }

    private void OnApplyButtonClicked()
    {
        if (selectedWeaponIndex != -1)
        {
            InventorySystem.Instance.WeaponType = weaponsButtons[selectedWeaponIndex].weaponType;

            if (SceneIndex != 0)
            {
                WeaponController.Instance.UpdateWeapon(weaponsButtons[selectedWeaponIndex].weaponType.GetWeaponPrefab());
            }
        }

        if (selectedMagazineIndex != -1 && SceneIndex != 0)
        {
            int value = int.Parse(magazinesButtons[selectedMagazineIndex].valueText.text);
            InventorySystem.Instance.RemoveMagazine(value);
            SetMagazinesCount(value, -1);
        }

        if (selectedHealerIndex != -1 && SceneIndex != 0)
        {
            int value = int.Parse(healersButtons[selectedHealerIndex].valueText.text);
            InventorySystem.Instance.RemoveHealer(value);
            SetHealersCount(value, -1);
        }

        if (selectedPatronIndex != -1)
        {
            InventorySystem.Instance.PatronType = patronsButtons[selectedPatronIndex].patronType;

            if (SceneIndex != 0)
            {
                WeaponController.Instance.Weapon.SetPatronData();
            }
        }

        saveSelectionText.gameObject.SetActive(true);
        StartCoroutine(HideSaveSelectionTextAfterDelay(1f));
        InventorySystem.Instance.SaveInventory();
    }

    private IEnumerator HideSaveSelectionTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        saveSelectionText.gameObject.SetActive(false);
    }

    private void OnExitButtonClicked()
    {
        gameObject.SetActive(false);

        if (SceneIndex != 0)
        {
            PlayerMovementController.Instance.PlayerInput.actions["Look"].Enable();
        }
    }
}