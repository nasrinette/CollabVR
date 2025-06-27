using UnityEngine;
using System;

public class PenSettings : MonoBehaviour
{
    public static PenSettings Instance { get; private set; }

    public float  strokeWidth = 0.008f;

    Color _strokeColor = Color.red;
    public Color StrokeColor => _strokeColor;

    public static event Action<Color> OnColorChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetStrokeColor(Color c)
    {
        if (c == _strokeColor) return;
        _strokeColor = c;
        OnColorChanged?.Invoke(c);
    }
}
