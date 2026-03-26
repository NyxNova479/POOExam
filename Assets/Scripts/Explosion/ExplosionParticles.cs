using UnityEngine;

public class ExplosionParticles : MonoBehaviour
{
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
    private GameObject explosionParticlePrefab;
}
