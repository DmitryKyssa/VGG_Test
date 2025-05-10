using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 100;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    private int currentHealth;

    public static Action<Enemy> EnemyKilled;

    private Transform playerTransform;
    private float lastAttackTime;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        Debug.Log($"Enemy took {damage} damage. Has {currentHealth}");
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        EnemyKilled?.Invoke(this);
    }

    private void Start()
    {
        playerTransform = PlayerMovementController.Instance.transform;
        lastAttackTime = Time.time;
        currentHealth = health;
        gameObject.SetTag(Tag.Enemy);
    }
}