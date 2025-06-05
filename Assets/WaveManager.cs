using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenEnemies = 1f;
    }

    [SerializeField] List<ObjectPool> enemyPools;
    [SerializeField] List<Wave> waves;
    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] Transform spawnPoint;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        if (enemyPools.Count == 0)
        {
            Debug.LogError("No enemy pools assigned!");
            return;
        }

        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWave >= waves.Count)
        {
            Debug.Log("All waves completed!");
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWave]));
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // Select random pool
            ObjectPool selectedPool = enemyPools[Random.Range(0, enemyPools.Count)];
            
            // Get enemy from pool and set position
            GameObject enemy = selectedPool.GetFromPool();
            if (enemy != null)
            {
                enemy.transform.position = spawnPoint.position;
            }

            yield return new WaitForSeconds(wave.timeBetweenEnemies);
        }

        isSpawning = false;
        currentWave++;

        // Wait before starting next wave
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }
} 