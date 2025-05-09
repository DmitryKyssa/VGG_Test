using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData weaponData;
    private float lastFireTime = 0f;

    private InputAction fireAction;

    private void Awake()
    {
        fireAction = new InputAction("Fire", binding: "<Mouse>/leftButton");
        fireAction.performed += ctx => Fire();
        fireAction.Enable();
    }

    private void Fire()
    {
        if (Time.time - lastFireTime < weaponData.fireRate)
            return;

        StartCoroutine(PlayFireAnimation());

        if (Physics.Raycast(firePoint.position, firePoint.up, out RaycastHit hit, weaponData.range))
        {
            Debug.Log($"Hit: {hit.collider.name}");
        }

        lastFireTime = Time.time;
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