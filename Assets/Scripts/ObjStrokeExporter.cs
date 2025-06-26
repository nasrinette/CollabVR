// ObjStrokeExporter.cs
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;   // ← keep the other usings

public class ObjStrokeExporter : MonoBehaviour
{
    const string HEADER = "# VR-Draw OBJ export\n";

    public void ExportSceneStrokesToOBJ()
    {
        var strokes = StrokeRegistry.Instance?.Strokes;
        if (strokes == null || strokes.Count == 0)
        {
            Debug.LogWarning("[Exporter] No strokes found.");
            return;
        }

        StringBuilder sb = new StringBuilder(HEADER);
        int vOffset = 1;

        foreach (var s in strokes)
        {
            LineRenderer lr = s.GetComponent<LineRenderer>();
            if (lr == null) continue;

            int n = lr.positionCount;
            Vector3[] pts = new Vector3[n];
            lr.GetPositions(pts);

            // vertices
            for (int i = 0; i < n; i++)
            {
                Vector3 p = pts[i];
                sb.AppendFormat("v {0} {1} {2}\n", -p.x, p.y, p.z); // flip X (Unity L-hand → OBJ R-hand)
            }

            // poly-line
            sb.Append('l');
            for (int i = 0; i < n; i++)
                sb.AppendFormat(" {0}", vOffset + i);
            sb.Append('\n');

            vOffset += n;
        }

        string fileName = $"draw_{System.DateTime.Now:yyyyMMdd_HHmmss}.obj";

        // string path = Path.Combine(Application.persistentDataPath, fileName);
        string basePath;

// ===== Android / Quest =====
        #if UNITY_ANDROID && !UNITY_EDITOR
        basePath = "/sdcard/Download";                 // “Downloads” seen in Files app
        #else
        // ===== Everything else (Editor, Windows build, etc.) =====
        basePath = Application.persistentDataPath;     // keep old behaviour on PC
        #endif

        Directory.CreateDirectory(basePath);           // makes sure the folder exists
        string path = Path.Combine(basePath, fileName);

        File.WriteAllText(path, sb.ToString());
        #if UNITY_ANDROID && !UNITY_EDITOR
        using var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                        .GetStatic<AndroidJavaObject>("currentActivity");
        using var media = new AndroidJavaClass("android.media.MediaScannerConnection");
        media.CallStatic("scanFile", act, new string[] { path }, null, null);
        #endif


        Debug.Log($"[Exporter] Saved {path}");


    }
}
