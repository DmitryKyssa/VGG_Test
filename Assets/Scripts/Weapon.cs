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

        GameUIController.Instance.UpdateMagazines(currentMagazines, weaponData.magazines);
        GameUIController.Instance.UpdatePatrons(currentPatrons, weaponData.patronsPerMagazine);
    }

    private void Fire()
    {
        if (Time.time - lastFireTime < weaponData.fireRate)
            return;

        if (currentPatrons <= 0 && currentMagazines == 0)
        {
            currentPatrons = 0;
            GameUIController.Instance.ShowMessage(GameUIController.PATRONS_CANCELED_MESSAGE);
            return;
        }

        StartCoroutine(PlayFireAnimation());

        GameUIController.Instance.UpdatePatrons(--currentPatrons, weaponData.patronsPerMagazine);

        if (currentPatrons == 0 && currentMagazines > 0)
        {
            GameUIController.Instance.ShowMessage(GameUIController.RELOAD_MESSAGE);
            currentMagazines--;
            GameUIController.Instance.UpdateMagazines(currentMagazines, weaponData.magazines);
            GameUIController.Instance.UpdatePatrons(weaponData.patronsPerMagazine, weaponData.patronsPerMagazine);
            currentPatrons = weaponData.patronsPerMagazine;
        }
        else if (currentPatrons <= 0 && currentMagazines == 0)
        {
            GameUIController.Instance.ShowMessage(GameUIController.PATRONS_CANCELED_MESSAGE);
            return;
        }

        if (Physics.Raycast(firePoint.position, firePoint.up, out RaycastHit hit, weaponData.range))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponData.damage);
            }
        }

        lastFireTime = Time.time;
    }

    public void TakeMagazines(int magazines)
    {
        currentMagazines += magazines;
        GameUIController.Instance.UpdateMagazines(currentMagazines, weaponData.magazines);
        Debug.Log($"Added {magazines} magazines. Current magazines: {currentMagazines}");
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
        fireAction.performed -= ctx => Fire();
        fireAction.Disable();
    }
}