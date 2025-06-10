using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    public float floatSpeed = 1.5f;
    public float fadeDuration = 1.0f;
    public float punchScale = 1.2f;
    public float punchDuration = 0.15f;
    public AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TextMeshPro textMesh;
    private Color startColor;
    private float lifetime;
    private float elapsed;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isActive;
    private Camera mainCamera;
    private System.Action<FloatingText> onDespawn;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        elapsed = 0f;
        isActive = true;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void Init(string content, Vector3 worldPos, Color color, float fontSize, System.Action<FloatingText> onDespawnCallback)
    {
        transform.position = worldPos;
        startPos = worldPos;
        endPos = worldPos + Vector3.up * 1.5f;
        textMesh.text = content;
        textMesh.color = color;
        textMesh.fontSize = fontSize;
        startColor = color;
        lifetime = fadeDuration;
        onDespawn = onDespawnCallback;
        // Punch scale animation
        transform.localScale = Vector3.one * punchScale;
    }

    void Update()
    {
        if (!isActive) return;
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        // Move up
        transform.position = Vector3.Lerp(startPos, endPos, floatCurve.Evaluate(t));
        // Fade out
        var c = textMesh.color;
        c.a = Mathf.Lerp(startColor.a, 0, t);
        textMesh.color = c;
        // Billboard
        if (mainCamera)
            transform.forward = mainCamera.transform.forward;
        // Lifetime
        if (elapsed >= lifetime)
        {
            isActive = false;
            onDespawn?.Invoke(this);
        }
    }
} 