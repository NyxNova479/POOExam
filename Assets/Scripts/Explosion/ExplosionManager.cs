using UnityEngine;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour
{

    [SerializeField] FragmentsCreator fragmentsCreator;

    [Header("Explosion Settings")]
    private float explosionForce = 15f;
    private float explosionRadius = 5f;
    private float upwardsModifier = 1f;
    private Vector2 fragmentsRange = new Vector2(5, 8); // Min and max number of fragments



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
        fragmentsCreator.CreateRandomFragments(objectToExplode, originalMesh, originalMaterial, originalPosition,
                             originalRotation, originalScale, fragmentsContainer.transform, fragmentCount);

        // Disable the original object renderer and collider
        meshRenderer.enabled = false;

        Collider objCollider = objectToExplode.GetComponent<Collider>();
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }

        // Set the container to be destroyed when all fragments are gone
        Destroy(fragmentsContainer, fragmentsCreator.getFragmentsLifetime() + 0.2f);

        // Destroy the original object after a delay
        Destroy(objectToExplode, fragmentsCreator.getFragmentsLifetime() + 0.1f);
    }

    


}

