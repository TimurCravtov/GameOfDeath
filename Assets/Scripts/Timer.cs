using UnityEngine;

public class Timer : MonoBehaviour
{
    private float startTime;
    private float gameTimer; // time elapsed in game

    public float cellUpdateInterval = 2f; // at which interval to update cells
    private float lastCellUpdate;

    public float inGameClockUpdateInterval = 1f; // at whoch interval to update in-game clock
    private float lastInGameClockUpdate;

    private static bool isTimeToUpdate = false;

    public static bool getTimeToUpdate()
    {
        return isTimeToUpdate;
    }

    public static void setTimeToUpdate(bool value)
    {
        isTimeToUpdate = value;
    }





    void Start()
    {
        startTime = Time.time;
        gameTimer = 0f;
        lastCellUpdate = 0f;
        lastInGameClockUpdate = 0f;
    }

    void Update()
    {
        if (isTimeToUpdate)
        {
            gameTimer = Time.time - startTime;
        }

        if (gameTimer - lastCellUpdate >= cellUpdateInterval)
        {
            UpdateCells();
            lastCellUpdate = gameTimer;
        }

        if (gameTimer - lastInGameClockUpdate >= inGameClockUpdateInterval)
        {
            UpdateInGameClock();
            lastInGameClockUpdate = gameTimer;
        }

        Debug.Log("In-game Time: " + gameTimer);
    }

    void UpdateCells()
    {
        Debug.Log("Updating cells at: " + gameTimer + " seconds.");
        // Add cell update function
    }

    void UpdateInGameClock()
    {
        Debug.Log("Updating in-game clock at: " + gameTimer + " seconds.");
        // Add function to display in-game clock function
    }
}
