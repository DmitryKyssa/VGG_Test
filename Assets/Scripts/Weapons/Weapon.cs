using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData weaponData;
    private PatronData patronData;
    private float lastFireTime = 0f;
    private int currentMagazines;
    private int currentPatrons;

    private InputAction fireAction;

    private void Awake()
    {
        currentMagazines = weaponData.magazines;
        currentPatrons = weaponData.patronsPerMagazine;

        GameUIController.Instance.UpdateMagazines(currentMagazines, weaponData.magazines);
        GameUIController.Instance.UpdatePatrons(currentPatrons, weaponData.patronsPerMagazine);
        SetPatronData();
    }

    public void SetPatronData()
    {
        patronData = Resources.Load<PatronData>($"PatronsDatas/{InventorySystem.Instance.PatronType}");
    }

    private void OnEnable()
    {
        fireAction = PlayerMovementController.Instance.PlayerInput.actions["Fire"];
        fireAction.performed += ctx => Fire();
        fireAction.Enable();
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

        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.up * weaponData.range, Color.yellow, 1f);
        if (Physics.Raycast(firePoint.position, firePoint.up, out RaycastHit hit, weaponData.range))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(patronData.damage);
            }
        }

        lastFireTime = Time.time;
    }

    public void TakeMagazines(int magazines)
    {
        if (currentMagazines >= weaponData.magazines)
        {
            currentMagazines = weaponData.magazines;
            InventorySystem.Instance.AddMagazine(magazines);
        }
        else
        {
            currentMagazines += magazines;
        }

        GameUIController.Instance.UpdateMagazines(currentMagazines, weaponData.magazines);
    }

    private IEnumerator PlayFireAnimation()
    {
        if (gameObject == null)
            yield break;

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

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}