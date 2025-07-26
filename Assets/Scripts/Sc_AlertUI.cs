using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sc_AlertUI : MonoBehaviour
{
    [Header("UI References")]
    public Image screenTint;
    public TextMeshProUGUI alertTimerText;
    public Image alertVignetteImage;

    [Header("Colors")]
    public Color normalTint = new Color(0f, 0.4f, 0.5f, 0.3f); // Calm teal
    public Color alertTint = new Color(0.5f, 0f, 0f, 0.3f);     // Strong dark red

    [Header("Pulse Settings")]
    public float pulseDuration = 2f; // Time for one full pulse cycle
    public float minAlpha = 0.15f;    // Lowest vignette alpha
    public float maxAlpha = 0.35f;   // Highest vignette alpha

    private Coroutine alertRoutine;
    private Coroutine pulseRoutine;
    private RectTransform textRect;

    private float currentTimeRemaining = 0f;
    private bool isShrinking = false;

    void Start()
    {
        textRect = alertTimerText.GetComponent<RectTransform>();
        screenTint.color = normalTint;
        alertTimerText.text = "";
        alertTimerText.gameObject.SetActive(false);
        alertVignetteImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called once per enemy detection event to trigger alert visuals and Detected message.
    /// </summary>
    public void TriggerAlertUI(float duration)
    {
        if (alertRoutine != null)
            StopCoroutine(alertRoutine);
        alertRoutine = StartCoroutine(AlertSequence(duration));
    }

    /// <summary>
    /// Called every frame by enemies who see the player.
    /// Shows "Being Watched" text during alert.
    /// </summary>
    public void ShowBeingWatched()
    {
        if (alertTimerText != null && currentTimeRemaining > 0f)
        {
            alertTimerText.text = "Being Watched";
        }
    }

    /// <summary>
    /// Starts or updates the alert countdown when player is hidden.
    /// </summary>
    public void UpdateAlertCountdown(float remainingTime)
    {
        currentTimeRemaining = Mathf.Max(0f, remainingTime);
        int displayTime = Mathf.CeilToInt(currentTimeRemaining);

        if (!alertTimerText.gameObject.activeSelf)
            alertTimerText.gameObject.SetActive(true);

        if (!isShrinking && currentTimeRemaining < Mathf.FloorToInt(displayTime))
        {
            // Begin shrinking only once timer starts counting down
            isShrinking = true;
        }

        if (isShrinking)
        {
            // Smoothly move and shrink the timer to top
            textRect.anchoredPosition = Vector2.Lerp(textRect.anchoredPosition, new Vector2(0, 300), Time.deltaTime * 5f);
            textRect.localScale = Vector3.Lerp(textRect.localScale, Vector3.one, Time.deltaTime * 5f);
        }

        alertTimerText.text = displayTime.ToString();
    }

    /// <summary>
    /// Called by GameManager when alert ends. Resets UI state.
    /// </summary>
    public void ClearAlertUI()
    {
        alertTimerText.text = "";
        alertTimerText.gameObject.SetActive(false);
        screenTint.color = normalTint;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        alertVignetteImage.gameObject.SetActive(false);
        isShrinking = false;
    }

    /// <summary>
    /// Handles pulsing vignette effect by oscillating alpha.
    /// </summary>
    IEnumerator PulseVignette()
    {
        float time = 0f;
        Color baseColor = new Color(1f, 0f, 0f, 0f); // Red, alpha adjusted in loop

        while (true)
        {
            float pulse = Mathf.Sin(time * Mathf.PI * 2f / pulseDuration) * 0.5f + 0.5f; // 0 to 1
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);
            alertVignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Initial alert sequence — sets up tint, message, and starts vignette pulse.
    /// </summary>
    IEnumerator AlertSequence(float duration)
    {
        screenTint.color = alertTint;
        alertVignetteImage.gameObject.SetActive(true);

        // "Detected" shown for the first 1 second
        alertTimerText.text = "Detected";
        alertTimerText.gameObject.SetActive(true);
        textRect.anchoredPosition = new Vector2(0,200);
        textRect.localScale = Vector3.one * 2f;
        isShrinking = false;

        // Start pulsing
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseVignette());

        // Keep "Detected" for 1 second
        yield return new WaitForSeconds(1f);

        // If the alert is still active, switch to "Being Watched" or let GameManager update timer
        alertTimerText.text = "";

        alertRoutine = null;
    }
}
