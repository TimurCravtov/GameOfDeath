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
    public Vector2 gridOffset = Vector2.zero;
    public bool centerGridInContainer = true;

    [Header("Random Generation Settings")]
    [Range(0f, 1f)]
    public float initialFillPercentage = 0.3f;

    private CellInfoOnMap[,] currentGenerationGrid;
    private CellInfoOnMap[,] nextGenerationGrid;

    private Dictionary<GameOfLifeCellType, GameObject> prefabMap;
    private Vector2 cellSpacing;
    private Vector2 gridStartPosition;

    void Start()
    {
        if (gameCanvas == null)
        {
            Debug.LogError("Canvas reference is missing! Please assign a canvas in the inspector.");
            return;
        }

        if (cellContainer == null)
        {
            cellContainer = gameCanvas.GetComponent<RectTransform>();
            Debug.Log("Cell container not assigned, using canvas as container.");
        }

        prefabMap = new Dictionary<GameOfLifeCellType, GameObject>
        {
            { GameOfLifeCellType.GRYFFINDOR, gryffindorPrefab },
            { GameOfLifeCellType.SLYTHERIN, slytherinPrefab },
            { GameOfLifeCellType.HUFFLEPUFF, hufflepuffPrefab },
            { GameOfLifeCellType.RAVENCLAW, ravenclawPrefab },
            { GameOfLifeCellType.DUMBLEDORE, dubledorePrefab },
            { GameOfLifeCellType.VOLDEMORT, volandemortPrefab }
        };

        CalculateGridPositioning();
        InitializeGrid();
        PopulateRandomCells();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            CellInfoOnMap clickedCell = GetCellByCoordinates(mousePosition.x, mousePosition.y);

            if (clickedCell != null)
            {
                Vector2Int cellPosition = clickedCell.GridPosition;
                Debug.Log($"Cell clicked at grid position: ({cellPosition.x}, {cellPosition.y})");
                Debug.Log($"Cell type: {clickedCell.Type}");

                if (clickedCell.Type == GameOfLifeCellType.DEAD)
                {
                    SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.GRYFFINDOR);
                }
                else
                {
                    SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.DEAD);
                }
            }
            else
            {
                Debug.Log("Click detected outside the grid area");
            }
        }
    }

    private Vector2Int GetCellIndexesByCoordinates(float x, float y)
    {
        // Screen coordinates: (0,1080) top-left, (1920,0) bottom-right
        // There are 27 cells in height and 48 cells in width
        // each cell is 40*40 pixels

        float gridWidth = width * cellSize;
        float gridHeight = height * cellSize;

        // Calculate screen-space position of grid based on centered placement
        float gridLeft = (1920f / 2) - (gridWidth / 2) + gridOffset.x;
        float gridTop = (1080f / 2) + (gridHeight / 2) + gridOffset.y; // Top edge NOT adjusted for cell size

        // Convert mouse position to grid-relative position
        float relativeX = x - gridLeft;
        float relativeY = gridTop - y; // Y decreases downward in screen space, so invert

        // Convert to grid indices
        int gridX = Mathf.FloorToInt(relativeX / cellSize);
        int gridY = Mathf.FloorToInt(relativeY / cellSize);

        // Check bounds
        if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height)
        {
            return new Vector2Int(gridX, gridY);
        }

        return new Vector2Int(-1, -1);
    }

    private CellInfoOnMap GetCellByCoordinates(float x, float y)
    {
        Vector2Int position = GetCellIndexesByCoordinates(x, y);

        if (position.x >= 0 && position.y >= 0 &&
            position.x < width && position.y < height)
        {
            return currentGenerationGrid[position.x, position.y];
        }

        return null;
    }

    private void CalculateGridPositioning()
    {
        cellSpacing = new Vector2(cellSize, cellSize);
        float totalGridWidth = width * cellSpacing.x;
        float totalGridHeight = height * cellSpacing.y;

        if (centerGridInContainer)
        {
            Vector2 containerSize = cellContainer.rect.size;
            float startX = -totalGridWidth / 2 + cellSpacing.x / 2;
            float startY = totalGridHeight / 2 - cellSpacing.y / 2; // Start from top, adjusted for cell size
            gridStartPosition = new Vector2(startX, startY) + gridOffset;
        }
        else
        {
            gridStartPosition = gridOffset;
        }

        Debug.Log($"Grid positioning calculated. Start: {gridStartPosition}, Spacing: {cellSpacing}");
    }

    private void InitializeGrid()
    {
        currentGenerationGrid = new CellInfoOnMap[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                currentGenerationGrid[x, y] = new CellInfoOnMap
                {
                    Type = GameOfLifeCellType.DEAD,
                    GridPosition = new Vector2Int(x, y)
                };
            }
        }

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
        int cellTypeCount = Enum.GetValues(typeof(GameOfLifeCellType)).Length - 1;
        int cellsToFill = Mathf.RoundToInt(width * height * initialFillPercentage);

        for (int i = 0; i < cellsToFill; i++)
        {
            int x = random.Next(0, width);
            int y = random.Next(0, height);
            int randomTypeIndex = random.Next(1, cellTypeCount + 1);
            GameOfLifeCellType randomType = (GameOfLifeCellType)randomTypeIndex;
            SpawnCell(x, y, randomType);
        }
    }

    private void SetupNeighbors(int x, int y)
    {
        CellInfoOnMap cell = currentGenerationGrid[x, y];
        cell.Top = GetCellAt(x, y + 1);
        cell.Bottom = GetCellAt(x, y - 1);
        cell.Left = GetCellAt(x - 1, y);
        cell.Right = GetCellAt(x + 1, y);
        cell.TopLeft = GetCellAt(x - 1, y + 1);
        cell.TopRight = GetCellAt(x + 1, y + 1);
        cell.BottomLeft = GetCellAt(x - 1, y - 1);
        cell.BottomRight = GetCellAt(x + 1, y - 1);
    }

    private CellInfoOnMap GetCellAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;
        return currentGenerationGrid[x, y];
    }

    public Vector2 CalculateCellPosition(int x, int y)
    {
        float posX = gridStartPosition.x + x * cellSpacing.x;
        float posY = gridStartPosition.y - y * cellSpacing.y; // Y decreases downward from top
        return new Vector2(posX, posY);
    }

    public void SpawnCell(int x, int y, GameOfLifeCellType type)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        CellInfoOnMap cell = currentGenerationGrid[x, y];

        if (cell.Instance != null)
        {
            Destroy(cell.Instance);
        }

        cell.Type = type;

        if (type != GameOfLifeCellType.DEAD && prefabMap.ContainsKey(type))
        {
            Vector2 position = CalculateCellPosition(x, y);
            cell.Instance = Instantiate(prefabMap[type], cellContainer);

            RectTransform rectTransform = cell.Instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            }
            else
            {
                Debug.LogWarning($"Cell prefab {type} doesn't have a RectTransform component.");
                cell.Instance.transform.localPosition = new Vector3(position.x, position.y, 0);
            }
        }
        else
        {
            cell.Instance = null;
        }
    }

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
}