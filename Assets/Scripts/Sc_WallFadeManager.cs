using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_WallFadeManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform cameraTransform;

    [Header("Fade Settings")]
    public LayerMask wallMask;
    public Material fadeMaterial;
    public float fadeCheckInterval = 0.1f;

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private List<Renderer> fadedWalls = new List<Renderer>();
    private float nextCheckTime = 0f;

    void Update()
    {
        if (Time.time < nextCheckTime) return;
        nextCheckTime = Time.time + fadeCheckInterval;

        // Reset all faded walls
        foreach (Renderer r in fadedWalls)
        {
            if (r != null && originalMaterials.ContainsKey(r))
                r.material = originalMaterials[r];
        }
        fadedWalls.Clear();

        // Cast rays from camera to player
        Vector3 direction = player.position - cameraTransform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, direction.normalized, distance, wallMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();
            if (r != null)
            {
                if (!originalMaterials.ContainsKey(r))
                    originalMaterials[r] = r.material;

                r.material = fadeMaterial;
                fadedWalls.Add(r);
            }
        }
    }
}

