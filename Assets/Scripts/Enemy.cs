using UnityEngine;

/*public class Enemy : MonoBehaviour
{
    private GameManager gameManager;
    private float initialSpawnRate = 2.0f; // Taux de spawn initial
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Le joueur a touché un enemy
            gameManager.HandlePlayerHit(gameObject);
        }
    }

    void MoveEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] != null)
            {
                // Utiliser le Rigidbody pour le mouvement
                Rigidbody rb = enemies[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Appliquer directement une v�locit� au Rigidbody
                    rb.linearVelocity = Vector3.back * enemySpeed;
                }
                else
                {
                    // Fallback au mouvement par transform si pas de Rigidbody
                    enemies[i].transform.position += Vector3.back * enemySpeed * Time.deltaTime;
                }

                // Les ennemis ne disparaissent qu'� z=-12 et enl�vent une vie
                if (enemies[i].transform.position.z < -12)
                {
                    // Enlever un point de vie au joueur
                    lives--;

                    // Effet visuel pour montrer que l'ennemi a travers�
                    if (playerDamageEffect != null)
                    {
                        Instantiate(playerDamageEffect, enemies[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ennemi
                    Destroy(enemies[i]);
                    enemies.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (lives <= 0)
                    {
                        GameOver();
                    }
                }
            }
            else
            {
                enemies.RemoveAt(i);
            }
        }
    }
*/