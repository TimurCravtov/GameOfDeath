using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public string test;
    public string Name;

    public Vector2 TopLeftCell;
    public Vector2 BottomRightCell; //inclusive

    public bool IsInZone(float x, float y)
    {
        return x >= TopLeftCell.x && x <= BottomRightCell.x &&
               y <= TopLeftCell.y && y >= BottomRightCell.y;
    }
}
