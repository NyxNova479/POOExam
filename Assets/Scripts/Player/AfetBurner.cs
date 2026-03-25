using UnityEngine;

public class AfterburnerEffect : MonoBehaviour
{
    public ParticleSystem mainThruster;
    public Light thrusterLight;
    public float minEmission = 5f;
    public float maxEmission = 50f;
    public float minLightIntensity = 0.5f;
    public float maxLightIntensity = 3f;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        if (mainThruster != null)
            emission = mainThruster.emission;

        if (thrusterLight == null)
            thrusterLight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        // Récupérer la vitesse actuelle du joueur (vertical input comme accélération)
        float speed = Mathf.Abs(Input.GetAxis("Vertical"));

        // Ajuster l'émission de particules en fonction de la vitesse
        if (mainThruster != null)
        {
            emission.rateOverTimeMultiplier = Mathf.Lerp(minEmission, maxEmission, speed);
        }

        // Ajuster l'intensité de la lumière
        if (thrusterLight != null)
        {
            thrusterLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, speed);
        }
    }
}