using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Level Configuration")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform castleTransform;
    [SerializeField] private float spawnX = 20f; // X position for spawning enemies

    [Header("Battle State")]
    [SerializeField] private int castleHealth = 100;
    [SerializeField] private int currentCastleHealth;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isBattleActive = false;
    private int currentLaneIndex = 0;
    private int currentWaveIndex = 0;
    private int totalEnemiesSpawned = 0;
    private int totalEnemiesInLevel = 0;
    private bool isSpawningWave = false;

    public event Action<int, int> OnCastleHealthChanged;
    public event Action OnBattleVictory;
    public event Action OnBattleDefeat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (levelData == null)
        {
            Debug.LogError("No level data assigned to BattleManager!");
            return;
        }

        // Calculate total enemies in level
        CalculateTotalEnemies();
        currentCastleHealth = castleHealth;
        StartBattle();
    }

    private void CalculateTotalEnemies()
    {
        totalEnemiesInLevel = 0;
        foreach (var lane in levelData.lanes)
        {
            foreach (var wave in lane.waves)
            {
                totalEnemiesInLevel += wave.enemyCount;
            }
        }
        Debug.Log($"Total enemies in level: {totalEnemiesInLevel}");
    }

    public void StartBattle()
    {
        if (levelData == null)
        {
            Debug.LogError("No level data assigned to BattleManager!");
            return;
        }

        isBattleActive = true;
        currentLaneIndex = 0;
        currentWaveIndex = 0;
        totalEnemiesSpawned = 0;
        StartCoroutine(ProcessLanes());
    }

    private IEnumerator ProcessLanes()
    {
        Debug.Log($"Starting battle with {levelData.lanes.Count} lanes");
        
        while (currentLaneIndex < levelData.lanes.Count && isBattleActive)
        {
            LaneWaveData currentLane = levelData.lanes[currentLaneIndex];
            currentWaveIndex = 0;

            Debug.Log($"Processing lane {currentLaneIndex + 1} at Z position {currentLane.laneZ}");

            while (currentWaveIndex < currentLane.waves.Count && isBattleActive)
            {
                WaveData wave = currentLane.waves[currentWaveIndex];
                Debug.Log($"Starting wave {currentWaveIndex + 1} in lane {currentLaneIndex + 1} with {wave.enemyCount} enemies");
                
                yield return new WaitForSeconds(wave.startDelay);

                isSpawningWave = true;
                yield return StartCoroutine(SpawnWave(wave, currentLane.laneZ));
                isSpawningWave = false;

                // Wait for a short time after wave is spawned
                yield return new WaitForSeconds(1f);
                
                currentWaveIndex++;
                Debug.Log($"Completed wave {currentWaveIndex} in lane {currentLaneIndex + 1}");
            }

            currentLaneIndex++;
            Debug.Log($"Completed lane {currentLaneIndex}");
        }

        Debug.Log($"All lanes completed. Total enemies spawned: {totalEnemiesSpawned}/{totalEnemiesInLevel}");

        // Check if all enemies are defeated
        if (isBattleActive && activeEnemies.Count == 0)
        {
            Debug.Log("All waves completed and enemies defeated!");
            OnBattleVictory?.Invoke();
        }
    }

    private IEnumerator SpawnWave(WaveData wave, float laneZ)
    {
        Debug.Log($"Spawning wave with {wave.enemyCount} enemies of type {wave.enemyType.name}");
        
        for (int i = 0; i < wave.enemyCount; i++)
        {
            if (!isBattleActive) yield break;

            GameObject enemyObj = EnemyPoolManager.Instance.GetEnemy(wave.enemyType);
            if (enemyObj != null)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Set position and target
                    Vector3 spawnPos = new Vector3(spawnX, 0f, laneZ);
                    enemyObj.transform.position = spawnPos;
                    enemy.SetTarget(castleTransform.position, castleTransform);

                    // Subscribe to events
                    enemy.OnEnemyDeath += HandleEnemyDeath;
                    enemy.OnEnemyReachedCastle += HandleEnemyReachedCastle;

                    activeEnemies.Add(enemy);
                    totalEnemiesSpawned++;
                    
                    Debug.Log($"Spawned enemy {totalEnemiesSpawned}/{totalEnemiesInLevel} (Wave {currentWaveIndex + 1}, Lane {currentLaneIndex + 1})");
                }
                else
                {
                    Debug.LogError($"Enemy component not found on spawned object");
                }
            }
            else
            {
                Debug.LogError($"Failed to get enemy from pool for type {wave.enemyType.name}");
            }

            yield return new WaitForSeconds(wave.delayBetweenEnemies);
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        if (enemy == null) return;

        activeEnemies.Remove(enemy);
        enemy.OnEnemyDeath -= HandleEnemyDeath;
        enemy.OnEnemyReachedCastle -= HandleEnemyReachedCastle;

        Debug.Log($"Enemy died. Active enemies: {activeEnemies.Count}, Total spawned: {totalEnemiesSpawned}/{totalEnemiesInLevel}");

        // Only check for victory if we're not currently spawning a wave
        if (!isSpawningWave && isBattleActive && activeEnemies.Count == 0 && 
            currentLaneIndex >= levelData.lanes.Count)
        {
            Debug.Log("Victory condition met!");
            OnBattleVictory?.Invoke();
        }
    }

    private void HandleEnemyReachedCastle(Enemy enemy)
    {
        if (enemy == null) return;

        // Apply damage to castle
        TakeCastleDamage(10); // Default damage, can be modified by enemy stats
    }

    private void TakeCastleDamage(int damage)
    {
        currentCastleHealth = Mathf.Max(0, currentCastleHealth - damage);
        OnCastleHealthChanged?.Invoke(currentCastleHealth, castleHealth);

        Debug.Log($"Castle took damage. Health: {currentCastleHealth}/{castleHealth}");

        if (currentCastleHealth <= 0)
        {
            Debug.Log("Castle destroyed! Defeat!");
            isBattleActive = false;
            OnBattleDefeat?.Invoke();
        }
    }

    public void PauseBattle()
    {
        isBattleActive = false;
        Debug.Log("Battle paused");
    }

    public void ResumeBattle()
    {
        if (!isBattleActive)
        {
            Debug.Log("Resuming battle");
            isBattleActive = true;
            StartCoroutine(ProcessLanes());
        }
    }
} 