using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireTimeout = 3f;
    [SerializeField] private float bulletSpeed = 20f;
    private float lastFireTime = 0f;

    private InputAction fireAction;
    public ObjectPool<Bullet> bulletPool;

    private void Awake()
    {
        fireAction = new InputAction("Fire", binding: "<Mouse>/leftButton");
        fireAction.performed += ctx => Fire();
        fireAction.Enable();

        bulletPool = new ObjectPool<Bullet>(
            createFunc: () => Instantiate(bulletPrefab, GameObject.FindWithTag(Tag.BulletsPool.ToTagString()).transform),
            actionOnGet: bullet => bullet.gameObject.SetActive(true),
            actionOnRelease: bullet => bullet.gameObject.SetActive(false),
            actionOnDestroy: bullet => Destroy(bullet.gameObject),
            maxSize: 20
        );
    }

    private void Fire()
    {
        if (Time.time - lastFireTime < fireTimeout && lastFireTime != 0)
            return;

        Bullet bullet = bulletPool.Get();
        bullet.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        bullet.SetParentWeapon(this);
        bullet.SetActiveStatus(true);
        bullet.SetSpeed(bulletSpeed);

        lastFireTime = Time.time;
    }

    private void OnDisable()
    {
        fireAction.Disable();
    }
}