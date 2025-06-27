// StrokeMenuController.cs – final version
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrokeMenuController : MonoBehaviour
{
    [SerializeField] List<Button> buttons;      // drag the 6 buttons here
    [SerializeField] int columns = 3;
    [Header("OVR input")]
    [SerializeField] OVRInput.RawAxis2D navAxis  = OVRInput.RawAxis2D.RThumbstick;
    [SerializeField] OVRInput.RawButton selectButton = OVRInput.RawButton.RIndexTrigger;

    int   current = 0;
    float cooldown = 0.25f;
    float nextMove = 0f;

    void Start() => Highlight(current);

    void Update()
    {
        Vector2 a = OVRInput.Get(navAxis);

        if (Time.time >= nextMove)
        {
            int dx = Mathf.RoundToInt(a.x);
            int dy = Mathf.RoundToInt(a.y);

            if (dx != 0 || dy != 0)
            {
                Move(dx, dy);
                nextMove = Time.time + cooldown;
            }
        }

        if (OVRInput.GetDown(selectButton))
            Activate(current);
    }

    void Move(int dx, int dy)
    {
        int rows = Mathf.CeilToInt(buttons.Count / (float)columns);
        int x = current % columns;
        int y = current / columns;

        x = (x + dx + columns) % columns;
        y = Mathf.Clamp(y - dy, 0, rows - 1);

        current = Mathf.Clamp(y * columns + x, 0, buttons.Count - 1);
        Highlight(current);
    }

    void Highlight(int id)
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].transform.localScale = (i == id) ? Vector3.one * 1.2f : Vector3.one;
    }

    void Activate(int id)
    {
        string n = buttons[id].name;

        if (n.StartsWith("Width"))
        {
            float w = n switch {
                "Width-Small"  => 0.005f,
                "Width-Medium" => 0.010f,
                "Width-Large"  => 0.020f,
                _              => 0.010f
            };
            PenSettings.Instance.strokeWidth = w;
        }
        else                      // colour buttons
        {
            Color c = n switch {
                "Red"   => Color.red,
                "Green" => Color.green,
                "Blue"  => Color.blue,
                _       => Color.white
            };
            PenSettings.Instance.SetStrokeColor(c);
        }
    }
}
