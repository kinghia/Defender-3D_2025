using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyHpBar hpBar;
    private EnemyMover mover;
    private bool isAttacking = false;
    private float attackCooldown = 0f;
    private Transform castleTransform;

    public event Action<Enemy> OnEnemyDeath;
    public event Action<Enemy> OnEnemyReachedCastle;

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        hpBar = GetComponentInChildren<EnemyHpBar>();
        stats = GetComponent<EnemyStats>();
        mover = GetComponent<EnemyMover>();
        
        if (stats == null)
        {
            Debug.LogError($"EnemyStats component not found on {gameObject.name}");
            return;
        }
        
        if (hpBar == null)
        {
            Debug.LogError($"EnemyHpBar component not found on {gameObject.name}");
            return;
        }

        if (mover == null)
        {
            Debug.LogError($"EnemyMover component not found on {gameObject.name}");
            return;
        }

        stats.Initialize();
        hpBar.Init(stats);
        stats.onDeath += HandleDeath;
    }

    void OnDisable()
    {
        if (stats != null)
        {
            stats.onDeath -= HandleDeath;
        }
    }

    public void Initialize(Transform castle)
    {
        castleTransform = castle;
        if (mover != null && stats != null)
        {
            mover.Initialize(castle, stats.GetRange());
        }
    }

    void Update()
    {
        if (stats == null || stats.IsDead) return;

        if (isAttacking)
        {   
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                AttackCastle();
                attackCooldown = 1f; // Basic attack rate, can be modified by stats
            }
        }
    }

    public void StartAttacking()
    {
        isAttacking = true;
        OnEnemyReachedCastle?.Invoke(this);
    }

    private void AttackCastle()
    {
        // Trigger attack animation/effect here
        // For now, just notify that enemy is attacking castle
        OnEnemyReachedCastle?.Invoke(this);
    }

    private void HandleDeath()
    {
        OnEnemyDeath?.Invoke(this);
        EnemyPoolManager.Instance?.ReturnToPool(gameObject);
    }
} 