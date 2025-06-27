using UnityEngine;

/// <summary>
/// Shows the currently-selected palette colour on the pen nib.
/// Drop this on the mesh that represents the tip of each pen.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class PenTipVisualizer : MonoBehaviour
{
    [Header("Leave empty to auto-duplicate the current material")]
    [SerializeField] private Material tipMat;     // optional reference

    private MeshRenderer _mr;

    // ───────────────────────────────────────────────────────────────
    void Awake()
    {
        _mr = GetComponent<MeshRenderer>();

        // Duplicate or assign a unique material once.
        if (tipMat == null)
        {
            // No material supplied ➜ duplicate whatever is already on the mesh.
            tipMat = _mr.material;                // Unity auto-instantiates a copy.
        }
        else
        {
            // A shared material was supplied ➜ instantiate our own copy.
            tipMat = Instantiate(tipMat);
            _mr.material = tipMat;
        }
    }

    // ───────────────────────────────────────────────────────────────
    void Start()
    {
        // By Start(), PenSettings.Instance is guaranteed to exist.
        ApplyColor(PenSettings.Instance.StrokeColor);

        // Subscribe to live palette changes.
        PenSettings.OnColorChanged += ApplyColor;
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid dangling delegates if the pen is destroyed.
        PenSettings.OnColorChanged -= ApplyColor;
    }

    // ----------------------------------------------------------------
    // Helper
    // ----------------------------------------------------------------
    private void ApplyColor(Color c)
    {
        // Fastest way when each pen owns its own material.
        tipMat.color = c;

        // If you switch to using a shared material + MaterialPropertyBlock,
        // comment the line above and use _mr.SetPropertyBlock(…) instead.
    }
}
