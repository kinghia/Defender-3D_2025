using UnityEngine;
using System;


public abstract class SkillBase : MonoBehaviour
{
    [Header("Basic Info")]
    public string skillName;
    public string description;
    public Sprite icon;

    [Header("Visual Effects")]
    public GameObject skillEffectPrefab;
    public AudioClip skillSound;

    protected TowerBase ownerTower;
    public event Action OnSkillActivated;
    public event Action OnSkillFailed;
    private TowerStats towerStats;

    protected virtual void Awake()
    {
        ownerTower = GetComponent<TowerBase>();
        if (ownerTower == null)
        {
            Debug.LogError($"Skill {skillName} requires a TowerBase component!");
            return;
        }

        towerStats = GetComponent<TowerStats>();
        if (towerStats != null)
        {
            towerStats.OnManaFull += TryActivateSkill;
        }
    }

    protected virtual bool CheckAdditionalConditions()
    {
        // Override this in child classes to add specific conditions
        return true;
    }

    public virtual void TryActivateSkill()
    {
        if (!CheckAdditionalConditions())
        {
            OnSkillFailed?.Invoke();
            return;
        }

        OnSkillActivated?.Invoke();

        ExecuteSkill();
    }

    protected abstract void ExecuteSkill();

    protected void PlaySkillEffects(Vector3 position)
    {
        if (skillEffectPrefab != null)
        {
            Instantiate(skillEffectPrefab, position, Quaternion.identity);
        }

        if (skillSound != null)
        {
            AudioSource.PlayClipAtPoint(skillSound, position);
        }
    }

    void OnDestroy()
    {
        if (towerStats != null)
        {
            towerStats.OnManaFull -= TryActivateSkill;
        }
    }
}
