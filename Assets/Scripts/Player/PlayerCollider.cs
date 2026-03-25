// Script pour le joueur
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touch� le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);

        if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            Enemies.Remove(hitObject);
        }
        else if (hitObject.CompareTag("Asteroid"))
        {
            Destroy(hitObject);
            Asteroids.Remove(hitObject);
        }

        // Perte d'une vie
        lives--;

        if (lives <= 0)
        {
            GameOver();
        }
    }

    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Asteroid"))
        {
            // Le joueur a été touché par un ennemi ou un astéroïde
            gameManager.HandlePlayerHit(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            // Le joueur a collecté un power-up
            gameManager.ApplyPowerUp();
            Destroy(collision.gameObject);
            gameManager.powerUps.Remove(collision.gameObject);
        }
    }
}

