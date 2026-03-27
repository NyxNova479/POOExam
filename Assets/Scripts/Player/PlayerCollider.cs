// Script pour le joueur
using UnityEngine;

public class PlayerCollider : EntityColider
{




    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Asteroid"))
        {
            // Le joueur a été touché par un ennemi ou un astéroïde
            collision.gameObject.GetComponent<Dangers>().HandlePlayerHit(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            // Le joueur a collecté un power-up
            collision.gameObject.GetComponent<PowerUp>().ApplyPowerUp(gameManager.GetComponent<Bullets>(),gameManager.GetComponent<UIManager>());
            Destroy(collision.gameObject);
            collision.GetComponent<PowerUp>().PowerUps.Remove(collision.gameObject.GetComponent<PowerUp>());
        }
    }
}

