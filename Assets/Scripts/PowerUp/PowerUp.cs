using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PowerUp : Entity
{
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private PowerUpCollider upCollider;

    public void SpawnPowerUp(Vector3 position)
    {
        GameObject powerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);

        // Configuration des composants de collision pour le power-up
        upCollider.SetupCollisionComponents(powerUp, true, false, "PowerUp");

        // Ajouter le script de gestion de collision au power-up
        powerUp.AddComponent<PowerUpCollider>();

        gameManager.PowerUps.Add(powerUp.GetComponent<PowerUp>());
    }
    public void ApplyPowerUp(Bullets bullet, UIManager uiManager)
    {
        // Augmenter le nombre de projectiles pour tous les power-ups
        if (bullet.BulletCount < bullet.getMaxBulletCount())
        {
            bullet.BulletCount++;

            // Affichage d'un message temporaire pour informer le joueur
            StartCoroutine(uiManager.ShowPowerupMessage("Weapon Upgraded! Bullets: " + bullet.BulletCount));
        }
        else
        {
            // Bonus de score si le joueur a d�j� le maximum de projectiles
            gameManager.setScore(gameManager.getScore() + 200);
            StartCoroutine(uiManager.ShowPowerupMessage("Max Weapon Level! +200 Score"));
        }
    }
}
