using System.Collections.Generic;
using Fusion;
using UnityEngine;
using OVR;  // or the correct Oculus SDK namespace

public class PenDrawer : NetworkBehaviour
{
    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;
    // [SerializeField] private AudioClip drawSound;

    [Header("References")]
    [SerializeField] private Transform tip;
    [Tooltip("Drag the LineStroke prefab's NetworkObject here")]
    [SerializeField] private NetworkObject strokePrefab;

    [Header("Appearance")]
    [Tooltip("Material (and color) used for strokes")]
    [SerializeField] private Material strokeMaterial;

    [Header("Tuning")]
    [SerializeField] private float minPointDistance = 0.003f;

    private bool _isDrawing;
    private List<Vector3> _points = new();
    private NetworkLine _currentStroke;


    void Update()
    {
        if (!HasStateAuthority) return;

        // Get analog values for both index triggers
        float rightTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float leftTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        // Drawing starts if either trigger is pressed past the threshold
        bool drawButton = rightTrigger > 0.7f || leftTrigger > 0.7f;

        if (drawButton && !_isDrawing) StartStroke();
        else if (!drawButton && _isDrawing) EndStroke();

        if (_isDrawing) AddPointIfNeeded();
    }


    private void StartStroke()
    {
        Debug.Log("[PenDrawer] StartStroke()");
        _isDrawing = true;
        _points.Clear();
        float width   = PenSettings.Instance.strokeWidth;
      Color colour  = PenSettings.Instance.StrokeColor;
        var netObj = Runner.Spawn(
            strokePrefab,
            tip.position,
            Quaternion.identity,
            Object.InputAuthority
        );

        _currentStroke = netObj.GetComponent<NetworkLine>();

        var lr = netObj.GetComponent<LineRenderer>();
        // lr.material = new Material(strokeMaterial);
        // lr.startWidth = 0.008f;
        // lr.endWidth = 0.008f;
        lr.material        = new Material(strokeMaterial);
        lr.material.color  = colour;       // per-stroke colour
        lr.startWidth = lr.endWidth = width;

        // Smoother line settings
        lr.numCapVertices = 8;      // Rounded ends
        lr.numCornerVertices = 8;   // Rounded corners
        lr.alignment = LineAlignment.View; // Optional: always face camera

        _points.Add(tip.position);
        lr.positionCount = 1;
        lr.SetPosition(0, tip.position); 
        // if (audioSource != null && drawSound != null)
        // {
        //     audioSource.PlayOneShot(drawSound);
        // }
    }

    private void AddPointIfNeeded()
    {
        var p = tip.position;
        if (Vector3.Distance(_points[^1], p) >= minPointDistance)
        {
            Debug.Log($"[PenDrawer] Adding point {p}");
            _points.Add(p);

            var lr = _currentStroke.GetComponent<LineRenderer>();
            lr.positionCount = _points.Count;
            lr.SetPositions(_points.ToArray());
        }
    }




    private void EndStroke()
    {
        Debug.Log($"[PenDrawer] EndStroke() – sending {_points.Count} points");
        _isDrawing = false;

        // direct RPC call on the spawned component
        // _currentStroke.RPC_InitStroke(
        //     _points.ToArray(),
        //     strokeMaterial.color
        // );
      _currentStroke.RPC_InitStroke(
          _points.ToArray(),
          PenSettings.Instance.StrokeColor,
          PenSettings.Instance.strokeWidth
      );
        _currentStroke = null;
    }
} 