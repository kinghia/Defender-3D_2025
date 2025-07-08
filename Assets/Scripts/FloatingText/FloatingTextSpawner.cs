using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [Header("References")]
    public FloatingTextPool pool;

    [Header("Critical Hit Symbol")]
    [SerializeField] private string criticalHitSymbol = "💥";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnText(string content, Vector3 worldPosition, Color color, int fontsize = 12, float duration = 1.5f)
    {
        var floatingText = pool.Get();
        Vector3 position = worldPosition;

        floatingText.Init(content, worldPosition, color, fontsize, OnTextDespawned, duration);
    }

    public void SpawnDamage(string content, Vector3 worldPosition, Color color, int fontsize = 12, bool isCrit = false)
    {
        var floatingText = pool.Get();

        // Generate random offset around the position using configurable ranges
        Vector3 randomOffset = new Vector3(
            Random.Range(-1, 1), // Random X offset
            Random.Range(0.5f, 1.5f), // Random Y offset (always positive to float up)
            0
        );

        Vector3 position = worldPosition + randomOffset;

        if (isCrit)
        {
            content = criticalHitSymbol + " " + content;
            fontsize = (int)(fontsize * 1.5f);
        }

        floatingText.Init(content, position, color, fontsize, OnTextDespawned);
    }

    private void OnTextDespawned(FloatingText text)
    {
        pool.ReturnToPool(text);
    }
}