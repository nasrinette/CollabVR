using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class PenTipVisualizer : MonoBehaviour
{
    [Header("Leave empty to auto-duplicate the current material")]
    [SerializeField] private Material tipMat;   

    private MeshRenderer _mr;

    
    void Awake()
    {
        _mr = GetComponent<MeshRenderer>();

        if (tipMat == null)
        {
            tipMat = _mr.material;                
        }
        else
        {
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

      private void ApplyColor(Color c)
    {
        // Fastest way when each pen owns its own material.
        tipMat.color = c;

       
    }
}
