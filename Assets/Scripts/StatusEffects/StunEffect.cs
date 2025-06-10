using UnityEngine;

public class StunEffect : BaseStatusEffect
{
    public StunEffect(GameObject holder, float duration) : base(holder, duration)
    {
        // Stun effects usually replace or ignore, rarely extend in duration by merging.
        // Let's set it to Replace by default, or maybe Ignore if multiple stuns hit at once.
        // For this implementation, let's assume Replace is the most common scenario.
        this.StackingRule = EffectStackingRule.Replace;
    }

    public override void Apply()
    {
        FloatingTextSpawner.Instance.SpawnText(
           "Stunning",
           holder.transform.position + Vector3.up * 2,
           FloatingTextType.Default
        );

        if (holder.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.GetComponent<EnemyMover>().enabled = false;
            return;
        }
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime); // Update the main effect timer
        // Stun typically doesn't need per-tick logic other than timing out, handled by base class
    }

    public override void Remove()
    {
        if (holder.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.GetComponent<EnemyMover>().enabled = true;
            return;
        }
    }

    // For simplicity, StunEffect might not support merging, so no need to override MergeWith if StackingRule is Replace or Ignore.
    // If StackingRule were Extend, we would override MergeWith to add duration, etc.
}