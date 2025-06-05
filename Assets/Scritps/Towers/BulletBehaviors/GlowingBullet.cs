using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GlowingBullet : MonoBehaviour
{
    public float speed = 30f;
    [ColorUsage(true, true)] public Color emissionColor = Color.cyan;
    public float emissionIntensity = 2f;

    private MaterialPropertyBlock _mpb;
    private MeshRenderer _renderer;
    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();
        SetEmission(emissionColor, emissionIntensity);
    }

    public void SetEmission(Color color, float intensity)
    {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_EmissionColor", color * intensity);
        _renderer.SetPropertyBlock(_mpb);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}