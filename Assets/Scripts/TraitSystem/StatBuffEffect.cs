using UnityEngine;

[CreateAssetMenu(fileName = "NewStatBuffEffect", menuName = "Game/TraitBuff/StatBuffEffect")]
public class StatBuffEffect : TraitBuffEffect
{
    public StatType statType;
    public float flatBonus;
    public float percentBonus;

    public override void ApplyBuff(TowerBase tower)
    {
        var stats = tower.GetComponent<BaseStats>();
        if (stats != null)
        {
            stats.ModifyStat(statType, flatBonus, percentBonus);
        }
    }

    public override void RemoveBuff(TowerBase tower)
    {
        var stats = tower.GetComponent<BaseStats>();
        if (stats != null)
        {
            stats.ModifyStat(statType, -flatBonus, -percentBonus);
        }
    }
} 