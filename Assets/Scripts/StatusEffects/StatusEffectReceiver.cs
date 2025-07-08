using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StatusEffectReceiver : MonoBehaviour
{
    // Changed from private to public to allow other scripts (like ElectricBulletBehavior) to check active effects
    public List<BaseStatusEffect> activeEffects = new List<BaseStatusEffect>();

    void Start()
    {
        if (TryGetComponent<EnemyStats>(out var stats))
        {
            stats.onDeath += Reset;
        }
    }

    public void AddEffect(BaseStatusEffect effect)
    {
        // Check if an effect of the same type already exists
        BaseStatusEffect existingEffect = activeEffects.FirstOrDefault(e => e.GetType() == effect.GetType());

        if (existingEffect != null)
        {
            switch (effect.StackingRule)
            {
                case EffectStackingRule.Replace:
                    // Remove existing effect and add the new one
                    existingEffect.Remove();
                    activeEffects.Remove(existingEffect);
                    activeEffects.Add(effect);
                    effect.Apply(gameObject);
                    break;
                case EffectStackingRule.Extend:
                    existingEffect.MergeWith(effect);
                    break;
                case EffectStackingRule.Ignore:
                    break;
            }
        }
        else
        {
            // No existing effect of this type, just add the new one
            activeEffects.Add(effect);
            effect.Apply(gameObject);
        }
    }

    public bool HasEffect<T>() where T : BaseStatusEffect
    {
        return activeEffects.Any(effect => effect is T);
    }

    public T GetEffect<T>() where T : BaseStatusEffect
    {
        return activeEffects.FirstOrDefault(effect => effect is T) as T;
    }

    public void Reset()
    {
        // Remove all active effects
        foreach (var effect in activeEffects)
        {
            effect.Remove();
        }
        activeEffects.Clear();
    }

    void Update()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            BaseStatusEffect effect = activeEffects[i];
            effect.Tick(Time.deltaTime);

            if (effect.IsExpired())
            {
                effect.Remove();
                activeEffects.RemoveAt(i);
            }
        }
    }

    void OnDestroy()
    {
        if (TryGetComponent<EnemyStats>(out var stats))
        {
            stats.onDeath -= Reset;
        }
    }

}