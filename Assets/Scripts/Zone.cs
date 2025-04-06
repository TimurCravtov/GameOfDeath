using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public string test;
    public string Name;

    public Vector2Int TopLeftCell;
    public Vector2Int BottomRightCell;

    public void Start()
    {
        Debug.Log("Zone initiated:" + Name + " " + TopLeftCell.ToString() + ", " + BottomRightCell.ToString());
    }

    public bool IsInZone(int x, int y)
    {
        //Debug.Log("IsInZone params: " + x + " " + y);
        //Debug.Log("Zone " + Name + " " + TopLeftCell.ToString() + ", " + BottomRightCell.ToString());

        bool verdict = x >= TopLeftCell.x && x <= BottomRightCell.x &&
               y >= TopLeftCell.y && y <= BottomRightCell.y;
        //Debug.Log(verdict);

        return verdict;
    }
}



public enum ZoneType
{
    SLYTHERIN, DUBLEDORE, NONE
}