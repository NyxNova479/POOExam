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
    [SerializeField] private Dangers dangerScript;
    [SerializeField] private PowerUp powerUp;
    [SerializeField] private GameObject gameOverPanel;



    private ISpawnable spawnable;
    private IMovable movable;
    private IShootable shootable;

    [Header("Explosion")]
    private ExplosionManager explosionManager;



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





        // S'assurer que le joueur a les composants n�cessaires pour les collisions
        //SetupCollisionComponents(player.getPrefab(), true, false, "Player");


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
        foreach (PowerUp powerUp in powerUp.PowerUps)
        {
            spawnable = powerUp;
            spawnable.beSpawned(this);
        }
    }

    public void ApplyPowerUp()
    {
        return;
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