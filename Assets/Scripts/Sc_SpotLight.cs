using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light), typeof(HDAdditionalLightData))]
public class Sc_SpotLight : MonoBehaviour
{
    private Light unityLight;
    private HDAdditionalLightData hdData;
    private Sc_EnemyVision enemyVision;

    [Header("HDRP Light Settings")]
    public float intensityEv100 = 28f;
    public float defaultRange = 20f;
    public bool enableVolumetrics = true;
    public Color lightColor = Color.white;

    void Awake()
    {
        unityLight = GetComponent<Light>();
        hdData = GetComponent<HDAdditionalLightData>();
        enemyVision = GetComponentInParent<Sc_EnemyVision>();

        // Set up the spotlight
        unityLight.type = LightType.Spot;
        unityLight.color = lightColor;
        unityLight.range = defaultRange;

        // Use built-in Light.lightUnit (Unity 6+)
        unityLight.lightUnit = LightUnit.Lumen;

        // Convert EV100 to Lumen and assign
        unityLight.intensity = LightUnitUtils.ConvertIntensity(
            unityLight,
            intensityEv100,
            LightUnit.Ev100,
            unityLight.lightUnit
        );

        // Volumetric light toggle
        hdData.volumetricDimmer = enableVolumetrics ? 1f : 0f;

        SyncWithEnemyVision();
    }

    void Update()
    {
        SyncWithEnemyVision();
    }

    private void SyncWithEnemyVision()
    {
        if (enemyVision != null)
        {
            unityLight.spotAngle = enemyVision.fieldOfView;
            unityLight.range = enemyVision.visionRange;

            // Recalculate intensity if EV100 changed at runtime
            unityLight.intensity = LightUnitUtils.ConvertIntensity(
                unityLight,
                intensityEv100,
                LightUnit.Ev100,
                unityLight.lightUnit
            );
        }
    }
}
