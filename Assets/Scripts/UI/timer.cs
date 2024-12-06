using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class timer : MonoBehaviour
{
    public float countdownTime = 600f; // Time in seconds for the countdown
    [SerializeField] TextMeshProUGUI timerText;           // Reference to a UI Text component

    public GameManagerScript gameManager;
    private float currentTime;
    private bool isCountingDown = false;

    void Start()
    {
        ResetTimer();
        StartCountdown();
    }

    void Update()
    {
        if (currentTime - Time.deltaTime <= 0 && isCountingDown)
            {
                isCountingDown = false;
                currentTime = 0;
                OnCountdownEnd();
            }

        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

        }
    }

    // Resets the timer to the initial countdown time
    public void ResetTimer()
    {
        currentTime = countdownTime;
        UpdateTimerDisplay();
    }

    // Starts the countdown
    public void StartCountdown()
    {
        isCountingDown = true;
    }

    // Stops the countdown
    public void StopCountdown()
    {
        isCountingDown = false;
    }

    // Updates the displayed timer text
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Called when the countdown ends
    private void OnCountdownEnd()
    {
        Debug.Log("Countdown has ended!");
        gameManager.gameOver();
        // Add any additional behavior here, such as triggering events or animations
    }
}
