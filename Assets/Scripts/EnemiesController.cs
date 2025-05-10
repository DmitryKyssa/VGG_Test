using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemiesController : Singleton<EnemiesController>
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int initialEnemyCount = 5;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float spawnDelay = 5f;

    private ObjectPool<Enemy> enemyPool;

    private void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(
            createFunc: () => Instantiate(enemyPrefab, transform),
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            maxSize: 20
        );

        StartCoroutine(SpawnInitialEnemies());
        UIManager.Instance.UpdateEnemiesCount(enemyPool.CountActive);
        Enemy.EnemyKilled += EnemyKilling;
    }

    private IEnumerator SpawnInitialEnemies()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(spawnDelay);
        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemy();
            UIManager.Instance.ShowMessage(UIManager.ENEMY_SPAWNED_MESSAGE);
            UIManager.Instance.UpdateEnemiesCount(enemyPool.CountActive);
            yield return waitForSeconds;
        }
    }

    private void EnemyKilling(Enemy enemy)
    {
        ReturnEnemyToPool(enemy);
        UIManager.Instance.ShowMessage(UIManager.ENEMY_KILLED_MESSAGE);
        UIManager.Instance.UpdateEnemiesCount(enemyPool.CountActive);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = 1;
        Enemy enemy = enemyPool.Get();
        enemy.transform.position = spawnPosition;
        enemy.gameObject.SetActive(true);
    }

    public void ReturnEnemyToPool(Enemy enemy)
    {
        enemyPool.Release(enemy);
    }
}