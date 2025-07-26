using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_GameManager : MonoBehaviour
{
    public static Sc_GameManager Instance;
    public Sc_AlertUI alertUI;

    [Header("Message State")]
    public bool HasMessage = false; // Whether player holds a message (game-specific)

    [Header("Enemy Alert System")]
    public bool isAlerted = false; // Global alert state
    public float alertDuration = 15f; // How long alert lasts (after last sighting)
    private float alertTimer = 0f; // Internal countdown timer

    private int enemiesSeeingPlayer = 0; // How many enemies currently see the player
    private bool countdownStarted = false; // Whether the timer has started after losing sight

    void Awake()
    {
        // Singleton pattern to make GameManager globally accessible
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // While alerted, and no enemy currently sees the player, start countdown
        if (isAlerted && countdownStarted)
        {
            alertTimer -= Time.deltaTime;

            if (alertUI != null)
                alertUI.UpdateAlertCountdown(alertTimer); // Update UI with remaining time

            if (alertTimer <= 0f)
            {
                // Alert ends when timer reaches 0
                isAlerted = false;
                countdownStarted = false;

                if (alertUI != null)
                    alertUI.ClearAlertUI(); // Hide timer & reset visuals

                Debug.Log("Alert state ended. Enemies are back to normal.");
            }
        }
    }

    /// <summary>
    /// Called once when an enemy sees the player for the first time.
    /// </summary>
    public void OnPlayerDetected()
    {
        if (!isAlerted)
        {
            // First detection: enter alert state
            isAlerted = true;
            alertTimer = alertDuration;

            if (alertUI != null)
                alertUI.TriggerAlertUI(alertDuration);

            Debug.Log("ALERT TRIGGERED: Player detected by an enemy!");
        }
        else
        {
            // Already alerted, but player got seen again — reset timer
            alertTimer = alertDuration;

            if (alertUI != null)
                alertUI.TriggerAlertUI(alertDuration); // UI handles Detected message + reset

            Debug.Log("Player detected again! Alert timer reset.");
        }

        // Important: count how many enemies are watching
        enemiesSeeingPlayer++;
        countdownStarted = false;
    }

    /// <summary>
    /// Called every frame by enemies who currently see the player.
    /// </summary>
    public void OnPlayerBeingWatched()
    {
        if (isAlerted && alertUI != null)
        {
            alertUI.ShowBeingWatched(); // Show 'Being Watched' UI message
        }
    }

    /// <summary>
    /// Called once by each enemy when they lose sight of the player.
    /// </summary>
    public void OnPlayerLostFromEnemy()
    {
        enemiesSeeingPlayer = Mathf.Max(0, enemiesSeeingPlayer - 1); // Safety check

        if (enemiesSeeingPlayer == 0 && isAlerted)
        {
            // No enemies see player — start countdown
            countdownStarted = true;
            Debug.Log("All enemies lost the player. Countdown started.");
        }
    }
}
