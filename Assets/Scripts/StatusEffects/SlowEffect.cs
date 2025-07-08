using UnityEngine;

public class SlowEffect : BaseStatusEffect
{
    private float slowPercent;

    public SlowEffect(float duration, float slowPercent) : base(duration)
    {
        this.slowPercent = slowPercent;
        this.StackingRule = EffectStackingRule.Extend; // Replace existing slow effect
    }

    public override void Apply(GameObject holder)
    {
        base.Apply(holder);
        if (holder.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            // Apply slow by reducing moveSpeed
            enemyStats.ModifyStat(StatType.MoveSpeed, 0, -slowPercent);
        }
    }

    public override void Remove()
    {
        if (isRemoved) return;
        base.Remove();
        if (holder.TryGetComponent<EnemyStats>(out EnemyStats enemyStats))
        {
            // Remove slow effect
            enemyStats.ModifyStat(StatType.MoveSpeed, 0, slowPercent);
        }
    }

    public override void MergeWith(BaseStatusEffect newEffect)
    {
        if (newEffect is SlowEffect newSlow)
        {
            // If new slow is stronger, replace the old one
            if (newSlow.slowPercent > this.slowPercent)
            {
                Remove(); // Remove current slow
                this.slowPercent = newSlow.slowPercent;
                Apply(holder); // Apply new slow
            }

            timer = 0f;
        }
    }
}