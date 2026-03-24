using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    // Références au GameManager pour accéder aux données
    private GameManager gameManager;

    // Variables dupliquées qui créent des dépendances
    public float speed;
    public int lives;

    void Start()
    {
        // Recherche du GameManager dans la scène
        gameManager = FindObjectOfType<GameManager>();

        // Initialisation des variables
        speed = gameManager.playerSpeed;
        lives = gameManager.lives;
    }

    void Update()
    {
        // Mise à jour des variables depuis le GameManager
        speed = gameManager.playerSpeed;
        lives = gameManager.lives;
    }
}