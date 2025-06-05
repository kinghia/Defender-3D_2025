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

    public void TakeDamage(float amount, DamageType damageType)
    {
        float finalDamage = CalculateFinalDamage(amount, damageType);

        float remainingDamage = ProcessShieldDamage(finalDamage);

        float healthDamage = ProcessHealthDamage(remainingDamage, damageType);

        // Notify HP change
        InvokeHpChanged(currentHp, GetMaxHp());
    }

    private float ProcessHealthDamage(float damage, DamageType damageType)
    {
        currentHp = Mathf.Max(0, currentHp - damage);

        FloatingTextType fType = FloatingTextType.Default;
        switch (damageType)
        {
            case DamageType.Physical:
                fType = FloatingTextType.Physic;
                break;
            case DamageType.Magic:
                fType = FloatingTextType.Magic;
                break;
            case DamageType.True:
                fType = FloatingTextType.Default;
                break;

        }

        FloatingTextSpawner.Instance.SpawnText(
            damage.ToString("F0"),
            transform.position,
            fType
        );
        return damage;
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

    // Expose HP changed event for UI
    public new event System.Action<float, float> OnHpChanged
    {
        add { base.OnHpChanged += value; }
        remove { base.OnHpChanged -= value; }
    }
}

public enum DamageType
{
    Physical,
    Magic,
    True,
    SharedDamage,
    SelfExplore
}