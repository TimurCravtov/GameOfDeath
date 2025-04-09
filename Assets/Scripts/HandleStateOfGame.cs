using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleState : MonoBehaviour
{
    public GameOfDeath gameState;
    public Button button;
    public TextMeshProUGUI buttonText;

    public Sprite startSprite;
    public Sprite resumeSprite;

    private Image buttonImage; // <-- No need to assign in inspector

    public void Start()
    {
        // Get the Image component attached to the Button GameObject
        buttonImage = button.GetComponent<Image>();
        UpdateButtonVisuals();
    }

    public void HandleClick()
    {
        gameState.startSimulation = !gameState.startSimulation;
        UpdateButtonVisuals();
    }

    public void UpdateButtonVisuals()
    {
        if (gameState.startSimulation)
        {
            //buttonText.text = "Resume";
            buttonImage.sprite = startSprite;
        }
        else
        {
            buttonImage.sprite = resumeSprite;
            //buttonText.text = "Start";
        }
    }
}