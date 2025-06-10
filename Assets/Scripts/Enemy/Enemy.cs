using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyHpBar hpBar;

    private Vector3 targetPosition;
    private bool isAttacking = false;
    private float attackCooldown = 0f;
    private Transform castleTransform;
    [SerializeField] Transform enemyTransform;

    public event Action<Enemy> OnEnemyDeath;
    public event Action<Enemy> OnEnemyReachedCastle;

    void OnEnable()
    {
        if (enemyTransform == null)
        {
            enemyTransform = transform; // Gán giá trị mặc định
        }

        Init();
    }

    public void Init()
    {
        hpBar = GetComponentInChildren<EnemyHpBar>();
        stats = GetComponent<EnemyStats>();
        
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

    public void SetTarget(Vector3 targetPos, Transform castle)
    {
        targetPosition = targetPos;
        castleTransform = castle;
        isAttacking = false;
    }

    void Update()
    {
        enemyTransform.LookAt(castleTransform);

        if (stats == null || stats.IsDead) return;

        if (!isAttacking)
        {
            // Move towards target
            float distanceToCastle = Vector3.Distance(transform.position, castleTransform.position);
            if (distanceToCastle <= stats.GetRange())
            {
                isAttacking = true;
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    stats.GetMoveSpeed() * Time.deltaTime
                );
            }
        }
        else
        {
            // Handle attack
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                AttackCastle();
                attackCooldown = 1f; // Basic attack rate, can be modified by stats
            }
        }
    }

    private void AttackCastle()
    {
        // Trigger attack animation/effect here
        // For now, just notify that enemy reached castle
        OnEnemyReachedCastle?.Invoke(this);
    }

    private void HandleDeath()
    {
        OnEnemyDeath?.Invoke(this);
        EnemyPoolManager.Instance?.ReturnToPool(gameObject);
    }
} 