using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance { get; private set; }

    public List<TowerBase> activeTowers = new List<TowerBase>();
    private Dictionary<TowerTraitData, int> traitCounts = new Dictionary<TowerTraitData, int>();
    private Dictionary<TowerBase, List<TraitBuffEffect>> appliedSpecialBuffs = new Dictionary<TowerBase, List<TraitBuffEffect>>();
    private Dictionary<TowerBase, List<StatBuffConfig>> appliedStatBuffs = new Dictionary<TowerBase, List<StatBuffConfig>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterTower(TowerBase tower)
    {
        if (!activeTowers.Contains(tower))
        {
            activeTowers.Add(tower);
            UpdateTraits();
        }
    }

    public void UnregisterTower(TowerBase tower)
    {
        if (activeTowers.Contains(tower))
        {
            RemoveBuffs(tower);
            activeTowers.Remove(tower);
            UpdateTraits();
        }
    }

    private void UpdateTraits()
    {
        traitCounts.Clear();
        foreach (var tower in activeTowers)
        {
            foreach (var trait in tower.traits)
            {
                if (!traitCounts.ContainsKey(trait))
                    traitCounts[trait] = 0;
                traitCounts[trait]++;
            }
        }
        // Remove all buffs and re-apply
        foreach (var tower in activeTowers)
        {
            RemoveBuffs(tower);
            ApplyBuffs(tower);
        }
    }

    private void ApplyBuffs(TowerBase tower)
    {
        if (!appliedSpecialBuffs.ContainsKey(tower))
            appliedSpecialBuffs[tower] = new List<TraitBuffEffect>();
        if (!appliedStatBuffs.ContainsKey(tower))
            appliedStatBuffs[tower] = new List<StatBuffConfig>();

        foreach (var trait in tower.traits)
        {
            if (!traitCounts.ContainsKey(trait)) continue;
            int count = traitCounts[trait];
            TraitTier bestTier = null;
            foreach (var tier in trait.tiers)
            {
                if (count >= tier.requiredCount)
                    bestTier = tier;
            }
            if (bestTier != null)
            {
                // Apply stat buffs
                foreach (var statBuff in bestTier.statBuffs)
                {
                    var stats = tower.GetComponent<BaseStats>();
                    if (stats != null)
                    {
                        stats.ModifyStat(statBuff.statType, statBuff.flatBonus, statBuff.GetPercentBonusDecimal());
                        appliedStatBuffs[tower].Add(statBuff);
                        Debug.Log($"Applied stat buff to {tower.name}: {statBuff.statType} +{statBuff.flatBonus} {statBuff.GetPercentBonusDisplay()}");
                    }
                }

                // Apply special buffs
                foreach (var specialBuff in bestTier.specialBuffs)
                {
                    specialBuff.ApplyBuff(tower);
                    appliedSpecialBuffs[tower].Add(specialBuff);
                    Debug.Log($"Applied special buff to {tower.name}: {specialBuff.effectName}");
                }
            }
        }
    }

    private void RemoveBuffs(TowerBase tower)
    {
        
        // Remove stat buffs
        if (appliedStatBuffs.ContainsKey(tower))
        {
            var stats = tower.GetComponent<BaseStats>();
            if (stats != null)
            {
                foreach (var statBuff in appliedStatBuffs[tower])
                {
                    stats.ModifyStat(statBuff.statType, -statBuff.flatBonus, -statBuff.GetPercentBonusDecimal());
                    Debug.Log($"Removed stat buff from {tower.name}: {statBuff.statType} -{statBuff.flatBonus} -{statBuff.GetPercentBonusDisplay()}");
                }
            }
            else
            {
                Debug.LogWarning($"Tower {tower.name} missing BaseStats component when removing buffs");
            }
            appliedStatBuffs[tower].Clear();
        }

        // Remove special buffs
        if (appliedSpecialBuffs.ContainsKey(tower))
        {
            Debug.Log($"Found {appliedSpecialBuffs[tower].Count} special buffs to remove");
            foreach (var specialBuff in appliedSpecialBuffs[tower])
            {
                specialBuff.RemoveBuff(tower);
                Debug.Log($"Removed special buff from {tower.name}: {specialBuff.effectName}");
            }
            appliedSpecialBuffs[tower].Clear();
        }
    }
} 