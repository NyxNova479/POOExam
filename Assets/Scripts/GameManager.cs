// Le fichier GameManager.cs - Une classe monolithique qui fait tout
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.VisualScripting.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameManager : MonoBehaviour
{

    private static GameManager Instance;



    public GameManager getInstance()
    {
        if(Instance == null)
        {
            Instance = new GameManager();
        }
        return Instance;
    }

    private ISpawnable spawnable;
    private IMovable movable;

    [Header("Explosion")]
    public ExplosionManager explosionManager;

    // R�f�rence directe � tous les objets du jeu
    private GameObject playerShip;
    private GameObject enemyPrefab;
    private GameObject asteroidPrefab;
    private GameObject bulletPrefab;
    private GameObject explosionPrefab;
    private GameObject powerUpPrefab;

    // Variables publiques expos�es sans encapsulation
    public int score;
    public int lives;
    public float playerSpeed = 5.0f;
    public float bulletSpeed = 10.0f;
    public float enemySpeed = 3.0f;
    public float asteroidSpeed = 2.0f;
    public float spawnRate = 2.0f;

    // Nouvelles variables pour les fonctionnalit�s demand�es
    [Header("Weapon Settings")]
    public int bulletCount = 1; // Nombre de projectiles tir�s simultan�ment
    public float bulletSpacing = 0.5f; // Espacement horizontal entre les projectiles
    public int maxBulletCount = 5; // Limite maximale de projectiles simultan�s

    [Header("Difficulty Settings")]
    public float initialSpawnRate = 2.0f; // Taux de spawn initial
    public float minSpawnRate = 0.5f; // Taux de spawn minimal (plus difficile)
    public float spawnRateDifficulty = 0.1f; // R�duction du taux de spawn par minute
    private float gameTime = 0f; // Temps de jeu �coul�

    public float getGameTime()
    {
        return gameTime;
    }

    // Listes pour suivre tous les objets du jeu
    private List<Dangers> enemies = new List<Dangers>();
    private List<Dangers> asteroids = new List<Dangers>();
    private List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> powerUps = new List<GameObject>();


    public List<Dangers> getEnemies()
    {
        return Enemies;
    }

    // Variables pour le timing
    private float nextSpawnTime;


    // UI references
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text livesText;
    public GameObject gameOverPanel;
    public TMPro.TMP_Text powerupMessageText; // Pour afficher les messages de powerup
    public TMPro.TMP_Text timeText; // Pour afficher le temps �coul�
    public GameObject playerDamageEffect; // Effet visuel quand un ennemi traverse

    private bool isGameOver = false;
    private float restartCountdown = 3.0f;
    public TMPro.TMP_Text countdownText;

    public List<Dangers> Enemies { get => enemies; set => enemies = value; }
    public List<Dangers> Asteroids { get => asteroids; set => asteroids = value; }


    // Avant de remplacer le syst�me de collisions, il faut cr�er des classes pour g�rer les collisions
    // Ces classes seront attach�es aux objets du jeu concern�s

    // Voici les scripts � cr�er pour le syst�me de trigger/collision Unity
    // Note pour les �tudiants : Ces scripts devraient �tre dans des fichiers s�par�s pour respecter les principes SOLID










    // M�thode pour g�rer les collisions avec le joueur
    public void HandlePlayerHit(GameObject hitObject)
    {
        // Destruction de l'objet qui a touch� le joueur
        Instantiate(explosionPrefab, hitObject.transform.position, Quaternion.identity);

        if (hitObject.CompareTag("Enemy"))
        {
            Destroy(hitObject);
            Enemies.Remove(hitObject);
        }
        else if (hitObject.CompareTag("Asteroid"))
        {
            Destroy(hitObject);
            Asteroids.Remove(hitObject);
        }

        // Perte d'une vie
        lives--;

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void Start()
    {
        // Initialisation
        score = 0;
        lives = 3;
        bulletCount = 1;
        gameTime = 0f;
        spawnRate = initialSpawnRate;
        nextSpawnTime = Time.time + spawnRate;
        UpdateUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (powerupMessageText) powerupMessageText.gameObject.SetActive(false);

        // S'assurer que le joueur a les composants n�cessaires pour les collisions
        SetupCollisionComponents(playerShip, true, false, "Player");

        // Ajouter le script de gestion de collision au joueur
        if (playerShip.GetComponent<PlayerCollider>() == null)
        {
            playerShip.AddComponent<PlayerCollider>();
        }
    }

    // Nouvelle m�thode pour configurer les composants de collision
    void SetupCollisionComponents(GameObject obj, bool hasRigidbody, bool isTrigger, string tag)
    {
        // Ajouter ou configurer le collider si n�cessaire
        Collider collider = obj.GetComponent<Collider>();
        if (collider == null)
        {
            // Ajouter un BoxCollider par d�faut
            collider = obj.AddComponent<BoxCollider>();

            // Ajuster la taille du collider en fonction du tag
            BoxCollider boxCollider = (BoxCollider)collider;
            if (tag == "Bullet")
            {
                // Collider plus petit pour les balles
                boxCollider.size = new Vector3(0.3f, 0.3f, 0.5f);
            }
            else if (tag == "PowerUp")
            {
                // Collider plus grand pour les power-ups pour faciliter leur collecte
                boxCollider.size = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }

        // Configurer le collider comme trigger ou non
        collider.isTrigger = isTrigger;

        // Ajouter un Rigidbody si n�cessaire
        if (hasRigidbody && obj.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false; // D�sactiver la gravit� pour un jeu spatial
            rb.isKinematic = false; // Ne pas rendre kin�matique pour permettre les collisions physiques
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; // Figer certains axes
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // D�finir le tag
        obj.tag = tag;
    }

    public void HandleBulletEnemyCollision(GameObject bullet, GameObject enemy)
    {
        // Explosion avec effet de fragmentation
        if (explosionManager != null)
        {
            explosionManager.ExplodeObject(enemy);
        }
        else
        {
            // Fallback vers l'explosion originale
            Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        }

        // Destruction de l'ennemi
        Destroy(enemy, 0.1f); // Court d�lai pour permettre � l'explosion de commencer
        Enemies.Remove(enemy);

        // Destruction de la balle
        Destroy(bullet);
        bullets.Remove(bullet);
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Augmentation du temps de jeu
            gameTime += Time.deltaTime;

            // Calcul du nouveau taux de spawn en fonction du temps �coul� (en minutes)
            float minutesPlayed = gameTime / 2f;
            spawnRate = Mathf.Max(minSpawnRate, initialSpawnRate - (spawnRateDifficulty * minutesPlayed));

            // Affichage du temps de jeu (optionnel)
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60);
                int seconds = Mathf.FloorToInt(gameTime % 60);
                timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
            }

            // Gestion des entr�es du joueur
            HandlePlayerInput();

            // D�placement de tous les objets
            MoveEnemies();
            MoveAsteroids();
            MoveBullets();

            // Nous ne v�rifions plus les collisions manuellement
            // Les collisions sont maintenant g�r�es par les �v�nements OnTriggerEnter/OnCollisionEnter

            // G�n�ration de nouveaux ennemis/ast�ro�des
            spawnable.beSpawned(this);
            SpawnEnemiesAndAsteroids();

            // Mise � jour de l'UI
            UpdateUI();
        }

        // Gestion du d�compte de red�marrage
        if (isGameOver)
        {
            restartCountdown -= Time.deltaTime;

            // Mise � jour du texte avec la valeur arrondie � l'entier sup�rieur
            if (countdownText != null)
            {
                countdownText.text = "Red�marrage dans: " + Mathf.Ceil(restartCountdown).ToString();
            }

            // Lorsque le d�compte atteint z�ro
            if (restartCountdown <= 0)
            {
                RestartGame();
            }
        }
    }

    void HandlePlayerInput()
    {
        // D�placement du joueur
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // D�placement sur le plan XZ
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * playerSpeed * Time.deltaTime;
        playerShip.transform.position += movement;

        // Calcul des angles de rotation pour les deux axes
        float tiltAngleZ = -horizontalInput * 30f; // Inclinaison lat�rale (gauche/droite)
        float tiltAngleX = verticalInput * 15f;    // Inclinaison longitudinale (avant/arri�re)

        // Cr�ation d'une rotation qui combine les deux inclinaisons
        Quaternion targetRotation = Quaternion.Euler(tiltAngleX, 0, tiltAngleZ);

        // Application de la rotation avec un lissage pour un effet plus naturel
        playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, targetRotation, 5f * Time.deltaTime);

        // Si aucun input, retour progressif � la rotation neutre
        if (horizontalInput == 0 && verticalInput == 0)
        {
            playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.identity, 5f * Time.deltaTime);
        }

        // Limites de l'�cran pour le joueur
        Vector3 playerPos = playerShip.transform.position;
        playerPos.x = Mathf.Clamp(playerPos.x, -8.4f, 8.4f);
        playerPos.z = Mathf.Clamp(playerPos.z, -11, -2.5f);
        playerShip.transform.position = playerPos;

        // Tir
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        // Calcul de la position de d�part pour centrer les projectiles
        float startX = -((bulletCount - 1) * bulletSpacing) / 2;

        // Cr�ation de plusieurs balles c�te � c�te
        for (int i = 0; i < bulletCount; i++)
        {
            // Calcule la position avec l'offset horizontal
            Vector3 bulletOffset = new Vector3(startX + (i * bulletSpacing), -0.5f, 0.5f);
            Vector3 spawnPosition = playerShip.transform.position + bulletOffset;

            // Instanciation du projectile
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // Configuration des composants de collision pour la balle
            // Les projectiles doivent avoir un Rigidbody pour les collisions
            SetupCollisionComponents(bullet, true, false, "Bullet");

            // Ajouter le script de gestion de collision � la balle
            bullet.AddComponent<BulletCollider>();

            bullets.Add(bullet);
        }

        // Son de tir
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    void MoveEnemies()
    {
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            if (Enemies[i] != null)
            {
                // Utiliser le Rigidbody pour le mouvement
                Rigidbody rb = Enemies[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Appliquer directement une v�locit� au Rigidbody
                    rb.linearVelocity = Vector3.back * enemySpeed;
                }
                else
                {
                    // Fallback au mouvement par transform si pas de Rigidbody
                    Enemies[i].transform.position += Vector3.back * enemySpeed * Time.deltaTime;
                }

                // Les ennemis ne disparaissent qu'� z=-12 et enl�vent une vie
                if (Enemies[i].transform.position.z < -12)
                {
                    // Enlever un point de vie au joueur
                    lives--;

                    // Effet visuel pour montrer que l'ennemi a travers�
                    if (playerDamageEffect != null)
                    {
                        Instantiate(playerDamageEffect, Enemies[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ennemi
                    Destroy(Enemies[i]);
                    Enemies.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (lives <= 0)
                    {
                        GameOver();
                    }
                }
            }
            else
            {
                Enemies.RemoveAt(i);
            }
        }
    }

    void MoveAsteroids()
    {
        for (int i = Asteroids.Count - 1; i >= 0; i--)
        {
            if (Asteroids[i] != null)
            {
                // Direction al�atoire pour chaque ast�ro�de
                float randomX = Random.Range(-0.5f, 0.5f);

                // Utiliser le Rigidbody pour le mouvement
                Rigidbody rb = Asteroids[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Appliquer directement une v�locit� au Rigidbody
                    rb.linearVelocity = new Vector3(randomX, 0, -1) * asteroidSpeed;

                    // Appliquer une rotation
                    Asteroids[i].transform.Rotate(0, 30 * Time.deltaTime, 0);
                }
                else
                {
                    // Fallback au mouvement par transform si pas de Rigidbody
                    Vector3 movement = new Vector3(randomX, 0, -1) * asteroidSpeed * Time.deltaTime;
                    Asteroids[i].transform.position += movement;
                    Asteroids[i].transform.Rotate(0, 30 * Time.deltaTime, 0);
                }

                // Les ast�ro�des ne disparaissent qu'� z=-12 et enl�vent une vie
                if (Asteroids[i].transform.position.z < -12)
                {
                    // Enlever un point de vie au joueur
                    lives--;

                    // Effet visuel pour montrer que l'ast�ro�de a travers�
                    if (playerDamageEffect != null)
                    {
                        Instantiate(playerDamageEffect, Asteroids[i].transform.position, Quaternion.identity);
                    }

                    // Destruction de l'ast�ro�de
                    Destroy(Asteroids[i]);
                    Asteroids.RemoveAt(i);

                    // V�rifier si le joueur n'a plus de vies
                    if (lives <= 0)
                    {
                        GameOver();
                    }
                }
            }
            else
            {
                Asteroids.RemoveAt(i);
            }
        }
    }

    void MoveBullets()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (bullets[i] != null)
            {
                // Ajouter des forces au Rigidbody au lieu de d�placer la Transform
                Rigidbody rb = bullets[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // R�initialiser la v�locit� et appliquer une nouvelle force
                    rb.linearVelocity = Vector3.forward * bulletSpeed;
                }
                else
                {
                    // Fallback au mouvement par transform si pas de Rigidbody
                    bullets[i].transform.position += Vector3.forward * bulletSpeed * Time.deltaTime;
                }

                // Suppression des balles qui sortent de l'�cran
                if (bullets[i].transform.position.z > 9) // Chang� de y � z
                {
                    Destroy(bullets[i]);
                    bullets.RemoveAt(i);
                }
            }
            else
            {
                bullets.RemoveAt(i);
            }
        }
    }

    void SpawnEnemiesAndAsteroids()
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

                Enemies.Add(enemy);
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

                Asteroids.Add(asteroid);
            }

            nextSpawnTime = Time.time + spawnRate;
        }
    }

    public void SpawnPowerUp(Vector3 position)
    {
        GameObject powerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);

        // Configuration des composants de collision pour le power-up
        SetupCollisionComponents(powerUp, true, false, "PowerUp");

        // Ajouter le script de gestion de collision au power-up
        powerUp.AddComponent<PowerUpCollider>();

        powerUps.Add(powerUp);
    }

    public void ApplyPowerUp()
    {
        // Augmenter le nombre de projectiles pour tous les power-ups
        if (bulletCount < maxBulletCount)
        {
            bulletCount++;

            // Affichage d'un message temporaire pour informer le joueur
            StartCoroutine(ShowPowerupMessage("Weapon Upgraded! Bullets: " + bulletCount));
        }
        else
        {
            // Bonus de score si le joueur a d�j� le maximum de projectiles
            score += 200;
            StartCoroutine(ShowPowerupMessage("Max Weapon Level! +200 Score"));
        }
    }

    // Coroutine pour afficher un message temporaire
    IEnumerator ShowPowerupMessage(string message)
    {
        if (powerupMessageText != null)
        {
            powerupMessageText.text = message;
            powerupMessageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            powerupMessageText.gameObject.SetActive(false);
        }
        yield return null;
    }

    void UpdateUI()
    {
        // Mise � jour des textes de score et de vies
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }

    void GameOver()
    {
        // Affichage du panel de game over
        gameOverPanel.SetActive(true);

        // Initialisation du compte � rebours
        isGameOver = true;
        restartCountdown = 3.0f;

        // Mise � jour initiale du texte de d�compte
        if (countdownText != null)
        {
            countdownText.text = "Red�marrage dans: " + Mathf.Ceil(restartCountdown).ToString();
            countdownText.gameObject.SetActive(true);
        }

        // Note: ne pas arr�ter le temps ici puisque nous voulons que le d�compte fonctionne
        // Time.timeScale = 0; -- retirez cette ligne s'il elle est pr�sente
    }

    public void RestartGame()
    {
        // R�initialisation du statut de game over
        isGameOver = false;

        // Masquage du texte de d�compte
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Remise � z�ro du jeu
        Time.timeScale = 1;

        // Destruction de tous les objets
        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }
        Enemies.Clear();

        foreach (GameObject asteroid in Asteroids)
        {
            Destroy(asteroid);
        }
        Asteroids.Clear();

        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        bullets.Clear();

        foreach (GameObject powerUp in powerUps)
        {
            Destroy(powerUp);
        }
        powerUps.Clear();

        // R�initialisation des variables
        score = 0;
        lives = 3;
        bulletCount = 1;
        gameTime = 0f;
        spawnRate = initialSpawnRate;
        nextSpawnTime = Time.time + spawnRate;

        // Masquage du panel de game over
        gameOverPanel.SetActive(false);

        // Replacement du joueur
        playerShip.transform.position = new Vector3(0, 0, -7);
        playerShip.transform.rotation = Quaternion.identity;
    }
}