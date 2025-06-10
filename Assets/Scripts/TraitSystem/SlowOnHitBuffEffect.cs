using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SlowOnHitBuff", menuName = "Tower/Traits/SlowOnHitBuff")]
public class SlowOnHitBuffEffect : TraitBuffEffect
{
    [Header("Slow Effect Settings")]
    public float slowPercent = 30f; // Default 30% slow
    public float slowDuration = 2f; // Default 2 seconds

    public override void ApplyBuff(TowerBase tower)
    {
        Debug.Log($"Applying SlowOnHitBuff to tower: {tower.name}");
        // Subscribe to tower's OnHit event
        tower.OnHit += HandleTowerHit;
    }

    public override void RemoveBuff(TowerBase tower)
    {
        // Unsubscribe from tower's OnHit event
        tower.OnHit -= HandleTowerHit;
    }

    private void HandleTowerHit(Transform hitTarget)
    {
        if (hitTarget == null) return;

        // Get or add StatusEffectReceiver
        StatusEffectReceiver statusEffectReceiver = hitTarget.GetComponent<StatusEffectReceiver>();
        if (statusEffectReceiver == null)
        {
            statusEffectReceiver = hitTarget.gameObject.AddComponent<StatusEffectReceiver>();
        }

        // Create and apply slow effect
        SlowEffect slowEffect = new SlowEffect(hitTarget.gameObject, slowDuration, slowPercent/100f);
        statusEffectReceiver.AddEffect(slowEffect);
    }
} 