using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : Singleton<WeaponController>
{
    private Weapon weapon;
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float aimFOV = 30f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private Camera playerCamera;
    private PlayerInput playerInput;
    private InputAction aimAction;
    private Vector3 defaultWeaponPosition;
    private Quaternion defaultWeaponRotation;
    private Vector3 aimedWeaponPosition;
    private Vector3 screenCenter;

    public Weapon Weapon => weapon;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponentInParent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];
        weapon = transform.GetComponentInChildren<Weapon>();

        if (weapon == null)
        {
            Weapon prefab = InventorySystem.Instance.WeaponType.GetWeaponPrefab();
            if (prefab != null)
            {
                weapon = Instantiate(prefab, transform);
            }
            else
            {
                Debug.LogError("No weapon prefab found for the current WeaponType.");
            }
        }

        defaultWeaponPosition = weapon.transform.position;
        defaultWeaponRotation = Quaternion.Inverse(Quaternion.LookRotation(transform.forward)) * weapon.transform.rotation;
        aimedWeaponPosition = defaultWeaponPosition + new Vector3(0f, 0f, 0.4f);
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    public void UpdateWeapon(Weapon newWeaponPrefab)
    {
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }

        weapon = Instantiate(newWeaponPrefab, transform);
    }

    private void Update()
    {
        HandleAiming();

        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 direction = targetPoint - weapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        weapon.transform.rotation = lookRotation * defaultWeaponRotation;
    }

    private void HandleAiming()
    {
        if (aimAction.triggered)
        {
            Aim();
        }

        if (aimAction.IsPressed())
        {
            Aim();
        }
        else
        {
            StopAiming();
        }
    }

    private void Aim()
    {
        weapon.transform.localPosition = Vector3.Lerp(weapon.transform.localPosition, aimedWeaponPosition, Time.deltaTime * aimSpeed);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimFOV, Time.deltaTime * aimSpeed);
    }

    private void StopAiming()
    {
        weapon.transform.localPosition = Vector3.Lerp(weapon.transform.localPosition, defaultWeaponPosition, Time.deltaTime * aimSpeed);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * aimSpeed);
    }

    private void OnEnable()
    {
        aimAction.Enable();
    }

    private void OnDisable()
    { 
        aimAction.Disable();
    }
}