// Script pour les projectiles
using UnityEngine;

public class BulletCollider : EntityColider
{

    public override void SetupCollisionComponents(GameObject obj, bool hasRigidbody, bool isTrigger)
    {
        // Ajouter ou configurer le collider si n�cessaire
        Collider collider = obj.GetComponent<Collider>();
        if (tag == "Bullet")
        {
            // Ajuster la taille du collider en fonction du tag
            BoxCollider boxCollider = (BoxCollider)GetComponent<Collider>();

            // Collider plus petit pour les balles
            boxCollider.size = new Vector3(0.3f, 0.3f, 0.5f);
        }
        base.SetupCollisionComponents(obj, hasRigidbody, isTrigger);
    }
    

    // Utilisons OnCollisionEnter au lieu de OnTriggerEnter
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Balle touche ennemi
            HandleBulletEnemyCollision(gameObject, collision.gameObject, collision.gameObject.GetComponent<Dangers>().getExplosionPrefab());
            gameManager.setScore(gameManager.getScore() + 100);

            // Chance de générer un power-up
            if (Random.value < 0.5f)
            {
                gameManager.SpawnPowerUp(collision.transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("Asteroid"))
        {
            // Balle touche astéroïde
            HandleBulletEnemyCollision(gameObject, collision.gameObject,collision.gameObject.GetComponent<Dangers>().getExplosionPrefab());
            gameManager.setScore(gameManager.getScore() + 50);
        }
    }

    public void HandleBulletEnemyCollision(GameObject bullet, GameObject enemy, GameObject explosionPrefab)
    {
        // Explosion avec effet de fragmentation
        if (exploder != null)
        {
            exploder.createExplosion(enemy);
        }
        else
        {
            // Fallback vers l'explosion originale
            Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        }

        // Destruction de l'ennemi
        Destroy(enemy, 0.1f); // Court d�lai pour permettre � l'explosion de commencer
        enemy.GetComponent<Dangers>().lDangers.Remove(enemy.GetComponent<Dangers>());

        // Destruction de la balle
        Destroy(bullet);
        bullet.GetComponent<Bullets>().lBullets.Remove(bullet.GetComponent<Bullets>());
    }
}