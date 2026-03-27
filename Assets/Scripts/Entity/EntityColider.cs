using UnityEngine;

public abstract class EntityColider : MonoBehaviour, IColidable
{

    protected GameManager gameManager;
    protected IExploder exploder;


    private void Start()
    {
        gameManager = GameManager.getInstance();

    }


    public virtual void SetupCollisionComponents(GameObject obj, bool hasRigidbody, bool isTrigger)
    {
        // Ajouter ou configurer le collider si n�cessaire
        Collider collider = obj.GetComponent<Collider>();


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

    public void collide(PlayerCollider player)
    {
        throw new System.NotImplementedException();
    }
}
