using UnityEngine;
using System.Collections.Generic;

public class ArrowTower : TowerBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject hitEffect;

    protected override void AttackTarget()
    {
        List<Enemy> targets = FindTargetsInRange();
        if (targets.Count == 0 || bulletPrefab == null || firePoint == null) return;

        float chance = 0.5f; // 50% cho tia đầu tiên
        int arrowCount = 1;
        while (Random.value < chance && arrowCount < targets.Count)
        {
            arrowCount++;
            chance = Mathf.Max(chance - 0.1f, 0.2f); // giảm 10%, không dưới 20%
        }

        // Chọn ngẫu nhiên các mục tiêu khác nhau
        List<Enemy> chosenTargets = PickRandomTargets(targets, arrowCount);
        foreach (var target in chosenTargets)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            ArrowBehavior arrow = bulletObj.GetComponent<ArrowBehavior>();
            if (arrow != null)
            {
                arrow.SetTarget(target.transform);
                arrow.OnHit += HandleBulletHit;
            }
        }
    }

    private List<Enemy> FindTargetsInRange()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        List<Enemy> targets = new List<Enemy>();
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= stats.GetRange())
            {
                targets.Add(enemy);
            }
        }
        return targets;
    }

    private List<Enemy> PickRandomTargets(List<Enemy> targets, int count)
    {
        List<Enemy> chosen = new List<Enemy>();
        List<Enemy> pool = new List<Enemy>(targets);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            chosen.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return chosen;
    }

    protected override void HandleBulletHit(Transform hitTarget)
    {
        if (hitTarget == null) return;
        base.HandleBulletHit(hitTarget);
        ShowHitEffect(hitTarget);
        EnemyStats emStats = hitTarget.GetComponent<EnemyStats>();
        if (emStats != null)
        {
            emStats.TakeDamage(stats.GetPhysicalDamage(), DamageType.Physical);
        }
    }

    private void ShowHitEffect(Transform hitTarget)
    {
        var aoe = Instantiate(hitEffect, hitTarget.position + Vector3.up, Quaternion.identity);
        ParticleSystem ps = aoe.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(aoe, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(aoe, 0.5f);
        }
    }
} 