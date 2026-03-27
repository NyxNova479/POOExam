using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Dangers : Entity, IMovable, ISpawnable
{

    [SerializeField] private GameObject explosionPrefab;
    protected float spawnRate = 2.0f;
    protected List<Dangers> dangers = new List<Dangers>();

    [Header("Difficulty Settings")]
    protected float initialSpawnRate = 2.0f; // Taux de spawn initial
    protected float minSpawnRate = 0.5f; // Taux de spawn minimal (plus difficile)
    protected float spawnRateDifficulty = 0.1f; // R�duction du taux de spawn par minute

    // Variables pour le timing
    protected float nextSpawnTime;

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
        Debug.Log("Je spawn");
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

    public void beSpawned()
    {
        Spawn();
    }
    // Méthode pour gérer les collisions avec le joueur
    public virtual void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touché le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);

        if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            dangers.Remove(hitObject.GetComponent<Dangers>());
        }
        else if (hitObject.CompareTag("Asteroid"))
        {
            Destroy(hitObject);
            dangers.Remove(hitObject.GetComponent<Dangers>());
        }

        // Perte d'une vie
        gameManager.setLives(gameManager.getLives() - 1);

        if (gameManager.getLives() <= 0)
        {
            gameManager.GameOver();
        }
    }
}
