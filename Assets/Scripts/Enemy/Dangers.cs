using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public abstract class Dangers : Entity, IMovable, ISpawnable
{
    protected GameManager gameManager;

    protected float spawnRate = 2.0f;


    [Header("Difficulty Settings")]
    protected float initialSpawnRate = 2.0f; // Taux de spawn initial
    protected float minSpawnRate = 0.5f; // Taux de spawn minimal (plus difficile)
    protected float spawnRateDifficulty = 0.1f; // R�duction du taux de spawn par minute

    // Variables pour le timing
    protected float nextSpawnTime;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spawnRate = initialSpawnRate;
        nextSpawnTime = Time.time + spawnRate;
    }

    private void Update()
    {
        float gameTime = gameManager.getGameTime();

        float minutesPlayed = gameTime / 2f;
        spawnRate = Mathf.Max(minSpawnRate, initialSpawnRate - (spawnRateDifficulty * minutesPlayed));
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

    public abstract void Move(List<Dangers> dangers);

    public void beMoved(GameManager gameManager)
    {
        Move(gameManager.lDangers);
    }

    public abstract void Spawn();
    

    

    public void beSpawned(GameManager gameManager)
    {
        Spawn();
    }
}
