using UnityEngine;
using System.Collections.Generic;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    [System.Serializable]
    public class EnemyPool
    {
        public EnemyData enemyType;
        public GameObject enemyPrefab;
        public int initialPoolSize = 20;
        public int maxPoolSize = 100; // Maximum pool size to prevent memory issues
    }

    [SerializeField] private List<EnemyPool> enemyPools = new List<EnemyPool>();
    private Dictionary<EnemyData, Queue<GameObject>> poolDictionary = new Dictionary<EnemyData, Queue<GameObject>>();
    private Dictionary<EnemyData, int> currentPoolSizes = new Dictionary<EnemyData, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var pool in enemyPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            currentPoolSizes[pool.enemyType] = 0;

            for (int i = 0; i < pool.initialPoolSize; i++)
            {
                CreateNewEnemy(pool, objectPool);
            }

            poolDictionary[pool.enemyType] = objectPool;
        }
    }

    private void CreateNewEnemy(EnemyPool pool, Queue<GameObject> objectPool)
    {
        if (currentPoolSizes[pool.enemyType] >= pool.maxPoolSize)
        {
            Debug.LogWarning($"Pool for {pool.enemyType.name} has reached maximum size of {pool.maxPoolSize}");
            return;
        }

        GameObject obj = Instantiate(pool.enemyPrefab);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        currentPoolSizes[pool.enemyType]++;
    }

    public GameObject GetEnemy(EnemyData enemyType)
    {
        if (!poolDictionary.ContainsKey(enemyType))
        {
            Debug.LogError($"No pool found for enemy type: {enemyType.name}");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[enemyType];
        
        // If pool is empty, try to create new enemies
        if (pool.Count == 0)
        {
            EnemyPool enemyPool = enemyPools.Find(p => p.enemyType == enemyType);
            if (enemyPool != null)
            {
                // Create multiple new enemies to prevent constant pool expansion
                int newEnemiesToCreate = Mathf.Min(5, enemyPool.maxPoolSize - currentPoolSizes[enemyType]);
                for (int i = 0; i < newEnemiesToCreate; i++)
                {
                    CreateNewEnemy(enemyPool, pool);
                }
            }
        }

        // If still no enemies available, return null
        if (pool.Count == 0)
        {
            Debug.LogWarning($"No enemies available in pool for {enemyType.name}");
            return null;
        }

        GameObject obj = pool.Dequeue();
        if (obj != null)
        {
            obj.SetActive(true);
        }
        return obj;
    }

    public void ReturnToPool(GameObject enemy)
    {
        if (enemy == null) return;

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError("Enemy component not found on object being returned to pool");
            return;
        }

        EnemyStats stats = enemyComponent.GetComponent<EnemyStats>();
        if (stats == null || stats.Data == null)
        {
            Debug.LogError("EnemyStats or EnemyData not found on object being returned to pool");
            return;
        }

        EnemyData enemyType = stats.Data;
        if (!poolDictionary.ContainsKey(enemyType))
        {
            Debug.LogError($"No pool found for enemy type: {enemyType.name}");
            return;
        }

        if (enemy != null)
        {
            enemy.SetActive(false);
            poolDictionary[enemyType].Enqueue(enemy);
        }
    }
} 