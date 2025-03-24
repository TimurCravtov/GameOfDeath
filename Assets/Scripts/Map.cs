using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int width = 20;
    public int height = 20;

    private GameOfLifeCellType[,] cells; // Removed property syntax, needs initialization

    void Start()
    {
        cells = new GameOfLifeCellType[width, height]; // Initialize the array
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


