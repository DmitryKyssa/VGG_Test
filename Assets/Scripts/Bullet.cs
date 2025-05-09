using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    private Weapon parentWeapon;
    private float speed;

    public void SetActiveStatus(bool status)
    {
        gameObject.SetActive(status);
        if (status)
        {
            StartCoroutine(Deactivate());
        }
    }

    public void SetParentWeapon(Weapon weapon)
    {
        parentWeapon = weapon;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(lifetime);
        parentWeapon.bulletPool.Release(this);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}