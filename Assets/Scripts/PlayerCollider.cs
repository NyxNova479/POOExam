// Script pour le joueur
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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

