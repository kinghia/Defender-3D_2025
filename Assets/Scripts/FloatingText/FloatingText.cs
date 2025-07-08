using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    public float lifetime = 1.0f;
    public float speed = 1.0f;
    public AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private TextMeshPro textMesh;
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

    public void Init(string content, Vector3 worldPos, Color color, float fontSize, System.Action<FloatingText> onDespawnCallback, float duration = 0f)
    {
        transform.position = worldPos;
        startPos = worldPos;
        endPos = worldPos + Vector3.up * speed;
        textMesh.text = content;
        textMesh.color = color;
        textMesh.fontSize = fontSize;
        onDespawn = onDespawnCallback;
        if (duration > 0) {
            lifetime = duration;
        }
    }

    void FixedUpdate()
    {
        if (!isActive) return;
        elapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        // Move up
        transform.position = Vector3.Lerp(startPos, endPos, floatCurve.Evaluate(t));
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