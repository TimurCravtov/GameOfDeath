using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GameOfDeath : MonoBehaviour
{

    [Header("Grid settings")]
    public int width = 20;
    public int height = 20;

    [Header("Cells settings")]
    public GameObject cellPrefab;


    [Header("Zone settings")]
    public Zone zone1;
    public Zone zone2;


    private GameOfLifeCellType[,] cells;

    void Start()
    {
        cells = new GameOfLifeCellType[width, height];
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Add initialization logic here if needed
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = GameOfLifeCellType.ONE_VALUE;
            }
        }
    }

    private void Update()
    {

    }

    void UpdateCellVisual(int x, int y)
    {
    }


    void SetCell(GameOfLifeCellType cellType, int xIndex, int yIndex)
    {
        if (xIndex >= 0 && xIndex < width && yIndex >= 0 && yIndex < height)
        {
            cells[xIndex, yIndex] = cellType;
        }
        else
        {
            Console.Error.WriteLine("Wrong index " + xIndex + " " + yIndex);
        }
    }
}


enum GameOfLifeCellType
{
    ONE_VALUE
}


