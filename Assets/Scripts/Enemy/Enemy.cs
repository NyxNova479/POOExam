using UnityEngine;

public abstract class Enemy : Entity, IMovable, ISpawnable
{
    protected GameManager gameManager;
    protected float initialSpawnRate = 2.0f; // Taux de spawn initial
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

    public abstract void Move();

    public void beMoved()
    {
        Move();
    }

    public void abstract void Spawn()
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
                SetupCollisionComponents(enemy, true, false, "Enemy");

                // Ajouter le script de gestion de collision � l'ennemi
                enemy.AddComponent<EnemyCollider>();

                enemies.Add(enemy);
            }
            else
            {
                // Spawn d'un ast�ro�de
                float randomX = Random.Range(-8f, 8f);
                // Position de spawn sur l'axe Z au lieu de Y
                Vector3 spawnPosition = new Vector3(randomX, 0, 9);
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

                // Configuration des composants de collision pour l'ast�ro�de
                SetupCollisionComponents(asteroid, true, false, "Asteroid");

                // Ajouter le script de gestion de collision � l'ast�ro�de
                asteroid.AddComponent<AsteroidCollider>();

                asteroids.Add(asteroid);
            }

            nextSpawnTime = Time.time + spawnRate;
        }
    }

    public void beSpawned(GameManager gameManager)
    {
        Spawn();
    }
}
