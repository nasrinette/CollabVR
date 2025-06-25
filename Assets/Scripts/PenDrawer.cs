using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;    // Meta Building Blocks / Interaction SDK

[RequireComponent(typeof(NetworkObject))]
public class PenDrawer : NetworkBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawSound;

    [Header("References")]
    [SerializeField] private Transform tip;
    [SerializeField] private NetworkObject strokePrefab;
    [SerializeField] private Material strokeMaterial;

    [Header("Tuning")]
    [SerializeField] private float minPointDistance = 0.003f;

    // Internals
    private bool _isDrawing;
    private List<Vector3> _points = new List<Vector3>();
    private NetworkLine _currentStroke;
    private GrabInteractable _grabInteractable;
    private HandGrabInteractable _handGrabInteractable;

    private void Awake()
    {
        // Look for the grab‐components on any child of this GameObject
        _grabInteractable = GetComponentInChildren<GrabInteractable>();
        _handGrabInteractable = GetComponentInChildren<HandGrabInteractable>();

        if (_grabInteractable == null && _handGrabInteractable == null)
            Debug.LogWarning("[PenDrawer] No GrabInteractable or HandGrabInteractable found in children!");
    }

    void Update()
    {
        if (!HasStateAuthority) return;

        // Bail out (and force‐end) if neither grab-component reports being held
        if (!IsHeld)
        {
            if (_isDrawing) EndStroke();
            return;
        }

        // Now we're sure the pen is in someone's hand → read the triggers
        float rt = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float lt = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        bool drawButton = rt > 0.7f || lt > 0.7f;

        if (drawButton && !_isDrawing) StartStroke();
        else if (!drawButton && _isDrawing) EndStroke();

        if (_isDrawing) AddPointIfNeeded();
    }

    /// <summary>
    /// True if either grab-component is currently selected by a hand/controller.
    /// </summary>
    private bool IsHeld
    {
        get
        {
            if (_grabInteractable != null && _grabInteractable.Interactors.Count > 0)
                return true;
            if (_handGrabInteractable != null && _handGrabInteractable.Interactors.Count > 0)
                return true;
            return false;
        }
    }

    private void StartStroke()
    {
        Debug.Log("[PenDrawer] StartStroke()");
        _isDrawing = true;
        _points.Clear();

        var netObj = Runner.Spawn(
            strokePrefab,
            tip.position,
            Quaternion.identity,
            Object.InputAuthority
        );
        _currentStroke = netObj.GetComponent<NetworkLine>();

        var lr = netObj.GetComponent<LineRenderer>();
        lr.material = new Material(strokeMaterial);
        lr.startWidth = lr.endWidth = 0.008f;
        lr.numCapVertices = 8;
        lr.numCornerVertices = 8;
        lr.alignment = LineAlignment.View;

        // seed the first point, but NO audio here
        _points.Add(tip.position);
        lr.positionCount = 1;
        lr.SetPosition(0, tip.position);
    }

    private void AddPointIfNeeded()
    {
        Vector3 raw = tip.position;

        // only when we've moved far enough
        if (Vector3.Distance(_points[^1], raw) < minPointDistance)
            return;

        // simple 2‐point average: this new point sits halfway 
        // between the last one and the raw tip position
        Vector3 smooth = (_points[^1] + raw) * 0.5f;

        _points.Add(smooth);

        var lr = _currentStroke.GetComponent<LineRenderer>();
        lr.positionCount = _points.Count;
        lr.SetPositions(_points.ToArray());
        if (_points.Count == 2 && audioSource && drawSound && !audioSource.isPlaying)
        {
            audioSource.clip = drawSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }



    private void EndStroke()
    {
        Debug.Log($"[PenDrawer] EndStroke() – sending {_points.Count} points");
        _isDrawing = false;

        if (_currentStroke != null)
        {
            // Send the full point array and color to everyone
            _currentStroke.RPC_InitStroke(
                _points.ToArray(),
                strokeMaterial.color
            );
            _currentStroke = null;
        }
        if (audioSource && audioSource.isPlaying)
        audioSource.Stop();
    }
}
