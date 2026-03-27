using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Asteroids : Dangers, ISpawnable
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private AsteroidCollider astCollider;

    private float asteroidSpeed = 2.0f;

    public override void Move(List<Dangers> dangers, PlayerShip player)
    {
        MoveAsteroids(dangers, player);
    }

    public override void Spawn()
    {


            if (Random.value >= 0.3f)
            {
                // Spawn d'un ast�ro�de
                float randomX = Random.Range(-8f, 8f);
                // Position de spawn sur l'axe Z au lieu de Y
                Vector3 spawnPosition = new Vector3(randomX, 0, 9);
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

                // Ajouter le script de gestion de collision � l'ast�ro�de
                asteroid.AddComponent<AsteroidCollider>();
                // Configuration des composants de collision pour l'ast�ro�de
                astCollider = gameObject.GetComponent<AsteroidCollider>();
                astCollider.SetupCollisionComponents(asteroid, true, false);


                dangers.Add(asteroid.GetComponent<Asteroids>());
            }
            nextSpawnTime = Time.time + spawnRate;
        
    }


    protected void MoveAsteroids(List<Dangers> asteroids, PlayerShip player)
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
                    gameManager.setLives(gameManager.getLives() - 1);

                    // Effet visuel pour montrer que l'ast�ro�de a travers�
                    if (player.getPlayerDamageEffect() != null)
                    {
                        Instantiate(player.getPlayerDamageEffect(), asteroids[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ast�ro�de
                    Destroy(asteroids[i]);
                    asteroids.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (gameManager.getLives() <= 0)
                    {
                        gameManager.GameOver();
                    }
                }
            }
            else
            {
                asteroids.RemoveAt(i);
            }
        }
    }
    public void HandlePlayerHit(GameObject hitObject, GameObject explosionPrefab)
    {
        // Destruction de l'objet qui a touch� le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);
        Destroy(hitObject);
        lDangers.Remove(hitObject.GetComponent<Dangers>());
        base.HandlePlayerHit(hitObject);


        
    }
}
