using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    private NavMeshAgent navMeshAgent;

    [Header("Stats")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 100;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    private int currentHealth;
    private float lastAttackTime;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float losePlayerRange = 15f;

    [Header("Patrol")]
    private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    private int currentPatrolIndex = 0;
    private bool isWaiting = false;
    private float distanceToPlayer;

    [Header("Materials")]
    private Material defaultMaterial;
    [SerializeField] private Material hitMaterial;
    private Renderer rend;

    public static Action<Enemy> EnemyKilled;

    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState currentState;

    private void Awake()
    {
        patrolPoints = EnemiesController.Instance.PatrolPoints.ToArray();

        lastAttackTime = Time.time;
        currentHealth = health;
        gameObject.SetTag(Tag.Enemy);

        rend = GetComponent<Renderer>();
        defaultMaterial = rend.material;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        currentState = EnemyState.Patrol;
        navMeshAgent.enabled = false;
        StartCoroutine(EnableNavMeshAgent());
    }

    private void Update()
    {
        if (!navMeshAgent.enabled)
            return;

        distanceToPlayer = Vector3.Distance(transform.position, PlayerMovementController.Instance.transform.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();

                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    currentState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                ChasePlayer();

                if (distanceToPlayer > losePlayerRange)
                {
                    currentState = EnemyState.Patrol;
                    GoToNextPatrolPoint();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                break;

            case EnemyState.Attack:
                AttackPlayer();

                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }

    private IEnumerator EnableNavMeshAgent()
    {
        yield return new WaitForSeconds(1f);
        navMeshAgent.enabled = true;
        GoToNextPatrolPoint();
    }

    private void Patrol()
    {
        if (isWaiting)
            return;

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;

        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);

        GoToNextPatrolPoint();

        navMeshAgent.isStopped = false;
        isWaiting = false;
    }

    private void GoToNextPatrolPoint()
    {
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void ChasePlayer()
    {
        navMeshAgent.SetDestination(PlayerMovementController.Instance.transform.position);
    }

    private void AttackPlayer()
    {
        Vector3 playerTransform = PlayerMovementController.Instance.transform.position;
        transform.LookAt(new Vector3(playerTransform.x, transform.position.y, playerTransform.z));

        navMeshAgent.isStopped = true;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerHealthController.Instance.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 playerPosition = PlayerMovementController.Instance.transform.position;
        Ray ray = new Ray(transform.position, playerPosition - transform.position);
        return Physics.Raycast(ray, out RaycastHit hit, detectionRange) && hit.transform.position == playerPosition;
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Patrol)
        {
            currentState = EnemyState.Chase;
        }

        currentHealth -= damage;
        StartCoroutine(FlashHitMaterial());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashHitMaterial()
    {
        rend.material = hitMaterial;
        yield return new WaitForSeconds(1f);
        rend.material = defaultMaterial;
    }

    private void Die()
    {
        navMeshAgent.isStopped = true;
        EnemyKilled?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
    }
}