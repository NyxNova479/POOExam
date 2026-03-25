// Script pour les astéroïdes
using UnityEngine;

public class AsteroidCollider : Enemy
{
    private GameObject asteroidPrefab;

    private float asteroidSpeed = 2.0f;

    public override void Move()
    {
        MoveAsteroids();
    }

    protected void MoveAsteroids()
    {
        for (int i = asteroids.Count - 1; i >= 0; i--)
        {
            if (asteroids[i] != null)
            {
                // Direction al�atoire pour chaque ast�ro�de
                float randomX = Random.Range(-0.5f, 0.5f);

                // Utiliser le Rigidbody pour le mouvement
                Rigidbody rb = asteroids[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Appliquer directement une v�locit� au Rigidbody
                    rb.linearVelocity = new Vector3(randomX, 0, -1) * asteroidSpeed;

                    // Appliquer une rotation
                    asteroids[i].transform.Rotate(0, 30 * Time.deltaTime, 0);
                }
                else
                {
                    // Fallback au mouvement par transform si pas de Rigidbody
                    Vector3 movement = new Vector3(randomX, 0, -1) * asteroidSpeed * Time.deltaTime;
                    asteroids[i].transform.position += movement;
                    asteroids[i].transform.Rotate(0, 30 * Time.deltaTime, 0);
                }

                // Les ast�ro�des ne disparaissent qu'� z=-12 et enl�vent une vie
                if (asteroids[i].transform.position.z < -12)
                {
                    // Enlever un point de vie au joueur
                    lives--;

                    // Effet visuel pour montrer que l'ast�ro�de a travers�
                    if (playerDamageEffect != null)
                    {
                        Instantiate(playerDamageEffect, asteroids[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ast�ro�de
                    Destroy(asteroids[i]);
                    asteroids.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (lives <= 0)
                    {
                        GameOver();
                    }
                }
            }
            else
            {
                asteroids.RemoveAt(i);
            }
        }
    }
}