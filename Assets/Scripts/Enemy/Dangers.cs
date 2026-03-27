using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Dangers : Entity, IMovable,ISpawnable
{

    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Asteroids asteroid;

    protected float spawnRate = 2.0f;
    protected List<Dangers> dangers = new List<Dangers>();

    [Header("Difficulty Settings")]
    protected float initialSpawnRate = 2.0f; // Taux de spawn initial
    protected float minSpawnRate = 0.5f; // Taux de spawn minimal (plus difficile)
    protected float spawnRateDifficulty = 0.1f; // R�duction du taux de spawn par minute

    // Variables pour le timing
    protected float nextSpawnTime;

    private ISpawnable spawnable;

    public List<Dangers> lDangers { get => dangers; set => dangers = value; }

    public float getInitialSpawnRate()
    {
        return initialSpawnRate;
    }

    public float getSpawnRate()
    {
        return spawnRate;
    }

    public GameObject getExplosionPrefab()
    {
        return explosionPrefab;
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


    public virtual void Move(List<Dangers> dangers, PlayerShip playerShip)
    {
        Debug.Log("Je bouge");
    }

    public virtual void Spawn()
    {
        if (Time.time > nextSpawnTime)
        {
            gameManager.Spawnable = enemy;
            gameManager.Spawnable.beSpawned();
        }
        else if (Time.time <= nextSpawnTime)
        {
            gameManager.Spawnable = asteroid;
            gameManager.Spawnable.beSpawned();
        }

    }
            public void beSpawned()
            {
                Spawn();
            }


    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Le joueur a touché un enemy
            HandlePlayerHit(gameObject);
        }
    }

    
    public void beMoved()
    {
        Move(dangers, gameManager.getPlayer());
    }


    // Méthode pour gérer les collisions avec le joueur
    public virtual void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touché le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);

        // Perte d'une vie
        gameManager.setLives(gameManager.getLives() - 1);

        if (gameManager.getLives() <= 0)
        {
            gameManager.GameOver();
        }
    }
}
