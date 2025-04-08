using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameOfLife.CellSystem;
using System.Data;
using System.Linq;

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
    private bool startSimulation = false;
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
    public float updateInterval = 0.5f; // Time between generations (in seconds)
    private float timer = 0f;
    void Update()
    {
        // Check for reset key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            clearGrid();
            currentGenerationGrid = null;
            nextGenerationGrid = null;
            Start();
            startSimulation = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startSimulation = true;
        }
        if (startSimulation)
        {
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                UpdateGeneration();
                timer = 0f;
            }

            // Optional: Control speed with keys
            if (Input.GetKeyDown(KeyCode.UpArrow))
                updateInterval = Mathf.Max(0.01f, updateInterval - 0.1f);

            if (Input.GetKeyDown(KeyCode.DownArrow))
                updateInterval += 0.1f;
        }
        else {
            // Check for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                CellInfoOnMap clickedCell = GetCellByCoordinates(mousePosition.x, mousePosition.y);

                if (clickedCell != null)
                {
                    Vector2Int cellPosition = clickedCell.GridPosition;
                    Debug.Log($"Cell clicked at grid position: ({cellPosition.x}, {cellPosition.y})");

                    // Spawn the selected cell type based on the key press
                    if (Input.GetKey(KeyCode.Alpha1))
                    {
                        SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.GRYFFINDOR);
                    }
                    else if (Input.GetKey(KeyCode.Alpha2))
                    {
                        SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.SLYTHERIN);
                    }
                    else if (Input.GetKey(KeyCode.Alpha3))
                    {
                        SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.HUFFLEPUFF);
                    }
                    else if (Input.GetKey(KeyCode.Alpha4))
                    {
                        SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.RAVENCLAW);
                    }
                    else if (Input.GetKey(KeyCode.Alpha5))
                    {
                        SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.VOLDEMORT);
                    }
                }

                else
                {
                    Debug.Log("Click detected outside the grid area");
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                Vector2 mousePosition = Input.mousePosition;
                CellInfoOnMap clickedCell = GetCellByCoordinates(mousePosition.x, mousePosition.y);
                Vector2Int cellPosition = clickedCell.GridPosition;
                SpawnCell(cellPosition.x, cellPosition.y, GameOfLifeCellType.DEAD);

            }
        }
        // Update the simulation for the next generation (if needed)
    }

    void clearGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnCell(x, y, GameOfLifeCellType.DEAD);
            }
        }
    }
    private Vector2Int GetCellIndexesByCoordinates(float x, float y)
    {
        // Screen coordinates: (0,1080) top-left, (1920,0) bottom-right
        // Grid coordinates: (0,0) top-left, (47,23) bottom-right (after flip)

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
                    GridPosition = new Vector2Int(x, y),
                    Instance = null
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
        if (currentGenerationGrid[x, y].Instance != null) {
            Destroy(currentGenerationGrid[x, y].Instance);
            currentGenerationGrid[x, y].Instance = null;
        }

        if (type != GameOfLifeCellType.DEAD && prefabMap.ContainsKey(type))
        {
            Vector2 position = CalculateCellPosition(x, y);
            currentGenerationGrid[x, y] = new CellInfoOnMap
            {
                GridPosition = new Vector2Int(x, y),
                Type = type,
                Instance = Instantiate(prefabMap[type], cellContainer)
            };

            RectTransform rectTransform = currentGenerationGrid[x, y].Instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = position;
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            }
            else
            {
                Debug.LogWarning($"Cell prefab {type} doesn't have a RectTransform component.");
                currentGenerationGrid[x, y].Instance.transform.localPosition = new Vector3(position.x, position.y, 0);
            }

        }
        else
        {
            currentGenerationGrid[x, y].Instance = null;
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
    bool isSlytherinDormitory(int x, int y)
    {
        return x >= 3 && x <= 22 && y >= 10 && y <= 20;
    }
    // Call this every simulation cycle to compute the next generation.
    private void UpdateGeneration()
    {
        // Initialize nextGenerationGrid
        nextGenerationGrid = new CellInfoOnMap[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                nextGenerationGrid[i, j] = new CellInfoOnMap
                {
                    GridPosition = currentGenerationGrid[i, j].GridPosition,
                    Type = currentGenerationGrid[i, j].Type,
                    Instance = null
                };
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellInfoOnMap currentCell = currentGenerationGrid[x, y];
                CellInfoOnMap newCell = nextGenerationGrid[x, y];

                // Count neighbors for this cell
                Dictionary<GameOfLifeCellType, int> neighborCount = CountNeighbors(x, y);
                // --- Overcrowding death rule ---
                int totalLivingNeighbors = 0;
                foreach (var kv in neighborCount)
                {
                    if (kv.Key != GameOfLifeCellType.DEAD)
                        totalLivingNeighbors += kv.Value;
                }
                if (totalLivingNeighbors > 6 && currentCell.Type != GameOfLifeCellType.DEAD)
                {
                    newCell.Type = GameOfLifeCellType.DEAD;
                    nextGenerationGrid[x, y] = newCell;
                    continue; // Skip other rules for this cell
                }
                if (isSlytherinDormitory(x, y))
                {
                    switch (currentCell.Type)
                    {
                        case GameOfLifeCellType.SLYTHERIN:
                            // Slytherins thrive here � can't die in their own dorm
                            newCell.Type = GameOfLifeCellType.SLYTHERIN;
                            break;

                        case GameOfLifeCellType.GRYFFINDOR:
                            // Gryffindors are not welcome � they die
                            newCell.Type = GameOfLifeCellType.DEAD;
                            break;

                        case GameOfLifeCellType.HUFFLEPUFF:
                        case GameOfLifeCellType.RAVENCLAW:
                            // No resurrection for non-Slytherins in this dark environment
                            // Just let them stay dead or follow normal dead rules
                            break;

                        case GameOfLifeCellType.DUMBLEDORE:
                            // Dumbledore can't survive in this hostile region
                            newCell.Type = GameOfLifeCellType.DEAD;
                            break;

                        case GameOfLifeCellType.VOLDEMORT:
                            // Voldemort is strengthened � convert neighbors
                            TransformHogwartsNeighbors(x, y);
                            newCell.Type = GameOfLifeCellType.VOLDEMORT;
                            break;

                        case GameOfLifeCellType.DEAD:
                            // Resurrection not possible in Slytherin dormitory
                            break;
                    }
                    nextGenerationGrid[x, y] = newCell;
                    continue;
                }
                // --- Hogwarts House Cells Rules ---
                else if (IsHouseCell(currentCell.Type))
                {
                    // Survival rule: if 2-3 same house neighbors, cell survives.
                    int sameHouse = neighborCount.ContainsKey(currentCell.Type) ? neighborCount[currentCell.Type] : 0;
                    if (!(sameHouse >= 2 && sameHouse <= 3))
                    {
                        // Also check if the cell is overwhelmed by diversity
                        int distinctHouseTypes = 0;
                        foreach (var kv in neighborCount)
                        {
                            if (IsHouseCell(kv.Key))
                            {
                                distinctHouseTypes++;
                            }
                        }
                        if (distinctHouseTypes > 4)
                        {
                            newCell.Type = GameOfLifeCellType.DEAD;
                        }
                    }
                    // Rivalry: Gryffindor vs. Slytherin
                    if (currentCell.Type == GameOfLifeCellType.GRYFFINDOR)
                    {
                        int slythCount = neighborCount.ContainsKey(GameOfLifeCellType.SLYTHERIN) ? neighborCount[GameOfLifeCellType.SLYTHERIN] : 0;
                        if (slythCount > 4)
                        {
                            newCell.Type = GameOfLifeCellType.DEAD;
                        }
                    }
                    if (currentCell.Type == GameOfLifeCellType.SLYTHERIN)
                    {
                        int gryffCount = neighborCount.ContainsKey(GameOfLifeCellType.GRYFFINDOR) ? neighborCount[GameOfLifeCellType.GRYFFINDOR] : 0;
                        if (gryffCount > 4)
                        {
                            newCell.Type = GameOfLifeCellType.DEAD;
                        }
                    }
                    // Hufflepuff is preyed upon by Slytherin.
                    if (currentCell.Type == GameOfLifeCellType.HUFFLEPUFF)
                    {
                        int slythCount = neighborCount.ContainsKey(GameOfLifeCellType.SLYTHERIN) ? neighborCount[GameOfLifeCellType.SLYTHERIN] : 0;
                        if (slythCount >= 2)
                        {
                            newCell.Type = GameOfLifeCellType.DEAD;
                        }
                    }
                }
                // --- Dumbledore Rules ---
                else if (currentCell.Type == GameOfLifeCellType.DUMBLEDORE)
                {
                    int voldCount = neighborCount.ContainsKey(GameOfLifeCellType.VOLDEMORT) ? neighborCount[GameOfLifeCellType.VOLDEMORT] : 0;
                    int slythCount = neighborCount.ContainsKey(GameOfLifeCellType.SLYTHERIN) ? neighborCount[GameOfLifeCellType.SLYTHERIN] : 0;
                    // Dumbledore dies if there�s any Voldemort or too many Slytherins.
                    if (voldCount >= 1 || slythCount > 5)
                    {
                        newCell.Type = GameOfLifeCellType.DEAD;
                    }
                    else
                    {
                        // Remains as Dumbledore.
                        newCell.Type = GameOfLifeCellType.DUMBLEDORE;
                        // Attempt to revive any dead neighbors (50% chance per dead neighbor).
                        ReviveDeadNeighbors(x, y);
                    }
                }
                // --- Voldemort Rules ---
                else if (currentCell.Type == GameOfLifeCellType.VOLDEMORT)
                {
                    int dumbledoreCount = neighborCount.ContainsKey(GameOfLifeCellType.DUMBLEDORE) ? neighborCount[GameOfLifeCellType.DUMBLEDORE] : 0;
                    // Voldemort dies if surrounded by three or more Dumbledore cells.
                    if (dumbledoreCount >= 3)
                    {
                        newCell.Type = GameOfLifeCellType.DEAD;
                    }
                    else
                    {
                        newCell.Type = GameOfLifeCellType.VOLDEMORT;
                        // Transform any neighboring Hogwarts house cell (except Slytherin) into Voldemort.
                        TransformHogwartsNeighbors(x, y);
                    }
                }
                // --- Revival for Dead Cells via Ravenclaw/Hufflepuff Interaction ---
                else if (currentCell.Type == GameOfLifeCellType.DEAD)
                {
                    int totalNeighbors = neighborCount.Count;
                    // Reproduction rule: exactly 3 neighbors causes birth
                    if (totalNeighbors == 3)
                    {
                        // Find the most common house type among neighbors
                        var houseCounts = neighborCount
                            .Where(kv => IsHouseCell(kv.Key))
                            .OrderByDescending(kv => kv.Value)
                            .ToList();
                        Debug.Log(houseCounts);
                        if (houseCounts.Count > 0)
                        {
                            newCell.Type = houseCounts[0].Key;
                        }
                    }
                }


                // Assign the new cell into the next generation grid.
                nextGenerationGrid[x, y] = newCell;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellInfoOnMap cell = nextGenerationGrid[x, y];
                SpawnCell(x, y, cell.Type);
            }
        }
    }

    // Helper: Count neighbor types for cell at (x,y)
    private Dictionary<GameOfLifeCellType, int> CountNeighbors(int x, int y)
    {
        Dictionary<GameOfLifeCellType, int> counts = new Dictionary<GameOfLifeCellType, int>();

        // List of all 8 possible neighbor positions.
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1),
            new Vector2Int(-1,  0),                      new Vector2Int(1,  0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
        };

        foreach (Vector2Int offset in neighborOffsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                GameOfLifeCellType neighborType = currentGenerationGrid[nx, ny].Type;
                if (counts.ContainsKey(neighborType))
                {
                    counts[neighborType]++;
                }
                else
                {
                    counts[neighborType] = 1;
                }
            }
        }

        return counts;
    }

    // Helper: Determines if a cell type is one of the Hogwarts houses.
    private bool IsHouseCell(GameOfLifeCellType type)
    {
        return type == GameOfLifeCellType.GRYFFINDOR ||
               type == GameOfLifeCellType.SLYTHERIN ||
               type == GameOfLifeCellType.HUFFLEPUFF ||
               type == GameOfLifeCellType.RAVENCLAW;
    }

    // Helper: For a Dumbledore cell, attempt to revive neighboring dead cells.
    // For each dead neighbor, with a 50% chance, spawn a random Hogwarts house.
    private void ReviveDeadNeighbors(int x, int y)
    {
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1),
            new Vector2Int(-1,  0),                      new Vector2Int(1,  0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
        };

        // Array of Hogwarts house types for random selection.
        GameOfLifeCellType[] houseTypes = new GameOfLifeCellType[]
        {
            GameOfLifeCellType.GRYFFINDOR,
            GameOfLifeCellType.SLYTHERIN,
            GameOfLifeCellType.HUFFLEPUFF,
            GameOfLifeCellType.RAVENCLAW
        };

        foreach (Vector2Int offset in neighborOffsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                // Revive only if the neighbor is currently dead.
                if (currentGenerationGrid[nx, ny].Type == GameOfLifeCellType.DEAD)
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        // Choose a random house type.
                        GameOfLifeCellType revivedType = houseTypes[UnityEngine.Random.Range(0, houseTypes.Length)];
                        // Directly update the next generation grid for the neighbor.
                        nextGenerationGrid[nx, ny] = new CellInfoOnMap
                        {
                            GridPosition = new Vector2Int(nx, ny),
                            Type = revivedType
                        };
                    }
                }
            }
        }
    }

    // Helper: For a Voldemort cell, transform any neighboring Hogwarts house cell (except Slytherin) into Voldemort.
    private void TransformHogwartsNeighbors(int x, int y)
    {
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1),
            new Vector2Int(-1,  0),                      new Vector2Int(1,  0),
            new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
        };

        foreach (Vector2Int offset in neighborOffsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                GameOfLifeCellType neighborType = currentGenerationGrid[nx, ny].Type;
                // Transform if the neighbor is a Hogwarts house (but not Slytherin) and not already Voldemort.
                if (IsHouseCell(neighborType) && neighborType != GameOfLifeCellType.SLYTHERIN)
                {
                    nextGenerationGrid[nx, ny] = new CellInfoOnMap
                    {
                        GridPosition = new Vector2Int(nx, ny),
                        Type = GameOfLifeCellType.VOLDEMORT
                    };
                }
            }
        }
    }


    /// <summary>
    /// Special rules for specific regions of the grid. *In progress*
    /// </summary>
    private Dictionary<string, (Vector2Int start, Vector2Int end, GameOfLifeRule rule)> specialRegions;
    private void InitializeSpecialRegions()
    {
        specialRegions = new Dictionary<string, (Vector2Int, Vector2Int, GameOfLifeRule)>
        {
            { "Slytherin Dormitory", (new Vector2Int(0, 0), new Vector2Int(5, 5), new GameOfLifeRule { surviveMin = 2, surviveMax = 3, birth = 3 }) },
            { "Dumbledore's Office", (new Vector2Int(6, 6), new Vector2Int(9, 9), new GameOfLifeRule { surviveMin = 1, surviveMax = 4, birth = 2 }) }
        };
    }
    private GameOfLifeRule GetRuleForPosition(int x, int y)
    {
        foreach (var region in specialRegions)
        {
            Vector2Int start = region.Value.start;
            Vector2Int end = region.Value.end;

            if (x >= start.x && x <= end.x && y >= start.y && y <= end.y)
            {
                return region.Value.rule;
            }
        }

        // Default rule
        return new GameOfLifeRule { surviveMin = 2, surviveMax = 3, birth = 3 };
    }
}
public class GameOfLifeRule
{
    public int surviveMin;
    public int surviveMax;
    public int birth;
}
