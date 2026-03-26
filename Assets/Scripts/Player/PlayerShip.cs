using UnityEngine;

public class PlayerShip : Entity, IPlayable
{


    // Variables dupliquées qui créent des dépendances
    private float speed = 5.0f;
    private int lives;
    [SerializeField] private GameObject playerDamageEffect; // Effet visuel quand un ennemi traverse
    [SerializeField] private GameObject playerShip;

    private IShootable shootable;


    public GameObject getPrefab()
    {
        return playerShip;
    }

    public GameObject getPlayerDamageEffect()
    {
        return playerDamageEffect;
    }

    public Transform getTransform()
    {
        return playerShip.transform;
    }

    public void setPosition(Vector3 pos)
    {
        playerShip.transform.position = pos;
    }
    public void setRotation(Quaternion rota)
    {
        playerShip.transform.rotation = rota;
    }

    void Start()
    {
        // Recherche du GameManager dans la scène
        gameManager = GameManager.getInstance();

        // Initialisation des variables
        lives = gameManager.getLives();
    }

    void Update()
    {
        // Mise à jour des variables depuis le GameManager
        lives = gameManager.getLives();
        HandlePlayerInput();
    }

    void HandlePlayerInput()
    {
        // D�placement du joueur
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // D�placement sur le plan XZ
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
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

            shootable.beShot(this);
        }
    }

    public void bePlayed()
    {
        HandlePlayerInput();
    }
}