using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameOfLife.CellSystem;

public class GameOfDeath : MonoBehaviour
{
    [Header("Grid settings")]
    public int width = 48;
    public int height = 24;

    private int cellSize = 1920 / 48; 


    [Header("Cells Prefabs settings")]
    public GameObject gryffindorPrefab;
    public GameObject slytherinPrefab;
    public GameObject hufflepuffPrefab;
    public GameObject ravenclawPrefab;
    public GameObject volandemortPrefab;
    public GameObject dubledorePrefab;

    [Header("UI References")]
    public Canvas gameCanvas;
    public RectTransform cellContainer;

    [Header("Grid Positioning")]
    public Vector2 gridOffset = Vector2.zero; // Optional offset for the entire grid
    public bool centerGridInContainer = true; // Whether to center the grid in the container

    [Header("Random Generation Settings")]
    [Range(0f, 1f)]
    public float initialFillPercentage = 0.3f;

    // Grid of cells
    private CellInfoOnMap[,] grid;
    // Maps each cell type to its corresponding prefab for easy instantiation
    private Dictionary<GameOfLifeCellType, GameObject> prefabMap;
    // Calculated positioning values
    private Vector2 cellSpacing;
    private Vector2 gridStartPosition;

    void Start()
    {
        // Validate canvas reference
        if (gameCanvas == null)
        {
            Debug.LogError("Canvas reference is missing! Please assign a canvas in the inspector.");
            return;
        }

        // If cellContainer is not assigned, use the canvas's transform
        if (cellContainer == null)
        {
            cellContainer = gameCanvas.GetComponent<RectTransform>();
            Debug.Log("Cell container not assigned, using canvas as container.");
        }

        // Set up prefab dictionary
        prefabMap = new Dictionary<GameOfLifeCellType, GameObject>
        {
            { GameOfLifeCellType.GRYFFINDOR, gryffindorPrefab },
            { GameOfLifeCellType.SLYTHERIN, slytherinPrefab },
            { GameOfLifeCellType.HUFFLEPUFF, hufflepuffPrefab },
            { GameOfLifeCellType.RAVENCLAW, ravenclawPrefab },
            { GameOfLifeCellType.DUMBLEDORE, dubledorePrefab },
            { GameOfLifeCellType.VOLDEMORT, volandemortPrefab }
        };

        // Calculate positioning for the grid
        CalculateGridPositioning();

        // Initialize grid
        InitializeGrid();

        // Populate with random cells
        PopulateRandomCells();
    }

    private void CalculateGridPositioning()
    {
        // Calculate the spacing between cells (accounting for size)
        cellSpacing = new Vector2(cellSize, cellSize);

        // Calculate the total grid size
        float totalGridWidth = width * cellSpacing.x;
        float totalGridHeight = height * cellSpacing.y;

        // Determine the starting position
        if (centerGridInContainer)
        {
            // Get the size of the container
            Vector2 containerSize = cellContainer.rect.size;

            // Calculate the starting position to center the grid
            float startX = -totalGridWidth / 2 + cellSpacing.x / 2;
            float startY = -totalGridHeight / 2 + cellSpacing.y / 2;

            gridStartPosition = new Vector2(startX, startY) + gridOffset;
        }
        else
        {
            // If not centering, use the offset as the starting position
            gridStartPosition = gridOffset;
        }

        Debug.Log($"Grid positioning calculated. Start: {gridStartPosition}, Spacing: {cellSpacing}");
    }

    void Update()
    {
        // Add game update logic here
    }

    private void InitializeGrid()
    {
        // Create the grid
        grid = new CellInfoOnMap[width, height];
        // Initialize each cell in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Create a new cell
                grid[x, y] = new CellInfoOnMap
                {
                    Type = GameOfLifeCellType.DEAD,
                    GridPosition = new Vector2Int(x, y)
                };
            }
        }
        // Set up neighbors for each cell
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetupNeighbors(x, y);
            }
        }
    }

    private void PopulateRandomCells()
    {
        System.Random random = new System.Random();

        // Get the number of cell types (excluding DEAD)
        int cellTypeCount = Enum.GetValues(typeof(GameOfLifeCellType)).Length - 1;

        // Calculate how many cells to fill based on the percentage
        int cellsToFill = Mathf.RoundToInt(width * height * initialFillPercentage);

        for (int i = 0; i < cellsToFill; i++)
        {
            // Pick random coordinates
            int x = random.Next(0, width);
            int y = random.Next(0, height);

            // Pick a random cell type (excluding DEAD which is usually 0)
            int randomTypeIndex = random.Next(1, cellTypeCount + 1);
            GameOfLifeCellType randomType = (GameOfLifeCellType)randomTypeIndex;

            // Spawn the cell
            SpawnCell(x, y, randomType);
        }
    }

    private void SetupNeighbors(int x, int y)
    {
        CellInfoOnMap cell = grid[x, y];
        // Top neighbor
        cell.Top = GetCellAt(x, y + 1);
        // Bottom neighbor
        cell.Bottom = GetCellAt(x, y - 1);
        // Left neighbor
        cell.Left = GetCellAt(x - 1, y);
        // Right neighbor
        cell.Right = GetCellAt(x + 1, y);
        // Diagonal neighbors
        cell.TopLeft = GetCellAt(x - 1, y + 1);
        cell.TopRight = GetCellAt(x + 1, y + 1);
        cell.BottomLeft = GetCellAt(x - 1, y - 1);
        cell.BottomRight = GetCellAt(x + 1, y - 1);
    }

    private CellInfoOnMap GetCellAt(int x, int y)
    {
        // Handle wrapping or boundary conditions
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null; // Or implement wrapping if desired
        return grid[x, y];
    }

    public Vector2 CalculateCellPosition(int x, int y)
    {
        // Calculate the position based on the grid start and spacing
        float posX = gridStartPosition.x + x * cellSpacing.x;
        float posY = gridStartPosition.y + y * cellSpacing.y;

        return new Vector2(posX, posY);
    }

    public void SpawnCell(int x, int y, GameOfLifeCellType type)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        CellInfoOnMap cell = grid[x, y];

        // Destroy previous instance if it exists
        if (cell.Instance != null)
        {
            Destroy(cell.Instance);
        }

        // Set new cell type
        cell.Type = type;

        // If it's not dead, instantiate the appropriate prefab
        if (type != GameOfLifeCellType.DEAD && prefabMap.ContainsKey(type))
        {
            // Calculate the proper position for this cell within the grid
            Vector2 position = CalculateCellPosition(x, y);

            // Instantiate the cell as a child of the canvas or container
            cell.Instance = Instantiate(prefabMap[type], cellContainer);

            // Set the local position within the UI
            RectTransform rectTransform = cell.Instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;

                // Optionally ensure the cell size is set correctly
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            }
            else
            {
                Debug.LogWarning($"Cell prefab {type} doesn't have a RectTransform component. UI positioning may not work correctly.");
                cell.Instance.transform.localPosition = new Vector3(position.x, position.y, 0);
            }
        }
    }

    // For debugging - visualize the grid in the scene view
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = CalculateCellPosition(x, y);
                Vector3 worldPos = cellContainer.TransformPoint(new Vector3(pos.x, pos.y, 0));
                Gizmos.DrawWireCube(worldPos, new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.01f));
            }
        }
    }

    // Additional methods for game logic...
}