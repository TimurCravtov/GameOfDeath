using UnityEngine;

public class Timer : MonoBehaviour
{
    [Range(0.5f, 3f)]
    public float timerInterval = 1f;

    private float lastUpdateTime = 0f;
    private float gameTimer = 0f;

    private bool _shouldUpdate = false;

    void Start()
    {
        lastUpdateTime = 0f;
        gameTimer = 0f;
    }

    void Update()
    {
        gameTimer = Time.time;

        if (gameTimer - lastUpdateTime >= timerInterval)
        {
            lastUpdateTime = gameTimer;
            _shouldUpdate = true;
        }
    }

    public void IncreaseInterval()
    {
        timerInterval = Mathf.Min(timerInterval + 0.5f, 3f);
        Debug.Log($"Increased interval: {timerInterval}");
    }

    public void SetGamePase(float speed) 
    {
        timerInterval = Mathf.Max(Mathf.Min(speed, 3f), 0.5f ) ;
    }

    public void DecreaseInterval()
    {
        timerInterval = Mathf.Max(timerInterval - 0.5f, 0.5f);
        Debug.Log($"Decreased interval: {timerInterval}");
    }

    /// <summary>
    /// Returns true only once per interval. Resets after returning true.
    /// </summary>
    public bool IsTimeToUpdate()
    {
        if (_shouldUpdate)
        {
            _shouldUpdate = false;
            return true;
        }
        return false;
    }
}
