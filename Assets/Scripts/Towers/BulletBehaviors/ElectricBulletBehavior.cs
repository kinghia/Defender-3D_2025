using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ElectricBulletBehavior : BulletBase
{
    public float bounceRange = 10f;
    private int guaranteedBounces = 1; // 1 bounce always happens after the first target
    public int maxConditionalBounces = 4; // Max additional bounces if target has ElectricityEffect
    private int bouncesRemaining;
    private int conditionalBouncesRemaining;
    private List<Enemy> hitTargets = new List<Enemy>();

    void Start()
    {
        bouncesRemaining = guaranteedBounces;
        conditionalBouncesRemaining = maxConditionalBounces;
    }

    public override void SetTarget(Transform target)
    {
        hitTargets.Add(target.GetComponent<Enemy>());
        this.target = target;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 dir = target.position + Vector3.up * 4 - transform.position;
        float distanceThisFrame = speed * Time.fixedDeltaTime;

        // Hit
        if (dir.magnitude <= distanceThisFrame)
        {
            StatusEffectReceiver statusEffectReceiver = target.GetComponent<StatusEffectReceiver>();

            // Nếu hết lần nảy chính mà target không có ElectricityEffect thì trực tiếp kết thúc
            if (bouncesRemaining <= 0 && !statusEffectReceiver.activeEffects.Any(effect => effect is ElectricityEffect))
            {
                Destroy(gameObject);
            }

            // Nếu hết cả 2 thì kết thúc
            if (bouncesRemaining <= 0 && conditionalBouncesRemaining <= 0)
            {
                Destroy(gameObject);
            }

            hitTargets.Add(target.GetComponent<Enemy>());
            Hit(target);

            if (bouncesRemaining > 0)
            {
                bouncesRemaining--;
                CheckNextTarget();
            }
            else
            {
                if (conditionalBouncesRemaining > 0
                    && statusEffectReceiver != null
                    && statusEffectReceiver.activeEffects.Any(effect => effect is ElectricityEffect))
                {
                    conditionalBouncesRemaining--;
                    CheckNextTarget();
                }
            }
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private void CheckNextTarget()
    {
        Transform target = FindNearestTarget();
        if (target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            this.target = target;
        }
    }

    Transform FindNearestTarget()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        float minDist = float.MaxValue;
        Enemy nearest = null;
        foreach (var enemy in enemies)
        {
            if (hitTargets.Contains(enemy)) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= bounceRange)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        if (nearest == null) return null;
        else return nearest.transform;
    }
}