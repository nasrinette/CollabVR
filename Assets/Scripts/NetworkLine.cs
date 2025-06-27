using Fusion;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(NetworkObject))]
public class NetworkLine : NetworkBehaviour
{
    private LineRenderer _lr;

    public override void Spawned()
    {
        _lr = GetComponent<LineRenderer>();
         StrokeRegistry.Instance?.Register(this);
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        StrokeRegistry.Instance?.Unregister(this);
    }

   [Rpc(RpcSources.All, RpcTargets.All)]

    public void RPC_InitStroke(Vector3[] points, Color col, float width)
    {
        _lr.positionCount = points.Length;
        _lr.SetPositions(points);

        var mat = new Material(_lr.material);
        mat.color = col;
        _lr.material = mat;
    }
}
