using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Game/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public Sprite image;
    [TextArea]
    public string description;

    [Header("Offensive Stats")]
    [Range(0f, 500f)]
    public float physicalDamage;     // Sát thương vật lý

    [Range(0f, 500f)]
    public float magicDamage;        // Sát thương phép

    [Range(0f, 100f)]
    public float criticalChance;     // % Tỉ lệ chí mạng

    [Range(150f, 500f)]
    public float criticalDamage;     // % Tăng sát thương khi chí mạng

    [Header("Utility")]
    [Range(0f, 5f)]
    public float attackSpeed;        // Tốc độ đánh

    [Range(0, 100)]
    public int range;                // Tầm đánh

    [Range(0f, 1000f)]
    public float maxMana;            // Mana tối đa

    private void OnValidate()
    {
        // Giới hạn các giá trị
        physicalDamage = Mathf.Max(0, physicalDamage);
        magicDamage = Mathf.Max(0, magicDamage);
        criticalChance = Mathf.Clamp(criticalChance, 0, 100);
        criticalDamage = Mathf.Max(150f, criticalDamage);
        attackSpeed = Mathf.Max(0.1f, attackSpeed);
        range = Mathf.Max(0, range);
        maxMana = Mathf.Max(0, maxMana);
    }
} 