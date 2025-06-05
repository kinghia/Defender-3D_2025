using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyStats))]
public class EnemyHpBar : MonoBehaviour
{
    [Header("References")]
    public Image hpBar;
    public Image delayHpBar;
    public Image shieldBar;

    [Header("Animation")]
    public float delaySpeed = 2f;

    private EnemyStats stats;
    private float targetHpPercent;
    private float delayHpPercent;
    private float maxHp;
    private float lastShield;

    public void Init(EnemyStats enemyStats)
    {
        stats = enemyStats;
        maxHp = stats.GetMaxHp();
        targetHpPercent = stats.CurrentHP / maxHp;
        delayHpPercent = targetHpPercent;
        lastShield = stats.GetTotalShield();

        stats.OnHpChanged += OnHpChanged;
        stats.OnShieldChanged += OnShieldChanged;

        UpdateHpBar(targetHpPercent);
        UpdateShieldBar(stats.GetTotalShield());
    }

    void OnDestroy()
    {
        if (stats != null)
        {
            stats.OnHpChanged -= OnHpChanged;
            stats.OnShieldChanged -= OnShieldChanged;
        }
    }

    void Update()
    {
        // Smoothly animate the delayed HP bar
        if (delayHpBar != null && delayHpPercent > targetHpPercent)
        {
            delayHpPercent = Mathf.MoveTowards(delayHpPercent, targetHpPercent, delaySpeed * Time.deltaTime);
            delayHpBar.fillAmount = delayHpPercent;
        }
    }

    void OnHpChanged(float current, float max)
    {
        targetHpPercent = Mathf.Clamp01(current / max);
        if (hpBar != null)
            hpBar.fillAmount = targetHpPercent;
    }

    void OnShieldChanged(float shield)
    {
        UpdateShieldBar(shield);
    }

    void UpdateHpBar(float percent)
    {
        if (hpBar != null)
            hpBar.fillAmount = percent;
        if (delayHpBar != null)
            delayHpBar.fillAmount = percent;
    }

    void UpdateShieldBar(float shield)
    {
        if (shieldBar != null)
        {
            shieldBar.gameObject.SetActive(shield > 0);
            shieldBar.fillAmount = Mathf.Clamp01(shield / maxHp);
        }
    }
}