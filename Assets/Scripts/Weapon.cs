using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData weaponData;
    private float lastFireTime = 0f;
    private int currentMagazines;
    private int currentPatrons;

    private InputAction fireAction;

    private void Awake()
    {
        fireAction = new InputAction("Fire", binding: "<Mouse>/leftButton");
        fireAction.performed += ctx => Fire();
        fireAction.Enable();

        currentMagazines = weaponData.magazines;
        currentPatrons = weaponData.patronsPerMagazine;

        UIManager.Instance.UpdateMagazines(currentMagazines);
        UIManager.Instance.UpdatePatrons(currentPatrons);
    }

    private void Fire()
    {
        if (Time.time - lastFireTime < weaponData.fireRate)
            return;

        StartCoroutine(PlayFireAnimation());

        currentPatrons--;
        UIManager.Instance.UpdatePatrons(currentPatrons);

        if (currentPatrons == 0 && currentMagazines > 0)
        {
            UIManager.Instance.ShowMessage(UIManager.RELOAD_MESSAGE);
            currentMagazines--;
            UIManager.Instance.UpdateMagazines(currentMagazines);
            UIManager.Instance.UpdatePatrons(weaponData.patronsPerMagazine);
            currentPatrons = weaponData.patronsPerMagazine;
        }
        else if (currentPatrons <= 0 && currentMagazines == 0)
        {
            UIManager.Instance.ShowMessage(UIManager.PATRONS_CANCELED_MESSAGE);
            return;
        }

        if (Physics.Raycast(firePoint.position, firePoint.up, out RaycastHit hit, weaponData.range))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponData.damage);
            }

            Debug.Log($"Hit: {hit.collider.name}");
        }

        lastFireTime = Time.time;
    }

    public void TakeMagazines(int magazines)
    {
        currentMagazines += magazines;
        UIManager.Instance.UpdateMagazines(currentMagazines);
    }

    private IEnumerator PlayFireAnimation()
    {
        Vector3 originalPosition = transform.localPosition;
        Vector3 recoilPosition = originalPosition;
        recoilPosition.z -= 0.1f;
        float recoilDuration = 0.1f;
        float elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }
}