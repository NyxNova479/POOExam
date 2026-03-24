// Script simple pour détruire un GameObject après un temps spécifié
using UnityEngine;

// Helper class to destroy fragments after time
public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 2f;
    private float fadeStartTime;
    private bool isFading = false;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private float fadeDuration = 0.5f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            originalColor = meshRenderer.material.color;
        }

        fadeStartTime = lifetime - fadeDuration;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float aliveTime = Time.time - Time.deltaTime;

        // Start fading when approaching end of lifetime
        if (!isFading && aliveTime > fadeStartTime)
        {
            isFading = true;
        }

        if (isFading && meshRenderer != null && meshRenderer.material != null)
        {
            float fadeProgress = (aliveTime - fadeStartTime) / fadeDuration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(originalColor.a, 0f, fadeProgress);

            // Make sure material can fade
            if (meshRenderer.material.HasProperty("_Mode"))
            {
                meshRenderer.material.SetFloat("_Mode", 3); // Fade mode
            }

            if (!meshRenderer.material.HasProperty("_Color"))
            {
                // Create a new material instance that can be modified
                meshRenderer.material = new Material(meshRenderer.material);
            }

            meshRenderer.material.color = newColor;
        }
    }
}