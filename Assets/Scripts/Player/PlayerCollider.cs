// Script pour le joueur
using UnityEngine;

public class PlayerCollider : EntityColider
{


    public void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touch� le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);

        if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            gameManager.lDangers.Remove(hitObject.GetComponent<Dangers>());
        }
        else if (hitObject.CompareTag("Asteroid"))
        {
            Destroy(hitObject);
            gameManager.lDangers.Remove(hitObject.GetComponent<Dangers>());
        }

        // Perte d'une vie
        gameManager.setLives(gameManager.getLives() - 1);

        if (gameManager.getLives() <= 0)
        {
            gameManager.GameOver();
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

