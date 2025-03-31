using UnityEngine;
using GameOfLife.CellSystem;

public class CellBehaviour : MonoBehaviour
{
    // This reference holds the data for this specific cell instance
    public CellInfoOnMap Info;

    // Called to assign cell info when the prefab is instantiated
    public void Initialize(CellInfoOnMap info)
    {
        Info = info;

        // OPTIONAL: Here you could update visuals based on Info.Type
        // Example: change color or animation depending on the cell type
    }
}
