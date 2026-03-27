using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public class Enemy : Dangers, IColidable
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private EnemyCollider enCollider;

    private float enemySpeed = 3.0f;

    public void collide(PlayerCollider player)
    {
        throw new System.NotImplementedException();
    }

    public override void Move(List<Dangers> dangers, PlayerShip player)
    {
        MoveEnemies(dangers, player);
    }

    public override void Spawn()
    {
        if (Time.time > nextSpawnTime)
        {
            if (Random.value < 0.3f)
            {
                // Spawn d'un ennemi
                float randomX = Random.Range(-8f, 8f);
                // Position de spawn sur l'axe Z au lieu de Y
                Vector3 spawnPosition = new Vector3(randomX, 0, 9);
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

                // Configuration des composants de collision pour l'ennemi
                enCollider.SetupCollisionComponents(enemy, true, false);

                // Ajouter le script de gestion de collision � l'ennemi
                enemy.AddComponent<EnemyCollider>();

                dangers.Add(enemy.GetComponent<Enemy>());
            }

            nextSpawnTime = Time.time + spawnRate;
        }

    }

    protected void MoveEnemies(List<Dangers> enemies, PlayerShip player)
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
                    gameManager.setLives(gameManager.getLives()-1);

                    // Effet visuel pour montrer que l'ennemi a travers�
                    if (player.getPlayerDamageEffect() != null)
                    {
                        Instantiate(player.getPlayerDamageEffect(), enemies[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ennemi
                    Destroy(enemies[i]);
                    enemies.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (gameManager.getLives() <= 0)
                    {
                        gameManager.GameOver();
                    }
                }
            }
            else
            {
                enemies.RemoveAt(i);
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
