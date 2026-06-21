using UnityEngine;

public class PlayerGlow : MonoBehaviour
{
    [Header("Renderer References")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Renderer legsRenderer;

    [Header("Glow Settings")]
    [SerializeField] private float glowIntensity = 1.5f;
    [SerializeField][Range(0f, 1f)] private float glowAlpha = 0.5f;

    private MaterialData[] jacketMaterials;
    private MaterialData[] legsMaterials;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int SurfaceType = Shader.PropertyToID("_Surface");
    private static readonly int BlendSrc = Shader.PropertyToID("_SrcBlend");
    private static readonly int BlendDst = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
    private struct MaterialData
    {
        public Material mat;
        public Color originalColor;
    }

    private void Awake()
    {
        jacketMaterials = InitRenderer(playerRenderer, "jacket");
        legsMaterials = InitRenderer(legsRenderer, "legs");
    }

    private MaterialData[] InitRenderer(Renderer rend, string label)
    {
        if (rend == null)
        {
            Debug.LogWarning($"PlayerGlow: No renderer assigned for {label}.");
            return null;
        }

        Material[] shared = rend.sharedMaterials;
        Material[] instanced = rend.materials;

        var result = new MaterialData[instanced.Length];

        for (int i = 0; i < instanced.Length; i++)
        {
            Material m = instanced[i];

            Color originalColor = Color.white;
            if (shared[i] != null && shared[i].HasProperty(BaseColor))
            {
                originalColor = shared[i].GetColor(BaseColor).gamma;
            }

            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.EnableKeyword("_EMISSION");
            m.SetColor(EmissionColor, Color.black);

            m.SetFloat(SurfaceType, 1f);
            m.SetFloat(BlendSrc, (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetFloat(BlendDst, (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetFloat(ZWrite, 0f);
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            result[i] = new MaterialData { mat = m, originalColor = originalColor };
        }

        Debug.Log($"PlayerGlow: Initialised {result.Length} material(s) for {label}.");
        return result;
    }

    public void SetGlow(Color color)
    {
        ApplyGlow(jacketMaterials, color);
        ApplyGlow(legsMaterials, color);
    }

    public void ClearGlow()
    {
        ClearGlowOn(jacketMaterials);
        ClearGlowOn(legsMaterials);
    }

    private void ApplyGlow(MaterialData[] mats, Color color)
    {
        if (mats == null) return;
        foreach (var data in mats)
        {
            Color blended = Color.Lerp(data.originalColor, color, 0.5f);
            Color emission = new Color(blended.r, blended.g, blended.b, glowAlpha) * glowIntensity;

            data.mat.EnableKeyword("_EMISSION");
            data.mat.SetColor(EmissionColor, emission);
        }
    }


    private void ClearGlowOn(MaterialData[] mats)
    {
        if (mats == null) return;
        foreach (var data in mats)
        {
            data.mat.SetColor(EmissionColor, Color.black);
            data.mat.DisableKeyword("_EMISSION");
        }
    }
}