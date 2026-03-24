using UnityEngine;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionForce = 15f;
    public float explosionRadius = 5f;
    public float upwardsModifier = 1f;
    public float fragmentLifetime = 2.5f;
    public Vector2 fragmentsRange = new Vector2(5, 8); // Min and max number of fragments

    [Header("Chaos Settings")]
    public float torqueMultiplier = 200f;
    public float scaleVariation = 0.3f;
    public float velocityVariation = 0.5f;

    public void ExplodeObject(GameObject objectToExplode)
    {
        // Récupérer le MeshRenderer et le MeshFilter de l'objet
        MeshRenderer meshRenderer = objectToExplode.GetComponent<MeshRenderer>();
        if(meshRenderer == null ) { meshRenderer= objectToExplode.GetComponentInChildren<MeshRenderer>(); }
        MeshFilter meshFilter = objectToExplode.GetComponent<MeshFilter>();
        if(meshFilter == null ) { meshFilter = objectToExplode.GetComponentInChildren<MeshFilter>(); }

        if (meshRenderer == null || meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogWarning("Object cannot be exploded: missing valid mesh components!");
            return;
        }

        // Get original properties
        Mesh originalMesh = meshFilter.mesh;
        Material originalMaterial = meshRenderer.material;
        Vector3 originalPosition = objectToExplode.transform.position;
        Quaternion originalRotation = objectToExplode.transform.rotation;
        Vector3 originalScale = objectToExplode.transform.localScale;

        // Create a container for fragments
        GameObject fragmentsContainer = new GameObject(objectToExplode.name + "_Fragments");
        fragmentsContainer.transform.position = originalPosition;

        // Determine random number of fragments
        int fragmentCount = Mathf.RoundToInt(Random.Range(fragmentsRange.x, fragmentsRange.y));

        // Create the fragments
        CreateRandomFragments(objectToExplode, originalMesh, originalMaterial, originalPosition,
                             originalRotation, originalScale, fragmentsContainer.transform, fragmentCount);

        // Disable the original object renderer and collider
        meshRenderer.enabled = false;

        Collider objCollider = objectToExplode.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }

        // Set the container to be destroyed when all fragments are gone
        Destroy(fragmentsContainer, fragmentLifetime + 0.2f);

        // Destroy the original object after a delay
        Destroy(objectToExplode, fragmentLifetime + 0.1f);
    }

    private void CreateRandomFragments(GameObject originalObject, Mesh originalMesh, Material originalMaterial,
                                    Vector3 position, Quaternion rotation, Vector3 scale,
                                    Transform containerTransform, int fragmentCount)
    {
        // Get mesh bounds for reference
        Bounds bounds = originalMesh.bounds;

        for (int i = 0; i < fragmentCount; i++)
        {
            // Create fragment GameObject
            GameObject fragment = new GameObject("Fragment_" + i);
            fragment.transform.parent = containerTransform;

            // Copy original transform
            fragment.transform.position = position;
            fragment.transform.rotation = rotation;
            float originalScale = originalObject.transform.localScale.x;
            // Apply scale variation
            float scaleMultiplier = Random.Range(originalScale - scaleVariation, originalScale) * 0.5f;
            fragment.transform.localScale = scale * scaleMultiplier;

            // Add mesh components
            MeshFilter fragmentMeshFilter = fragment.AddComponent<MeshFilter>();
            MeshRenderer fragmentRenderer = fragment.AddComponent<MeshRenderer>();

            // Use the original mesh and material
            fragmentMeshFilter.mesh = originalMesh;
            fragmentRenderer.material = originalMaterial;

            // Generate a random offset within the object's bounds
            Vector3 randomOffset = new Vector3(
                Random.Range(-bounds.extents.x, bounds.extents.x) * 0.8f,
                Random.Range(-bounds.extents.y, bounds.extents.y) * 0.8f,
                Random.Range(-bounds.extents.z, bounds.extents.z) * 0.8f
            );

            // Apply the offset in world space
            fragment.transform.position += originalObject.transform.TransformDirection(randomOffset);

            // Add physics
            Rigidbody fragmentRb = fragment.AddComponent<Rigidbody>();
            fragmentRb.useGravity = false; // No gravity for space feel

            // Add random rotation
            fragment.transform.rotation = Quaternion.Euler(
                rotation.eulerAngles.x + Random.Range(-180f, 180f),
                rotation.eulerAngles.y + Random.Range(-180f, 180f),
                rotation.eulerAngles.z + Random.Range(-180f, 180f)
            );

            // Apply explosion force
            Vector3 randomDirection = Random.onUnitSphere;
            float randomForce = explosionForce * Random.Range(1f - velocityVariation, 1f + velocityVariation);
            fragmentRb.AddForce(randomDirection * randomForce, ForceMode.Impulse);

            // Add chaotic rotation (torque)
            fragmentRb.AddTorque(
                Random.Range(-torqueMultiplier, torqueMultiplier),
                Random.Range(-torqueMultiplier, torqueMultiplier),
                Random.Range(-torqueMultiplier, torqueMultiplier),
                ForceMode.Impulse
            );

            // Add self-destruct behavior
            DestroyAfterTime destroyScript = fragment.AddComponent<DestroyAfterTime>();
            destroyScript.lifetime = fragmentLifetime * Random.Range(0.8f, 1.2f); // Add variation to lifetime too
        }
    }

    // Optional: Add particle effects alongside the fragments
    public void AddExplosionParticles(Vector3 position, float size)
    {
        // You can instantiate a particle system here if you want additional effects
        if (explosionParticlePrefab != null)
        {
            GameObject particles = Instantiate(explosionParticlePrefab, position, Quaternion.identity);
            particles.transform.localScale = Vector3.one * size;
            ParticleSystem ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particles, 3f);
            }
        }
    }

    // Optional particle system prefab for additional effects
    public GameObject explosionParticlePrefab;
}

