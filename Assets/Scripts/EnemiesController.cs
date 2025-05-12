using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemiesController : Singleton<EnemiesController>
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int allEnemiesCount = 5;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private List<Transform> patrolPoints;
    private ObjectPool<Enemy> enemyPool;
    private int killedEnemiesCount = 0;

    public int EnemyCount => allEnemiesCount;
    public List<Transform> PatrolPoints => patrolPoints;

    protected override void Awake()
    {
        base.Awake();
        patrolPoints.AddRange(spawnPoints);

        enemyPool = new ObjectPool<Enemy>(
            createFunc: () => Instantiate(enemyPrefab),
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            maxSize: allEnemiesCount
        );

        StartCoroutine(SpawnInitialEnemies());
        GameUIController.Instance.UpdateEnemiesCount(enemyPool.CountActive);
        Enemy.EnemyKilled += EnemyKilling;

        DontDestroyOnLoad(gameObject);
    }

    public void Iniztialize()
    {
        killedEnemiesCount = 0;
        enemyPool.Clear();
    }

    private IEnumerator SpawnInitialEnemies()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(spawnDelay);
        for (int i = 0; i < allEnemiesCount; i++)
        {
            SpawnEnemy();
            GameUIController.Instance.ShowMessage(GameUIController.ENEMY_SPAWNED_MESSAGE);
            GameUIController.Instance.UpdateEnemiesCount(enemyPool.CountActive);
            yield return waitForSeconds;
        }
    }

    private void EnemyKilling(Enemy enemy)
    {
        ReturnEnemyToPool(enemy);
        GameUIController.Instance.ShowMessage(GameUIController.ENEMY_KILLED_MESSAGE);
        GameUIController.Instance.UpdateEnemiesCount(enemyPool.CountActive);

        ++killedEnemiesCount;
        if (killedEnemiesCount == allEnemiesCount)
        {
            GameUIController.Instance.ShowFinishScreen(GameUIController.WIN_MESSAGE, isWin: true, LevelLoader.Instance.IsLastLevel);
        }
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomIndex].position;
        spawnPosition.y = 1;
        Enemy enemy = enemyPool.Get();
        enemy.transform.position = spawnPosition;
        enemy.Initialize();
        enemy.gameObject.SetActive(true);
    }

    public void ReturnEnemyToPool(Enemy enemy)
    {
        enemyPool.Release(enemy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var spawnPoint in spawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint.position, 3f);
        }
    }
}