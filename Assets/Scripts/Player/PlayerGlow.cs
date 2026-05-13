using UnityEngine;

public class PlayerGlow : MonoBehaviour
{
    [Header("Renderer Reference")]
    [SerializeField] private Renderer playerRenderer;

    [Header("Glow Intensity")]
    [SerializeField] private float glowIntensity = 1.5f;

    private Material materialInstance;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (playerRenderer == null)
            playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null)
        {
            materialInstance = playerRenderer.material;
        }
        else
        {
            Debug.LogWarning("PlayerGlow: No Renderer found.");
        }
    }

    public void SetGlow(Color color)
    {
        if (materialInstance == null) return;

        materialInstance.EnableKeyword("_EMISSION");
        materialInstance.SetColor(EmissionColor, color * glowIntensity);
    }

    public void ClearGlow()
    {
        if (materialInstance == null) return;

        materialInstance.SetColor(EmissionColor, Color.black);
        materialInstance.DisableKeyword("_EMISSION");
    }
}
