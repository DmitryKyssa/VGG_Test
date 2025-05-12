using System.Collections;
using TMPro;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    private PickableData pickableData;
    private Collider pickableCollider;
    [SerializeField] private TextMeshProUGUI pickableNameText;

    private void Start()
    {
        pickableCollider = GetComponent<Collider>();
        pickableCollider.isTrigger = true;

        StartCoroutine(ShowRotatingPickableName());
    }

    private IEnumerator ShowRotatingPickableName()
    {
        while (true)
        {
            pickableNameText.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            SetPickableEffect();
            PickablesController.Instance.PickablePool.Release(this);
        }
    }

    public void SetPickableData(PickableData data)
    {
        pickableData = data;
        gameObject.name = pickableData.pickableName;
        pickableNameText.text = pickableData.pickableName;
    }

    private void SetPickableEffect()
    {
        switch (pickableData.pickableType)
        {
            case PickableType.Weapon:
                InventorySystem.Instance.AddWeapon(pickableData.weaponType);
                InventoryUIController.Instance.SetWeaponActive(pickableData.weaponType);
                break;
            case PickableType.Health:
                PlayerHealthController.Instance.Heal(pickableData.pickableValue);
                break;
            case PickableType.Magazines:
                WeaponController.Instance.Weapon.TakeMagazines(pickableData.pickableValue);
                break;
            case PickableType.Patron:
                InventorySystem.Instance.AddPatron(pickableData.patronType);
                InventoryUIController.Instance.SetPatronActive(pickableData.patronType);
                break;
            default:
                Debug.LogError("Unknown pickable type");
                break;
        }

        Debug.Log($"Pickable effect applied: {pickableData.pickableName}");
    }
}