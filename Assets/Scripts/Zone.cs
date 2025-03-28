using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public string test;
    public string Name;

    public Vector2 TopLeft;
    public Vector2 BottomRight;

    public bool IsInZone(float x, float y)
    {
        return x >= TopLeft.x && x <= BottomRight.x &&
               y <= TopLeft.y && y >= BottomRight.y;
    }
}
