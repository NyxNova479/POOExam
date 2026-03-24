// Script pour les astéroïdes
using UnityEngine;

public class AsteroidCollider : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Le joueur a touché un astéroïde
            gameManager.HandlePlayerHit(gameObject);
        }
    }
}