using UnityEngine;

namespace GameOfLife.CellSystem
{
    public enum GameOfLifeCellType
    {
        DEAD,
        GRYFFINDOR,
        SLYTHERIN,
        HUFFLEPUFF,
        RAVENCLAW,
        DUMBLEDORE,
        VOLDEMORT
    }

    public class CellInfoOnMap
    {
        public GameOfLifeCellType Type;
        public GameObject Instance;
        public Vector2Int GridPosition;

        public CellInfoOnMap Top;
        public CellInfoOnMap Bottom;
        public CellInfoOnMap Left;
        public CellInfoOnMap Right;
        public CellInfoOnMap TopLeft;
        public CellInfoOnMap TopRight;
        public CellInfoOnMap BottomLeft;
        public CellInfoOnMap BottomRight;

        public Zone CurrentZone;

        public bool IsAlive => Type != GameOfLifeCellType.DEAD;
    }
}
