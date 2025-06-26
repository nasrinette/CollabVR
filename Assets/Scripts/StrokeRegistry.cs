// StrokeRegistry.cs
using System.Collections.Generic;
using UnityEngine;

public class StrokeRegistry : MonoBehaviour
{
    public static StrokeRegistry Instance { get; private set; }
    public readonly List<NetworkLine> Strokes = new();

    void Awake() => Instance = this;

    public void Register(NetworkLine l)  => Strokes.Add(l);
    public void Unregister(NetworkLine l) => Strokes.Remove(l);
}
