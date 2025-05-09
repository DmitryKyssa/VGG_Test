using UnityEngine;
using UnityEngine.Pool;

public class EnemiesController : Singleton<EnemiesController>
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private int initialEnemyCount = 5;
    [SerializeField] private float spawnRadius = 10f;

    private ObjectPool<Enemy> enemyPool;

    private void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(
            createFunc: () => Instantiate(enemyPrefab, enemiesParent),
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            maxSize: 20
        );

        SpawnInitialEnemies();
        UpdateEnemiesCount();
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < initialEnemyCount; i++)
        {
            SpawnEnemy();
        }
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

    public void UpdateEnemiesCount()
    {
        UIManager.Instance.UpdateEnemiesCount(enemyPool.CountActive);
    }

    public void ShowEnemyKilledMessage()
    {
        UIManager.Instance.ShowMessage(UIManager.EnemyKilledMessage, 2f);
    }
}