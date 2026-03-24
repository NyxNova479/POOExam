// Script pour les projectiles
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Balle touche ennemi
            gameManager.HandleBulletEnemyCollision(gameObject, collision.gameObject);
            gameManager.score += 100;

            // Chance de générer un power-up
            if (Random.value < 0.5f)
            {
                gameManager.SpawnPowerUp(collision.transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("Asteroid"))
        {
            // Balle touche astéroïde
            gameManager.HandleBulletEnemyCollision(gameObject, collision.gameObject);
            gameManager.score += 50;
        }
    }
}