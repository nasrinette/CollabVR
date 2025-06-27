using Fusion;
using UnityEngine;
using OVR; 
public class EraserTool : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform tip;
    [Tooltip("How close the eraser tip must be to a stroke to erase it")]
    [SerializeField] private float eraseRadius = 0.03f;

    private bool _isErasing;

    void Update()
    {
        if (!HasStateAuthority) return;

        // Pinch (index trigger) on either hand
        float rightTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float leftTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        bool eraseButton = rightTrigger > 0.7f || leftTrigger > 0.7f;

        if (eraseButton && !_isErasing)
        {
            _isErasing = true;
            TryErase();
        }
        else if (!eraseButton && _isErasing)
        {
            _isErasing = false;
        }
    }

    private void TryErase()
    {
        // Find all strokes in the scene
        NetworkLine[] strokes = FindObjectsOfType<NetworkLine>();
        foreach (var stroke in strokes)
        {
            // Only erase if we have authority to despawn
            if (stroke.Object == null || !stroke.Object.HasStateAuthority)
                continue;

            var lr = stroke.GetComponent<LineRenderer>();
            if (lr == null) continue;

            int count = lr.positionCount;
            for (int i = 0; i < count; i++)
            {
                if (Vector3.Distance(lr.GetPosition(i), tip.position) < eraseRadius)
                {
                    Runner.Despawn(stroke.Object);
                    break;
                }
            }
        }
    }
}
