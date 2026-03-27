// Le fichier GameManager.cs - Une classe monolithique qui fait tout
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Unity.VisualScripting.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameManager : MonoBehaviour
{

    private static GameManager Instance;

    [SerializeField] private PlayerShip player;
    [SerializeField] private Bullets bulletScript;
    [SerializeField] private UIManager uiManager;

    private Dangers dangerScript;
    private PowerUp powerUp;


    private ISpawnable spawnable;
    private IMovable movable;
    private IShootable shootable;

    [Header("Explosion")]
    private ExplosionManager explosionManager;

    // Référence directe à tous les objets du jeu

    private GameObject explosionPrefab;


    // Variables publiques expos�es sans encapsulation
    private int score;
    private int lives;

    private float gameTime = 0f; // Temps de jeu écoulé




    public static GameManager getInstance()
    {
        if(Instance == null)
        {
            Instance = new GameManager();
        }
        return Instance;
    }





    public PlayerShip getPlayer()
    {
        return player;
    }

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
        if (lives > 3) lives = 3;
        else if (lives < 0) lives = 0;
    }










    // UI references
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text livesText;
    public GameObject gameOverPanel;
    public TMPro.TMP_Text powerupMessageText; // Pour afficher les messages de powerup
    public TMPro.TMP_Text timeText; // Pour afficher le temps �coul�


    private bool isGameOver = false;
    private float restartCountdown = 3.0f;
    public TMPro.TMP_Text countdownText;






    // Avant de remplacer le système de collisions, il faut créer des classes pour gérer les collisions
    // Ces classes seront attach�es aux objets du jeu concernés

    // Voici les scripts à créer pour le système de trigger/collision Unity
    // Note pour les étudiants : Ces scripts devraient être dans des fichiers s�par�s pour respecter les principes SOLID



    // Méthode pour gérer les collisions avec le joueur
    public void HandlePlayerHit(GameObject hitObject)
    {
        return;
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
        //SetupCollisionComponents(player.getPrefab(), true, false, "Player");

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
        dangerScript.lDangers.Remove(enemy.GetComponent<Dangers>());

        // Destruction de la balle
        Destroy(bullet);
        bulletScript.lBullets.Remove(bullet.GetComponent<Bullets>());
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Augmentation du temps de jeu
            gameTime += Time.deltaTime;



            // Déplacement de tous les objets
            MoveDangers();
            MoveBullets();

            // Nous ne v�rifions plus les collisions manuellement
            // Les collisions sont maintenant g�r�es par les �v�nements OnTriggerEnter/OnCollisionEnter

            // Génération de nouveaux ennemis/ast�ro�des
            SpawnEnemiesAndAsteroids();


        }

        // Gestion du décompte de red�marrage
        if (isGameOver)
        {
            restartCountdown -= Time.deltaTime;

            // Mise à jour du texte avec la valeur arrondie à l'entier supérieur
            if (countdownText != null)
            {
                countdownText.text = "Redémarrage dans: " + Mathf.Ceil(restartCountdown).ToString();
            }

            // Lorsque le décompte atteint zéro
            if (restartCountdown <= 0)
            {
                RestartGame();
            }
        }
    }



    void MoveDangers()
    {
        foreach (Dangers danger in dangerScript.lDangers)
        {

            movable = danger;
            movable.beMoved(this);
        }
    }


    void MoveBullets()
    {
        foreach(Bullets bullet in bulletScript.lBullets)
        {

            movable = bullet;
            movable.beMoved(this);
        }

    }

    void SpawnEnemiesAndAsteroids()
    {
       foreach(Dangers dangers in dangerScript.lDangers)
       {
            spawnable = dangers;
            spawnable.beSpawned(this);
       }
    }

    public void SpawnPowerUp(Vector3 position)
    {
        return;
    }

    public void ApplyPowerUp()
    {
        return;
    }





    void UpdateUI()
    {
        // Mise à jour des textes de score et de vies
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

        // Initialisation du compte à rebours
        isGameOver = true;
        restartCountdown = 3.0f;

        // Mise à jour initiale du texte de décompte
        if (countdownText != null)
        {
            countdownText.text = "Redémarrage dans: " + Mathf.Ceil(restartCountdown).ToString();
            countdownText.gameObject.SetActive(true);
        }

        // Note: ne pas arrêter le temps ici puisque nous voulons que le décompte fonctionne
        // Time.timeScale = 0; -- retirez cette ligne si elle est présente
    }

    public void RestartGame()
    {
        // Réinitialisation du statut de game over
        isGameOver = false;

        // Masquage du texte de décompte
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Remise à zéro du jeu
        Time.timeScale = 1;

        // Destruction de tous les objets
        foreach (Dangers enemy in dangerScript.lDangers)
        {
            Destroy(enemy);
        }
        dangerScript.lDangers.Clear();

        foreach (Dangers asteroid in dangerScript.lDangers)
        {
            Destroy(asteroid);
        }
        dangerScript.lDangers.Clear();

        foreach (Bullets bullet in bulletScript.lBullets)
        {
            Destroy(bullet);
        }
        bulletScript.lBullets.Clear();

        foreach (Entity powerUp in powerUp.PowerUps)
        {
            Destroy(powerUp);
        }
        powerUp.PowerUps.Clear();

        // R�initialisation des variables
        score = 0;
        lives = 3;
        bulletScript.BulletCount = 1;
        gameTime = 0f;
        dangerScript.setSpawnRate(dangerScript.getInitialSpawnRate());
        dangerScript.setNextSpawnTime(Time.time + dangerScript.getSpawnRate());

        // Masquage du panel de game over
        gameOverPanel.SetActive(false);

        // Replacement du joueur
        player.setPosition(new Vector3(0, 0, -7));
        player.setRotation(Quaternion.identity);
    }
}