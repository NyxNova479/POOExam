using UnityEngine;

public class PlayerShip : Entity
{
    // Références au GameManager pour accéder aux données
    private GameManager gameManager;

    // Variables dupliquées qui créent des dépendances
    protected float speed;
    protected int lives;

    void Start()
    {
        // Recherche du GameManager dans la scène
        gameManager = FindFirstObjectByType<GameManager>();

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