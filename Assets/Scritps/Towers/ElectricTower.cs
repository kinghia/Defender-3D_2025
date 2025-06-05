using UnityEngine;

public class ElectricTower : TowerBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject hitEffect;

    protected override void AttackTarget()
    {
        if (currentTarget == null) return;
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            ElectricBulletBehavior bullet = bulletObj.GetComponent<ElectricBulletBehavior>();
            if (bullet != null)
            {
                bullet.SetTarget(currentTarget.transform);
                bullet.OnHit += HandleBulletHit;
            }
        }
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

        ElectricityEffect electricityEffect = new ElectricityEffect(hitTarget.gameObject, 5, 1, 0.2f, 1f);
        StatusEffectReceiver statusEffectReceiver = hitTarget.GetComponent<StatusEffectReceiver>();
        if (statusEffectReceiver != null) {
            statusEffectReceiver.AddEffect(electricityEffect);
        }

    }

    private void ShowHitEffect(Transform hitTarget)
    {
        if (hitEffect == null) return;
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