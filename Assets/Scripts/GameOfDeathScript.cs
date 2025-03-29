using System;
using UnityEngine;

public class GameOfDeath : MonoBehaviour
{
    [Header("Grid settings")]
    public int width = 50;
    public int height = 50;
    public float cellSize = 0.5f;

    [Header("Cells settings")]
    public GameObject cellPrefab;

    private GameOfLifeCellType[,] cells;
    private GameObject[,] cellObjects;

    // Reference to camera and its controller
    private Camera gameCamera;
    private CameraController cameraController;

    void Start()
    {
        // Initialize arrays
        cells = new GameOfLifeCellType[width, height];
        cellObjects = new GameObject[width, height];

        // Set up camera
        SetupCamera();

        // Create grid
        CreateGrid();
    }

    void SetupCamera()
    {
        // First, try to find an existing camera in the scene
        gameCamera = Camera.main;

        // If no camera exists, create one
        if (gameCamera == null)
        {
            Debug.Log("No main camera found. Creating a new camera.");

            // Create a new GameObject for the camera
            GameObject cameraObject = new GameObject("Game Camera");

            // Add Camera component
            gameCamera = cameraObject.AddComponent<Camera>();
            gameCamera.orthographic = true;

            // Set tag to make it the main camera
            cameraObject.tag = "MainCamera";
        }

        // Position camera to see grid center
        float gridWorldWidth = width * cellSize;
        float gridWorldHeight = height * cellSize;

        // Calculate the center position of the grid
        Vector3 gridCenter = new Vector3(
            gridWorldWidth / 2,
            gridWorldHeight / 2,
            -10  // Z position for the camera to see the grid
        );

        // Position camera at grid center
        gameCamera.transform.position = gridCenter;

        // Set orthographic size to fit the grid
        gameCamera.orthographicSize = Mathf.Max(
            gridWorldHeight / 2,  // Half height
            (gridWorldWidth / 2) * (Screen.height / (float)Screen.width)  // Adjusted width for aspect ratio
        ) * 1.1f;  // Add 10% padding

        // Add camera controller
        cameraController = gameCamera.gameObject.AddComponent<CameraController>();

        Debug.Log("Camera set up at position: " + gridCenter);
        Debug.Log("Camera orthographic size: " + gameCamera.orthographicSize);
    }

    void CreateGrid()
    {
        // Create an empty parent for all cells
        GameObject gridParent = new GameObject("Grid");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate position
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);

                // Instantiate cell
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, gridParent.transform);
                cell.name = $"Cell_{x}_{y}";

                // Scale according to cell size (slightly smaller for visual borders)
                cell.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 1);

                // Store reference
                cellObjects[x, y] = cell;

                // Initialize state
                cells[x, y] = GameOfLifeCellType.DEAD;

                // Color the cell (assuming it has a SpriteRenderer)
                SpriteRenderer renderer = cell.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = Color.black; // Default color for dead cells
                }
            }
        }

        Debug.Log("Grid created with " + (width * height) + " cells");
    }

    // Update is called once per frame
    void Update()
    {
        // For testing, toggle random cells on spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleRandomCells();
        }
    }

    void ToggleRandomCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (UnityEngine.Random.value > 0.8f)
                {
                    cells[x, y] = cells[x, y] == GameOfLifeCellType.ALIVE ?
                        GameOfLifeCellType.DEAD : GameOfLifeCellType.ALIVE;

                    // Update visual
                    SpriteRenderer renderer = cellObjects[x, y].GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        renderer.color = cells[x, y] == GameOfLifeCellType.ALIVE ?
                            Color.white : Color.black;
                    }
                }
            }
        }
    }
}

public enum GameOfLifeCellType
{
    DEAD,
    ALIVE
}