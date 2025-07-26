using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sc_PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 moveDir;
    private Rigidbody rb;
    void Awake()
    {
        // Cache Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Convert to isometric direction
        moveDir = new Vector3(h - v, 0, h + v).normalized;
    }

    void FixedUpdate()
    {
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }
}
