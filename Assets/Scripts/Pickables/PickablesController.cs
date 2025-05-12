using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PickablesController : Singleton<PickablesController>
{
    [SerializeField] private Transform[] pickablesSpawnPoints;
    [SerializeField] private Pickable pickablePrefab;
    [SerializeField] private float delay = 20f;
    private int defaultLayerMask = 0;
    private List<PickableData> pickableDatas;

    private ObjectPool<Pickable> pickablePool;
    public ObjectPool<Pickable> PickablePool => pickablePool;

    protected override void Awake()
    {
        base.Awake();
        pickablePool = new ObjectPool<Pickable>(
            CreatePickable,
            OnGetPickable,
            OnReleasePickable,
            OnDestroyPickable,
            maxSize: pickablesSpawnPoints.Length
        );

        defaultLayerMask = LayerMask.GetMask("Default");

        DontDestroyOnLoad(gameObject);
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
        pickableDatas = Resources.LoadAll<PickableData>("PickablesDatas").ToList();

        pickableDatas.RemoveAll(p => (p.pickableType == PickableType.Weapon && 
            InventorySystem.Instance.InventoryData.weaponTypes[p.weaponType]) ||
            (p.pickableType == PickableType.Patron &&
            InventorySystem.Instance.InventoryData.patronTypes[p.patronType]));

        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;

            while (pickablePool.CountActive == pickablesSpawnPoints.Length)
            {
                yield return null;
            }

            pickablePool.Get();
        }
    }

    private Pickable CreatePickable()
    {
        Pickable pickable = Instantiate(pickablePrefab, Vector3.zero, Quaternion.identity);
        pickable.gameObject.SetActive(false);
        return pickable;
    }

    private void OnGetPickable(Pickable pickable)
    {
        if (pickable == null)
        {
            CreatePickable();
        }

        List<int> shuffledIndices = new List<int>();
        for (int i = 0; i < pickablesSpawnPoints.Length; i++)
        {
            shuffledIndices.Add(i);
        }

        for (int i = 0; i < shuffledIndices.Count; i++)
        {
            int randIndex = Random.Range(i, shuffledIndices.Count);
            (shuffledIndices[i], shuffledIndices[randIndex]) = (shuffledIndices[randIndex], shuffledIndices[i]);
        }

        foreach (int index in shuffledIndices)
        {
            Vector3 pos = pickablesSpawnPoints[index].position;

            if (!Physics.CheckSphere(pos, 0.25f, defaultLayerMask, QueryTriggerInteraction.Collide))
            {
                pickable.transform.position = pos;
                pickable.gameObject.SetActive(true);
                pickable.SetPickableData(GetPickableData());
                return;
            }
        }

        Debug.LogWarning("No free spawn points found for pickable.");
        pickablePool.Release(pickable);
    }

    private PickableData GetPickableData()
    {
        return pickableDatas[Random.Range(0, pickableDatas.Count)];
    }

    private void OnReleasePickable(Pickable pickable)
    {
        pickable.gameObject.SetActive(false);
    }

    private void OnDestroyPickable(Pickable pickable)
    {
        Destroy(pickable.gameObject);
    }
}