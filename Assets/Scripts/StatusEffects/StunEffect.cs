using UnityEngine;

public class StunEffect : BaseStatusEffect
{
    public StunEffect(float duration) : base(duration)
    {
        // Stun effects usually replace or ignore, rarely extend in duration by merging.
        // Let's set it to Replace by default, or maybe Ignore if multiple stuns hit at once.
        // For this implementation, let's assume Replace is the most common scenario.
        this.StackingRule = EffectStackingRule.Replace;
    }

    public override void Apply(GameObject holder)
    {
        base.Apply(holder);

        if (holder.TryGetComponent<EnemyMover>(out EnemyMover mover))
        {
            FloatingTextSpawner.Instance.SpawnText(
                "Stunned!",
                holder.transform.position + Vector3.up * 2,
                Color.white
            );

            mover.PauseMovement();
        }
    }

    public override void Remove()
    {
        if (isRemoved) return;
        base.Remove();
        if (holder.TryGetComponent<EnemyMover>(out EnemyMover mover))
        {
            mover.ResumeMovement();
        }
    }

    // For simplicity, StunEffect might not support merging, so no need to override MergeWith if StackingRule is Replace or Ignore.
    // If StackingRule were Extend, we would override MergeWith to add duration, etc.
}