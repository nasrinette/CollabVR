// BButtonExportTrigger.cs
using UnityEngine;

public class BButtonExportTrigger : MonoBehaviour
{
    ObjStrokeExporter exporter;

    void Start() => exporter = FindObjectOfType<ObjStrokeExporter>();

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))   // B on Touch / Touch Pro
            exporter.ExportSceneStrokesToOBJ();
    }
}
