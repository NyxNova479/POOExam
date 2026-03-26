using UnityEngine;

public class FragmentsCreator : MonoBehaviour 
{
    [Header("Chaos Settings")]
    private float torqueMultiplier = 200f;
    private float scaleVariation = 0.3f;
    private float velocityVariation = 0.5f;

    private void CreateRandomFragments(GameObject originalObject, Mesh originalMesh, Material originalMaterial,Vector3 position, Quaternion rotation, Vector3 scale, Transform containerTransform, int fragmentCount)
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
}
