using UnityEngine;

public class EnemyStats : BaseStats
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;
    public bool IsDead => currentHp <= 0;
    public float CurrentHP => currentHp;
    public float CurrentHealthPercent => currentHp / GetMaxHp();
    public System.Action onDeath;

    public void Initialize(EnemyData emData = null)
    {
        if (emData != null)
        {
            data = emData;
        }

        if (data == null)
        {
            Debug.LogError("Enemy Stats missing data");
        }

        currentHp = data.maxHp;
        currentShield = 0;
        ResetModifiers();
    }


    public void TakeDamage(float amount, DamageType damageType, bool canCrit = false, BaseStats attacker = null)
    {
        float finalDamage = amount;
        bool isCritical = false;

        // Apply critical hit if enabled and successful
        if (canCrit && attacker != null)
        {
            // Use attacker's critical stats instead of enemy's own stats
            if (attacker is TowerStats towerStats)
            {
                if (towerStats.RollForCritical())
                {
                    finalDamage = towerStats.CalculateCriticalDamage(amount);
                    isCritical = true;
                    Debug.Log($"CRITICAL HIT! Base damage: {amount}, Critical damage: {finalDamage}");
                }
            }
        }

        finalDamage = CalculateFinalDamage(finalDamage, damageType);

        float remainingDamage = ProcessShieldDamage(finalDamage);

        float healthDamage = ProcessHealthDamage(remainingDamage, damageType, isCritical);

        // Notify HP change
        InvokeHpChanged(currentHp, GetMaxHp());
    }

    private float ProcessHealthDamage(float damage, DamageType damageType, bool isCritical = false)
    {
        currentHp = Mathf.Max(0, currentHp - damage);

        Color color = Color.white;
        int size = 14;
        switch (damageType)
        {
            case DamageType.Physical:
                color = Color.red;
                break;
            case DamageType.Magic:
                color = new Color(111, 0, 208);
                break;
            case DamageType.True:
                size = 12;
                break;
        }

        FloatingTextSpawner.Instance.SpawnDamage(
            damage.ToString("F0"),
            transform.position + Vector3.up * 5,
            color,
            size,
            isCritical
        );

        if (currentHp <= 0)
        {
            onDeath?.Invoke();
        }

        return damage;
    }
    public void SelfDead()
    {
        currentHp = 0;
        onDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        float healing = StatsCalculator.CalculateHealing(amount, this);
        currentHp = Mathf.Min(currentHp + healing, GetMaxHp());
        // Notify HP change
        InvokeHpChanged(currentHp, GetMaxHp());
    }

    public void SetCurrentHp(float hp)
    {
        currentHp = hp;
        // Notify HP change
        InvokeHpChanged(currentHp, GetMaxHp());
    }

    public bool RollForCritical()
    {
        return Random.value < GetCriticalChance() / 100f;
    }

    public float CalculateCriticalDamage(float damage)
    {
        return damage * GetCriticalDamage() / 100f;
    }


    // IStats Implementation
    public override float GetMaxHp() => GetModifiedStat(StatType.MaxHp, data.maxHp);
    public override float GetCurrentHp() => currentHp;
    public override float GetArmor() => GetModifiedStat(StatType.Armor, data.armor);
    public override float GetMagicResist() => GetModifiedStat(StatType.MagicResist, data.magicResist);
    public override float GetMoveSpeed() => GetModifiedStat(StatType.MoveSpeed, data.moveSpeed);
    public override int GetRange() => (int)GetModifiedStat(StatType.Range, data.range);
    public override float GetDamageReduction() => Mathf.Min(100, GetModifiedPercentStat(StatType.DamageReduction, data.damageReduction));
    public override float GetHealingReceived() => Mathf.Max(0, GetModifiedPercentStat(StatType.HealingReceived, data.healingReceivedPercent));
    public override float GetPhysicalDamage() => GetModifiedStat(StatType.PhysicalDamage, data.physicalDamage);
    public override float GetMagicDamage() => GetModifiedStat(StatType.MagicDamage, data.magicDamage);

    // Additional Unit specific stats
    public float GetCriticalChance() => data.criticalChance;
    public float GetCriticalDamage() => data.criticalDamage;
    public float GetArmorPenetration() => data.armorPenetration;
    public float GetMagicPenetration() => data.magicPenetration;
    public float GetDamageAmplification() => data.damageAmplification;
    public float GetTenacity() => data.tenacity;
    public float GetHPRegen() => data.hpRegen;
    public int GetDetectRange() => data.detectRange;

}

public enum DamageType
{
    Physical,
    Magic,
    True,
    SharedDamage,
    SelfExplore
}