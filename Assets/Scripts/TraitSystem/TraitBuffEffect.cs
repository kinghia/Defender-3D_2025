using UnityEngine;

public abstract class TraitBuffEffect : ScriptableObject
{
    public string effectName;
    [TextArea]
    public string effectDescription;
    public abstract void ApplyBuff(TowerBase tower);
    public abstract void RemoveBuff(TowerBase tower);
} 