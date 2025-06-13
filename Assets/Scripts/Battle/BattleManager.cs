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
    [SerializeField] private float spawnX = 20f;
    [SerializeField] private float zRandomRange = 0.5f; // Random range for Z position

    [Header("Battle State")]
    [SerializeField] private int castleHealth = 100;
    [SerializeField] private int currentCastleHealth;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool isBattleActive = false;
    private int totalEnemiesSpawned = 0;
    private int totalEnemiesInLevel = 0;
    private List<Coroutine> activeWaveCoroutines = new List<Coroutine>();

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
        totalEnemiesSpawned = 0;
        StartAllWaves();
    }

    private void StartAllWaves()
    {
        foreach (var lane in levelData.lanes)
        {
            foreach (var wave in lane.waves)
            {
                // Start each wave with its own delay
                Coroutine waveCoroutine = StartCoroutine(SpawnWaveWithDelay(wave, lane.laneZ, wave.startDelay));
                activeWaveCoroutines.Add(waveCoroutine);
            }
        }
    }

    private IEnumerator SpawnWaveWithDelay(WaveData wave, float baseLaneZ, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(SpawnWave(wave, baseLaneZ));
    }

    private IEnumerator SpawnWave(WaveData wave, float baseLaneZ)
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
                    // Randomize Z position within range
                    float randomZ = baseLaneZ + UnityEngine.Random.Range(-zRandomRange, zRandomRange);
                    Vector3 spawnPos = new Vector3(spawnX, 0f, randomZ);
                    enemyObj.transform.position = spawnPos;

                    // Initialize enemy
                    enemy.Initialize(castleTransform);

                    // Subscribe to events
                    enemy.OnEnemyDeath += HandleEnemyDeath;
                    enemy.OnEnemyReachedCastle += HandleEnemyReachedCastle;

                    activeEnemies.Add(enemy);
                    totalEnemiesSpawned++;
                    
                    Debug.Log($"Spawned enemy {totalEnemiesSpawned}/{totalEnemiesInLevel}");
                }
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

        // Check for victory condition
        if (isBattleActive && activeEnemies.Count == 0 && totalEnemiesSpawned >= totalEnemiesInLevel)
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
            StartAllWaves();
        }
    }
} 