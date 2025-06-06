using UnityEngine;

public enum FloatingTextType { Default, Physic, Magic, Gold, Crit }

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [Header("References")]
    public FloatingTextPool pool;

    [Header("Font Sizes")]
    public float defaultFontSize = 36f;
    public float physicFontSize = 40f;
    public float magicFontSize = 40f;
    public float goldFontSize = 32f;
    public float critFontSize = 48f;

    [Header("Colors")]
    public Color defaultColor = Color.white;
    public Color physicColor = Color.red;
    public Color magicColor = Color.cyan;
    public Color goldColor = Color.yellow;
    public Color critColor = new Color(1f, 0.5f, 0f);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnText(string content, Vector3 worldPosition, FloatingTextType type = FloatingTextType.Default)
    {
        var floatingText = pool.Get();
        Color useColor = GetColorByType(type);
        float useFontSize = GetFontSizeByType(type);
        floatingText.Init(content, worldPosition, useColor, useFontSize, OnTextDespawned);
    }

    private void OnTextDespawned(FloatingText text)
    {
        pool.ReturnToPool(text);
    }

    private Color GetColorByType(FloatingTextType type)
    {
        switch (type)
        {
            case FloatingTextType.Physic: return physicColor;
            case FloatingTextType.Magic: return magicColor;
            case FloatingTextType.Gold: return goldColor;
            case FloatingTextType.Crit: return critColor;
            default: return defaultColor;
        }
    }

    private float GetFontSizeByType(FloatingTextType type)
    {
        switch (type)
        {
            case FloatingTextType.Physic: return physicFontSize;
            case FloatingTextType.Magic: return magicFontSize;
            case FloatingTextType.Gold: return goldFontSize;
            case FloatingTextType.Crit: return critFontSize;
            default: return defaultFontSize;
        }
    }
}