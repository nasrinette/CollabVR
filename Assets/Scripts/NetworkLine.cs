using Fusion;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(NetworkObject))]
public class NetworkLine : NetworkBehaviour
{
    private LineRenderer _lr;

    public override void Spawned()
    {
        _lr = GetComponent<LineRenderer>();
    }

    // RPC signature in Fusion 2: no RpcInfo param, call instance.RPC_*(…)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    public void RPC_InitStroke(Vector3[] points, Color col)
    {
        _lr.positionCount = points.Length;
        _lr.SetPositions(points);

        // give each stroke its own material instance so colours don't bleed
        var mat = new Material(_lr.material);
        mat.color = col;
        _lr.material = mat;
    }
}
