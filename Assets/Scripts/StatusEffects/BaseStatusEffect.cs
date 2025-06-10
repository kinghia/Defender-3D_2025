using UnityEngine;

// Enum for defining how effects of the same type stack
public enum EffectStackingRule
{
    Replace, // Hiệu ứng cũ bị xóa và hiệu ứng mới được áp dụng.
    Extend, // MergeWith của hiệu ứng cũ được gọi để kết hợp với hiệu ứng mới (ví dụ: tăng thời gian tồn tại).
    Ignore // Hiệu ứng mới bị bỏ qua hoàn toàn.
}

public abstract class BaseStatusEffect
{
    protected GameObject holder;
    protected float duration;
    protected float timer;
    public EffectStackingRule StackingRule { get; protected set; }

    public BaseStatusEffect(GameObject holder, float duration)
    {
        this.holder = holder;
        this.duration = duration;
        this.timer = 0f;
        // Default stacking rule, concrete classes should set their own
        this.StackingRule = EffectStackingRule.Replace;
    }

    public virtual void Tick(float deltaTime)
    {
        timer += deltaTime;
    }

    public abstract void Apply();

    public abstract void Remove();

    public bool IsExpired()
    {
        return timer >= duration;
    }

    // Virtual method to handle merging effects, override in concrete classes for Extend rule
    public virtual bool MergeWith(BaseStatusEffect newEffect)
    {
        // By default, effects cannot be merged. Concrete classes must override.
        return false;
    }
} 