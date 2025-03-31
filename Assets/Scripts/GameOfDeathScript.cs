using System;
using UnityEngine;

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




    void Update()
    {
       
    }

}

public enum GameOfLifeCellType
{
    DEAD,
    ALIVE
}


class CellInfoOnMap
{

}