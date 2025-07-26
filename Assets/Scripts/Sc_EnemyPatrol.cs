using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_EnemyPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int current = 0;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float rotationSpeed = 2f;

    [Header("Patrol Timing")]
    public float waitTimeAtWaypoint = 2f;      // Time to stay idle
    public float preMoveRotationTime = 1.5f;   // Time to rotate before moving

    [Header("Vision")]
    public Transform visionOrigin;

    private bool isWaiting = false;
    private bool isRotating = false;
    private float waitTimer = 0f;
    private float rotateTimer = 0f;

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            HandleWait();
            return;
        }

        if (isRotating)
        {
            HandleRotationToNextWaypoint();
            return;
        }

        // === Movement Logic ===
        Vector3 moveDir = (waypoints[current].position - transform.position);
        Vector3 moveDirNormalized = moveDir.normalized;

        // Move toward waypoint
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].position, speed * Time.deltaTime);

        // Rotate toward direction while moving
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirNormalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Smooth vision cone rotation
            if (visionOrigin != null)
            {
                Quaternion targetVisionRotation = Quaternion.LookRotation(moveDirNormalized);
                visionOrigin.rotation = Quaternion.Slerp(visionOrigin.rotation, targetVisionRotation, Time.deltaTime * rotationSpeed);
            }
        }

        // Reached the waypoint?
        if (moveDir.magnitude < 0.1f)
        {
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    void HandleWait()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            isRotating = true;
            rotateTimer = preMoveRotationTime;
        }
    }

    void HandleRotationToNextWaypoint()
    {
        int nextIndex = (current + 1) % waypoints.Length;
        Vector3 dirToNext = (waypoints[nextIndex].position - transform.position).normalized;

        if (dirToNext != Vector3.zero)
        {
            // Smooth enemy body rotation
            Quaternion targetRotation = Quaternion.LookRotation(dirToNext);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Smooth vision cone rotation
            if (visionOrigin != null)
            {
                Quaternion targetVisionRotation = Quaternion.LookRotation(dirToNext);
                visionOrigin.rotation = Quaternion.Slerp(visionOrigin.rotation, targetVisionRotation, Time.deltaTime * rotationSpeed);
            }
        }

        rotateTimer -= Time.deltaTime;
        if (rotateTimer <= 0f)
        {
            isRotating = false;
            current = (current + 1) % waypoints.Length;
        }
    }
}