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
        gameObject.SetTag(Tag.Pickable);
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
                //PlayerController.Instance.SetWeapon(pickableData.weaponType);
                break;
            case PickableType.Health:
                PlayerHealthController.Instance.Heal(pickableData.pickableValue);
                break;
            case PickableType.Magazines:
                WeaponController.Instance.Weapon.TakeMagazines(pickableData.pickableValue);
                break;
            case PickableType.Patron:
                //PatronController.Instance.AddPatron(pickableData.patronType);
                break;
            default:
                Debug.LogError("Unknown pickable type");
                break;
        }

        Debug.Log($"Pickable effect applied: {pickableData.pickableName}");
    }
}