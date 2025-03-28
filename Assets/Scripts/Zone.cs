using UnityEngine;
using System;

public class Zone : MonoBehaviour
{
    public string Name { get; set; }
    public (float X, float Y) TopLeft { get; set; }
    public (float X, float Y) BottomRight { get; set; }

    public Zone(string name, float topLeftX, float topLeftY, float bottomRightX, float bottomRightY)
    {
        Name = name;
        TopLeft = (topLeftX, topLeftY);
        BottomRight = (bottomRightX, bottomRightY);
    }

    public static bool IsInZone(Zone z, float x, float y)
    {
        return x >= z.TopLeft.X && x <= z.BottomRight.X &&
               y <= z.TopLeft.Y && y >= z.BottomRight.Y;
    }

}
