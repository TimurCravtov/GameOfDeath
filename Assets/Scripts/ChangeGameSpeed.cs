using UnityEngine;

public class ChangeGameSpeed : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Timer timer;

    public void ChangeTheGamePase( float value ) {
        timer.SetGamePase(value);
    } 
}
