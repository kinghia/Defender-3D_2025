using System;
using UnityEngine;

public class TowerStats : BaseStats
{
    [SerializeField] private TowerData data;
    public TowerData Data => data;
    public ManaType manaType = ManaType.Medium;
    public float CurrentMana => currentMana;

    // 
    private SkillBase skill;
    private float currentMana;
    private float manaRegen = 10f;
    private float skillReuseTimer = 1f;

    // PUBLIC EVENT
    public event Action OnManaFull;

    // CONST
    private const float MAX_MANA = 100f;
    private const float REUSE_SKILL_FAIL = 1f; // IF skill fail by additional condition, use it again after {REUSE_SKILL_FAIL}

    void Start()
    {
        skill = GetComponent<SkillBase>();
        if (skill != null)
        {
            skill.OnSkillActivated += () =>
            {
                currentMana = 0;
            };

            skill.OnSkillFailed += () =>
            {
                skillReuseTimer = 0f;
            };
        }
    }

    void FixedUpdate()
    {
        skillReuseTimer = Mathf.Min(skillReuseTimer + 1f * Time.fixedDeltaTime, REUSE_SKILL_FAIL);
        
        // Regenerate mana
        if (currentMana < MAX_MANA)
        {
            currentMana = Mathf.Min(currentMana + manaRegen * Time.fixedDeltaTime, MAX_MANA);
        }

        if (currentMana >= MAX_MANA && skillReuseTimer >= REUSE_SKILL_FAIL)
        {
            OnManaFull?.Invoke();
        }
    }

    public void Initialize(TowerData towerData = null)
    {
        if (towerData != null)
        {
            data = towerData;
        }

        if (data == null)
        {
            Debug.LogError("Tower Stats missing data");
            return;
        }

        ResetModifiers();
    }

    public bool RollForCritical()
    {
        return UnityEngine.Random.value < GetCriticalChance() / 100f;
    }

    public float CalculateCriticalDamage(float damage)
    {
        return damage * GetCriticalDamage() / 100f;
    }

    // IStats Implementation
    public override float GetMaxHp() => 0; // Tower không có máu
    public override float GetCurrentHp() => 0;
    public override float GetArmor() => 0;
    public override float GetMagicResist() => 0;
    public override float GetMoveSpeed() => 0;
    public override int GetRange() => (int)GetModifiedStat(StatType.Range, data.range);
    public override float GetDamageReduction() => 0;
    public override float GetHealingReceived() => 0;
    public override float GetPhysicalDamage() => GetModifiedStat(StatType.PhysicalDamage, data.physicalDamage);
    public override float GetMagicDamage() => GetModifiedStat(StatType.MagicDamage, data.magicDamage);

    // Additional Tower specific stats
    public float GetCriticalChance() => data.criticalChance;
    public float GetCriticalDamage() => data.criticalDamage;
    public float GetAttackSpeed() => GetModifiedStat(StatType.AttackSpeed, data.attackSpeed);

    // Events
    public event System.Action<float, float> OnManaChanged; // (currentMana, maxMana)
}

public enum ManaType
{
    VeryFast,
    Fast,
    Medium,
    Slow,
    VerySlow
}