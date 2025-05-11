using UnityEngine;

public class Pickable : MonoBehaviour
{
    private PickableData pickableData;
    private Collider pickableCollider;

    private void Start()
    {
        gameObject.SetTag(Tag.Pickable);
        pickableCollider = GetComponent<Collider>();
        pickableCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            SetPickableEffect();
            PickablesController.Instance.PickablePool.Release(this);
        }
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
            default:
                Debug.LogError("Unknown pickable type");
                break;
        }
    }
}