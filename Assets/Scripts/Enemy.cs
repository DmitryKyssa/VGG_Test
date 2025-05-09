using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 100;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform playerTransform;
    private float lastAttackTime;

    private void Start()
    {
        playerTransform = PlayerController.Instance.transform;
        lastAttackTime = Time.time;
    }
}