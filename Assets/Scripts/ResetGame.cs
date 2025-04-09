using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public GameOfDeath gameState;

    public void HandleClick()
    {
        gameState.TriggerResetWithRandom();
    }
}