using UnityEngine;

public class Sc_SeekerAI : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 30f; // degrees per second

    [Header("Detection")]
    public Transform visionOrigin;
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public LayerMask playerLayer;
    public LayerMask obstacleMask;

    void Update()
    {
        // Rotate the Seeker's parent in Y (circle scanning)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Optional: Detect player every frame (for now)
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(visionOrigin.position, viewDistance, playerLayer);

        foreach (var hit in hits)
        {
            Vector3 dirToPlayer = (hit.transform.position - visionOrigin.position).normalized;
            float angleToPlayer = Vector3.Angle(visionOrigin.forward, dirToPlayer);

            if (angleToPlayer < viewAngle / 2f)
            {
                float distToPlayer = Vector3.Distance(visionOrigin.position, hit.transform.position);

                // Check for line of sight (no obstacle in between)
                if (!Physics.Raycast(visionOrigin.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    Debug.Log("Seeker sees the player!");
                    Sc_GameManager.Instance.OnPlayerDetected(); // Assuming such method exists
                }
            }
        }
    }
}
