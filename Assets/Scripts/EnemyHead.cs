using System.Collections;
using UnityEngine;


public class EnemyHead : MonoBehaviour, IDamageable
{
    [SerializeField] private Material hitMaterial;
    private Material defaultMaterial;
    private Renderer headRenderer;
    private IDamageable parent;

    public void TakeDamage(int damage)
    {
        parent.TakeDamage(damage);
        StartCoroutine(FlashHitEffect());
    }

    private IEnumerator FlashHitEffect()
    {
        headRenderer.material = hitMaterial;
        yield return new WaitForSeconds(1f);
        headRenderer.material = defaultMaterial;
    }

    private void Awake()
    {
        headRenderer = GetComponent<Renderer>();
        defaultMaterial = headRenderer.material;
        parent = GetComponentInParent<IDamageable>();

        gameObject.SetTag(Tag.EnemyHead);
    }
}