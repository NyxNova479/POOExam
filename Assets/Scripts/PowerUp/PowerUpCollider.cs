using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollider : EntityColider
{
    public override void SetupCollisionComponents(GameObject obj, bool hasRigidbody, bool isTrigger)
    {
        // Ajouter ou configurer le collider si n�cessaire
        Collider collider = obj.GetComponent<Collider>();
        if (collider == null)
        {
            BoxCollider boxCollider = (BoxCollider)collider;
            // Ajouter un BoxCollider par d�faut
            collider = obj.AddComponent<BoxCollider>();
            // Collider plus grand pour les power-ups pour faciliter leur collecte
            boxCollider.size = new Vector3(1.2f, 1.2f, 1.2f);


        }
        base.SetupCollisionComponents(obj, hasRigidbody, isTrigger);
        
    }
}
