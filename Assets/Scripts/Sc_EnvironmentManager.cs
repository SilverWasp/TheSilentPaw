using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Centralized environment settings manager (fog, ambient light, skybox, etc.)
/// Attach this to an empty GameObject and configure in Inspector.
/// </summary>
[ExecuteAlways]
public class Sc_EnvironmentManager : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public FogMode fogMode = FogMode.Linear;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.01f;
    public float fogStartDistance = 0f;
    public float fogEndDistance = 30f;

    [Header("Ambient Light")]
    public Color ambientLight = Color.white;
    public AmbientMode ambientMode = AmbientMode.Flat;

    [Header("Skybox")]
    public Material skyboxMaterial;

    [Header("Directional Light (Optional)")]
    public Light directionalLight;
    public Color directionalColor = Color.white;
    public float directionalIntensity = 1f;

    void OnEnable()
    {
        ApplyEnvironmentSettings();
    }

    void OnValidate()
    {
        ApplyEnvironmentSettings();
    }

    public void ApplyEnvironmentSettings()
    {
        // Fog settings
        RenderSettings.fog = enableFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;

        // Ambient light
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientLight = ambientLight;

        // Skybox
        if (skyboxMaterial != null)
            RenderSettings.skybox = skyboxMaterial;

        // Directional light
        if (directionalLight != null)
        {
            directionalLight.color = directionalColor;
            directionalLight.intensity = directionalIntensity;
        }
    }
}
