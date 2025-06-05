using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] int maxActiveEnemies = 5;

    private int currentEnemyIndex = 0;

    void Start()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Count active enemies
            int activeCount = 0;
            foreach (GameObject prefab in enemyPrefabs)
            {
                if (prefab.activeInHierarchy)
                {
                    activeCount++;
                }
            }

            // Spawn if under max limit
            if (activeCount < maxActiveEnemies)
            {
                // Get next enemy prefab
                GameObject enemyPrefab = enemyPrefabs[currentEnemyIndex];
                currentEnemyIndex = (currentEnemyIndex + 1) % enemyPrefabs.Length;

                // Spawn enemy
                GameObject enemy = Instantiate(enemyPrefab, transform);
                enemy.SetActive(true);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
} 