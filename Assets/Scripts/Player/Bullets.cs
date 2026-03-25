using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Bullets : Entity, IMovable, IShootable
{


    private GameObject bulletPrefab;
    public float bulletSpeed = 10.0f;


    // Nouvelles variables pour les fonctionnalit�s demand�es
    [Header("Weapon Settings")]
    public int bulletCount = 1; // Nombre de projectiles tir�s simultan�ment
    public float bulletSpacing = 0.5f; // Espacement horizontal entre les projectiles
    public int maxBulletCount = 5; // Limite maximale de projectiles simultan�s



    public void beMoved(GameManager gameManager)
    {
        MoveBullets(gameManager.lBullets);
    }
    void MoveBullets(List<Bullets> bullets)
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

    void FireBullet(Transform playerTransform)
    {
        // Calcul de la position de d�part pour centrer les projectiles
        float startX = -((bulletCount - 1) * bulletSpacing) / 2;

        // Cr�ation de plusieurs balles c�te � c�te
        for (int i = 0; i < bulletCount; i++)
        {
            // Calcule la position avec l'offset horizontal
            Vector3 bulletOffset = new Vector3(startX + (i * bulletSpacing), -0.5f, 0.5f);
            Vector3 spawnPosition = playerTransform.position + bulletOffset;

            // Instanciation du projectile
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // Configuration des composants de collision pour la balle
            // Les projectiles doivent avoir un Rigidbody pour les collisions
            SetupCollisionComponents(bullet, true, false, "Bullet");

            // Ajouter le script de gestion de collision � la balle
            bullet.AddComponent<BulletCollider>();

            gameManager.lBullets.Add(bullet.GetComponent<Bullets>());
        }

        // Son de tir
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void beShot(PlayerShip player)
    {
        FireBullet(player.getTransform());
    }
}
