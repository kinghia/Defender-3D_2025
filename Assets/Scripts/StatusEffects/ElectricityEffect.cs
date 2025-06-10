using UnityEngine;
using System;

public class ElectricityEffect : BaseStatusEffect
{
    private int currentStacks;
    private const int MAX_STACKS = 10;
    private float damagePerStackPerSecond;
    private float stunChance;
    private float stunDuration;

    private float damageTickTimer; // Timer for applying damage per second
    private const float DAMAGE_TICK_INTERVAL = 0.5f; // Apply damage every 1 second

    public ElectricityEffect(GameObject holder, float duration, float damagePerStackPerSecond, float stunChance, float stunDuration) : base(holder, duration)
    {
        this.currentStacks = 1; // Start with 1 stack when applied initially
        this.damagePerStackPerSecond = damagePerStackPerSecond;
        this.stunChance = stunChance;
        this.stunDuration = stunDuration;
        this.StackingRule = EffectStackingRule.Extend; // Stacks should extend the existing effect
        this.damageTickTimer = 0f;
    }

    public override void Apply()
    {
        CheckForMaxStacks(); // Check immediately on apply in case 10 stacks are applied at once
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime); // Update the main effect timer

        damageTickTimer += deltaTime;
        if (damageTickTimer >= DAMAGE_TICK_INTERVAL)
        {
            // Apply damage based on current stacks
            float totalDamage = currentStacks * damagePerStackPerSecond;
            // Assuming target has a method like TakeDamage
            TryDealDamageToHolder(totalDamage);
            damageTickTimer -= DAMAGE_TICK_INTERVAL; // Decrease by interval
        }
    }

    private void TryDealDamageToHolder(float damage)
    {
        if (holder.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.GetComponent<EnemyStats>().TakeDamage(damage, DamageType.True);
        }
    }

    public override void Remove()
    {
    }

    public override bool MergeWith(BaseStatusEffect newEffect)
    {
        if (newEffect is ElectricityEffect)
        {
            // Increase stacks, cap at MAX_STACKS
            currentStacks = Mathf.Min(currentStacks + 1, MAX_STACKS);

            // Làm mới thời gian hiệu ứng
            timer = 0f;

            CheckForMaxStacks();

            return true; // Indicate successful merge
        }
        return false; // Indicate cannot merge with this type of effect
    }

    private void CheckForMaxStacks()
    {
        if (currentStacks >= MAX_STACKS)
        {

            // Stun chance
            if (UnityEngine.Random.value < stunChance)
            {
                // Assuming target has a StatusEffectReceiver
                StatusEffectReceiver receiver = holder.GetComponent<StatusEffectReceiver>();
                if (receiver != null)
                {
                    // Create and add the StunEffect
                    StunEffect stun = new StunEffect(holder, stunDuration);
                    receiver.AddEffect(stun); // Add effect via receiver to handle its lifecycle
                }
            }

            // Reset stacks after triggering effect
            currentStacks = 0;
        }
    }
}