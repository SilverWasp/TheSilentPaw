using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float visionRange = 10f; // How far the enemy can see
    public float fieldOfView = 60f; // Vision cone angle
    public Transform player; // Player reference
    public Transform visionOrigin; // Point from which vision is cast (usually eyes or head)
    public LayerMask obstacleMask; // Layers considered obstacles (e.g., walls)

    [Header("Spotlight")]
    public Light visionSpotlight; // Visual spotlight that represents FOV
    public Color normalColor = Color.white; // Default color of spotlight
    public Color detectedColor = Color.red; // Color when player is detected

    private bool playerDetected = false; // Was player seen this frame
    private bool playerInView = false; // Is player currently within FOV and not blocked

    void Start()
    {
        // Set initial spotlight color
        if (visionSpotlight != null)
        {
            visionSpotlight.color = normalColor;
            // Use spotlight settings as vision parameters
            visionRange = visionSpotlight.range;
            fieldOfView = visionSpotlight.spotAngle;
        }
    }

    void Update()
    {
        playerInView = false;

        // Direction and distance from enemy to player
        Vector3 dirToPlayer = player.position - visionOrigin.position;
        float distToPlayer = dirToPlayer.magnitude;

        // Check if player is within range
        if (distToPlayer < visionRange)
        {
            // Check if player is within field of view
            float angle = Vector3.Angle(visionOrigin.forward, dirToPlayer);
            if (angle < fieldOfView / 2f)
            {
                // Raycast to check if vision is blocked by wall
                if (!Physics.Raycast(visionOrigin.position, dirToPlayer.normalized, distToPlayer, obstacleMask))
                {
                    playerInView = true;
                }
            }
        }

        // Handle detection states and transitions
        if (playerInView)
        {
            if (!playerDetected)
            {
                // First time seeing player (this detection session)
                playerDetected = true;
                visionSpotlight.color = detectedColor;

                // Let game manager know player was detected (restarts timer)
                Sc_GameManager.Instance.OnPlayerDetected();
            }

            // Continuously inform the manager player is still being seen
            Sc_GameManager.Instance.OnPlayerBeingWatched();
        }
        else
        {
            if (playerDetected)
            {
                // Player was seen, but now isn't. Start alert countdown.
                playerDetected = false;
                visionSpotlight.color = normalColor;

                // Inform manager that this enemy lost sight
                Sc_GameManager.Instance.OnPlayerLostFromEnemy();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (visionOrigin == null) return;

        Gizmos.color = Color.yellow;

        // Draw left and right bounds of FOV cone
        Vector3 leftRay = Quaternion.Euler(0, -fieldOfView / 2f, 0) * visionOrigin.forward;
        Vector3 rightRay = Quaternion.Euler(0, fieldOfView / 2f, 0) * visionOrigin.forward;

        Gizmos.DrawRay(visionOrigin.position, leftRay * visionRange);
        Gizmos.DrawRay(visionOrigin.position, rightRay * visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(visionOrigin.position, visionOrigin.forward * visionRange); // forward direction
    }
}
