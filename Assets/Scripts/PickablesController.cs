using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PickablesController : Singleton<PickablesController>
{
    [SerializeField] private Transform[] pickablesSpawnPoints;
    [SerializeField] private Pickable[] pickablePrefab;
    [SerializeField] private float delay = 20f;
    private int currentSpawnPointIndex = 0;
    private int currentPickableIndex = 0;

    private ObjectPool<Pickable> pickablePool;

    private void Awake()
    {
        pickablePool = new ObjectPool<Pickable>(
            CreatePickable,
            OnGetPickable,
            OnReleasePickable,
            OnDestroyPickable,
            maxSize: 10
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var spawnPoint in pickablesSpawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint.position, 3f);
        }
    }

    private IEnumerator Start()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);
        while (true)
        {           
            yield return waitForSeconds;
            Pickable pickable = pickablePool.Get();
            pickable.gameObject.SetActive(true);
        }
    }

    private Pickable CreatePickable()
    {
        int lastIndex = currentPickableIndex;
        //while (currentPickableIndex == lastIndex)
        //{
        //    currentPickableIndex = Random.Range(0, pickablePrefab.Length);
        //}

        //int lastSpawnPointIndex = currentSpawnPointIndex;
        //while (currentSpawnPointIndex == lastSpawnPointIndex)
        //{
        //    currentSpawnPointIndex = Random.Range(0, pickablesSpawnPoints.Length);
        //}

        return Instantiate(
            pickablePrefab[currentPickableIndex],
            pickablesSpawnPoints[currentSpawnPointIndex].position,
            Quaternion.identity,
            transform);
    }

    private void OnGetPickable(Pickable pickable)
    {
        pickable.gameObject.SetActive(true);
        int lastSpawnPointIndex = currentSpawnPointIndex;
        //while (currentSpawnPointIndex == lastSpawnPointIndex)
        //{
        //    currentSpawnPointIndex = Random.Range(0, pickablesSpawnPoints.Length);
        //}
        pickable.transform.position = pickablesSpawnPoints[currentSpawnPointIndex].position;
    }

    public void OnReleasePickable(Pickable pickable)
    {
        pickable.gameObject.SetActive(false);
    }

    private void OnDestroyPickable(Pickable pickable)
    {
        Destroy(pickable.gameObject);
    }
}