using System;
using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Tower Data")]
    [SerializeField] protected TowerData towerData;

    [Header("Traits")]
    public TowerTraitData[] traits;

    [Header("Skill")]
    [SerializeField] protected SkillBase skill;

    protected Enemy currentTarget;
    protected TowerStats stats;
    protected float attackCooldown = 0f;
    public event Action<Transform> OnHit;

    protected virtual void Awake()
    {
        stats = GetComponent<TowerStats>();
        if (stats == null)
        {
            Debug.LogError($"Tower {gameObject.name} is missing TowerStats component!");
            return;
        }
        stats.Initialize(towerData);
    }

    protected virtual void OnEnable()
    {
        TraitManager.Instance?.RegisterTower(this);
    }

    protected virtual void OnDisable()
    {
        TraitManager.Instance?.UnregisterTower(this);
    }

    protected virtual void FixedUpdate()
    {
        if (stats == null) return;

        attackCooldown -= Time.fixedDeltaTime;
        if (currentTarget == null || !IsTargetInRange(currentTarget))
        {
            FindNearestTarget();
        }
        if (currentTarget != null && attackCooldown <= 0f)
        {
            if (CanAttack())
            {
                AttackTarget();
                attackCooldown = 1f / stats.GetAttackSpeed();
            }
        }
    }

    protected virtual void HandleBulletHit(Transform hitTarget)
    {
        OnHit?.Invoke(hitTarget);
    }

    protected virtual bool CanAttack()
    {
        // Override this in child classes if they need mana or other conditions
        return true;
    }

    protected void FindNearestTarget()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        float minDist = float.MaxValue;
        Enemy nearest = null;
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= stats.GetRange())
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        currentTarget = nearest;
    }

    protected bool IsTargetInRange(Enemy enemy)
    {
        if (enemy == null || stats == null) return false;
        return Vector3.Distance(transform.position, enemy.transform.position) <= stats.GetRange();
    }

    protected abstract void AttackTarget();

    public SkillBase GetSkill()
    {
        return skill;
    }
}