using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerTrait", menuName = "Game/TowerTrait")]
public class TowerTraitData : ScriptableObject
{
    public string traitName;
    [TextArea]
    public string description;
    public Sprite icon;
    public List<TraitTier> tiers = new List<TraitTier>();
}

[System.Serializable]
public class StatBuffConfig
{
    public StatType statType;
    public float flatBonus;
    
    [Tooltip("Enter percentage value (e.g. 30 for 30%)")]
    [Range(0, 1000)]
    public float percentBonus;

    // Convert percentage to decimal (e.g. 30% -> 0.3)
    public float GetPercentBonusDecimal()
    {
        return percentBonus / 100f;
    }

    // For display purposes
    public string GetPercentBonusDisplay()
    {
        return $"{percentBonus}%";
    }
}

[System.Serializable]
public class TraitTier
{
    public int requiredCount;
    [TextArea]
    public string tierDescription;
    
    [Header("Stat Buffs")]
    public List<StatBuffConfig> statBuffs = new List<StatBuffConfig>();
    
    [Header("Special Effects")]
    public List<TraitBuffEffect> specialBuffs = new List<TraitBuffEffect>();
} 