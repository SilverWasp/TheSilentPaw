using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sc_SpotLight : MonoBehaviour
{
    private Light spotLight;
    private Sc_EnemyVision enemyVision;

    void Awake()
    {
        spotLight = GetComponent<Light>();
        spotLight.intensity = 5f; // Set default intensity

        // Look for Sc_EnemyVision in parent
        enemyVision = GetComponentInParent<Sc_EnemyVision>();

        if (enemyVision != null)
        {
            // Automatically sync spotlight FOV and range with enemy values
            spotLight.range = enemyVision.visionRange;
            spotLight.spotAngle = enemyVision.fieldOfView;
        }
        else
        {
            Debug.LogWarning("Sc_EnemyVision not found in parent for spotlight: " + gameObject.name);
        }
    }

    void Update()
    {
        // Optional: continuously sync if values may change at runtime
        if (enemyVision != null)
        {
            spotLight.range = enemyVision.visionRange;
            spotLight.spotAngle = enemyVision.fieldOfView;
        }
    }
}
