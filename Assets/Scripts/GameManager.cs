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

    private Dangers danger;

    private ISpawnable spawnable;
    private IMovable movable;
    private IShootable shootable;
    private PlayerShip player;

    [Header("Explosion")]
    public ExplosionManager explosionManager;

    // R�f�rence directe � tous les objets du jeu

    private GameObject explosionPrefab;
    private GameObject powerUpPrefab;

    // Variables publiques expos�es sans encapsulation
    private int score;
    private int lives;

    // Nouvelles variables pour les fonctionnalit�s demand�es
    [Header("Weapon Settings")]
    public int bulletCount = 1; // Nombre de projectiles tir�s simultan�ment
    public float bulletSpacing = 0.5f; // Espacement horizontal entre les projectiles
    public int maxBulletCount = 5; // Limite maximale de projectiles simultan�s


    private float gameTime = 0f; // Temps de jeu �coul�

    public float getGameTime()
    {
        return gameTime;
    }

    public int getScore()
    {
        return score;
    }
    public int getLives()
    {
        return lives;
    }

    public void setScore(int value)
    {
        score = value;
    }
    public void setLives(int value)
    {
        lives = value;
    }

    // Listes pour suivre tous les objets du jeu
    private List<Dangers> dangers = new List<Dangers>();
    private List<Bullets> bullets = new List<Bullets>();
    public List<GameObject> powerUps = new List<GameObject>();




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

    public List<Dangers> lDangers { get => dangers; set => dangers = value; }
    public List<Bullets> lBullets { get => bullets; set => bullets = value; }


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
            lDangers.Remove(hitObject.GetComponent<Dangers>());
        }
        else if (hitObject.CompareTag("Asteroid"))
        {
            Destroy(hitObject);
            lDangers.Remove(hitObject.GetComponent<Dangers>());
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

        gameTime = 0f;


        UpdateUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (powerupMessageText) powerupMessageText.gameObject.SetActive(false);

        // S'assurer que le joueur a les composants n�cessaires pour les collisions
        SetupCollisionComponents(player.getPrefab(), true, false, "Player");

        // Ajouter le script de gestion de collision au joueur
        if (player.GetComponent<PlayerCollider>() == null)
        {
            player.getPrefab().AddComponent<PlayerCollider>();
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
        dangers.Remove(enemy.GetComponent<Dangers>());

        // Destruction de la balle
        Destroy(bullet);
        lBullets.Remove(bullet.GetComponent<Bullets>());
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Augmentation du temps de jeu
            gameTime += Time.deltaTime;



            // Affichage du temps de jeu (optionnel)
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60);
                int seconds = Mathf.FloorToInt(gameTime % 60);
                timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
            }


            // D�placement de tous les objets
            MoveDangers();
            MoveBullets();

            // Nous ne v�rifions plus les collisions manuellement
            // Les collisions sont maintenant g�r�es par les �v�nements OnTriggerEnter/OnCollisionEnter

            // G�n�ration de nouveaux ennemis/ast�ro�des
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


    void FireBullet()
    {
        shootable.beShot(player);
    }

    void MoveDangers()
    {
        movable.beMoved(this);
    }

 
    

    void MoveBullets()
    {
        movable.beMoved(this);
    }

    void SpawnEnemiesAndAsteroids()
    {
        spawnable.beSpawned(this);
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

    public void GameOver()
    {
        // Affichage du panel de game over
        gameOverPanel.SetActive(true);

        // Initialisation du compte � rebours
        isGameOver = true;
        restartCountdown = 3.0f;

        // Mise � jour initiale du texte de d�compte
        if (countdownText != null)
        {
            countdownText.text = "Redémarrage dans: " + Mathf.Ceil(restartCountdown).ToString();
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
        foreach (Dangers enemy in lDangers)
        {
            Destroy(enemy);
        }
        lDangers.Clear();

        foreach (Dangers asteroid in lDangers)
        {
            Destroy(asteroid);
        }
        lDangers.Clear();

        foreach (Bullets bullet in lBullets)
        {
            Destroy(bullet);
        }
        lBullets.Clear();

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
        danger.setSpawnRate(danger.getInitialSpawnRate());
        danger.setNextSpawnTime(Time.time + danger.getSpawnRate());

        // Masquage du panel de game over
        gameOverPanel.SetActive(false);

        // Replacement du joueur
        player.setPosition(new Vector3(0, 0, -7));
        player.setRotation(Quaternion.identity);
    }
}