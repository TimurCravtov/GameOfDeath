using System;
using System.Collections.Generic;
using UnityEngine;
using GameOfLife.CellSystem;

public class GameOfDeath : MonoBehaviour
{
    [Header("Grid settings")]
    public int width = 50;
    public int height = 50;
    public float cellSize = 0.5f;

    [Header("Cells Prefabs settings")]
    public GameObject gryffindorPrefab;
    public GameObject slytherinPrefab;
    public GameObject hufflepuffPrefab;
    public GameObject ravenclawPrefab;
    public GameObject volandemortPrefab;
    public GameObject dubledorePrefab;

    // Maps each cell type to its corresponding prefab for easy instantiation
    private Dictionary<GameOfLifeCellType, GameObject> prefabMap;

    void Start()
    {
        prefabMap = new Dictionary<GameOfLifeCellType, GameObject>
        {
            { GameOfLifeCellType.GRYFFINDOR, gryffindorPrefab },
            { GameOfLifeCellType.SLYTHERIN, slytherinPrefab },
            { GameOfLifeCellType.HUFFLEPUFF, hufflepuffPrefab },
            { GameOfLifeCellType.RAVENCLAW, ravenclawPrefab },
            { GameOfLifeCellType.DUMBLEDORE, dubledorePrefab },
            { GameOfLifeCellType.VOLDEMORT, volandemortPrefab }
        };
    }

    void Update()
    {

    }
}