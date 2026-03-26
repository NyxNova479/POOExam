using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public abstract class Dangers : Entity, IMovable, ISpawnable
{

    private GameObject explosionPrefab;
    protected float spawnRate = 2.0f;


    [Header("Difficulty Settings")]
    protected float initialSpawnRate = 2.0f; // Taux de spawn initial
    protected float minSpawnRate = 0.5f; // Taux de spawn minimal (plus difficile)
    protected float spawnRateDifficulty = 0.1f; // R�duction du taux de spawn par minute

    // Variables pour le timing
    protected float nextSpawnTime;


    public float getInitialSpawnRate()
    {
        return initialSpawnRate;
    }

    public float getSpawnRate()
    {
        return spawnRate;
    }

    public void setSpawnRate(float value)
    {
        spawnRate = value;
    }

    public void setNextSpawnTime(float value)
    {
        nextSpawnTime = value;
    }

    void Start()
    {
        gameManager = GameManager.getInstance();
        spawnRate = initialSpawnRate;
        nextSpawnTime = Time.time + spawnRate;
    }

    private void Update()
    {
        float gameTime = gameManager.getGameTime();

        float minutesPlayed = gameTime / 2f;
        spawnRate = Mathf.Max(minSpawnRate, initialSpawnRate - (spawnRateDifficulty * minutesPlayed));
    }


    public abstract void Move(List<Dangers> dangers, PlayerShip playerShip);
    public abstract void Spawn();


    
    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Le joueur a touché un enemy
            HandlePlayerHit(gameObject);
        }
    }

    
    public void beMoved(GameManager gameManager)
    {
        Move(gameManager.lDangers, gameManager.getPlayer());
    }

    public void beSpawned(GameManager gameManager)
    {
        Spawn();
    }
    // Méthode pour gérer les collisions avec le joueur
    public void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touché le joueur
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
}
