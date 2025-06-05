using UnityEngine;

public class SlowEffect : BaseStatusEffect
{
    private float slowPercent;

    public SlowEffect(GameObject holder, float duration, float slowPercent) : base(holder, duration)
    {
        this.slowPercent = slowPercent;
        this.StackingRule = EffectStackingRule.Replace; // Replace existing slow effect
    }

    public override void Apply()
    {
        if (holder.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            // Apply slow by reducing moveSpeed
            enemyStats.ModifyStat(StatType.MoveSpeed, 0, -slowPercent);
        }
    }

    public override void Remove()
    {
        if (holder.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            // Remove slow effect
            enemyStats.ModifyStat(StatType.MoveSpeed, 0, slowPercent);
        }
    }

    public override bool MergeWith(BaseStatusEffect newEffect)
    {
        if (newEffect is SlowEffect newSlow)
        {
            // If new slow is stronger, replace the old one
            if (newSlow.slowPercent > this.slowPercent)
            {
                Remove(); // Remove current slow
                this.slowPercent = newSlow.slowPercent;
                Apply(); // Apply new slow
            }
            return true;
        }
        return false;
    }
} 