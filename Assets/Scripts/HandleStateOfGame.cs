using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleState : MonoBehaviour
{
    public GameOfDeath gameState; // Make sure to assign this in the Inspector
    public Button button;
    public TextMeshProUGUI buttonText;

    public void Start()
    {
        UpdateButtonText(); // Set initial button text
    }

    public void HandleClick()
    {
        gameState.startSimulation = !gameState.startSimulation;
        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        if (gameState.startSimulation)
        {
            buttonText.text = "Resume";
        }
        else
        {
            buttonText.text = "Start";
        }
    }
}