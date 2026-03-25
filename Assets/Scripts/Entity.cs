using UnityEngine;

public class Entity : MonoBehaviour
{
    protected GameManager gameManager;

    protected void DestroyObject()
    {
        if (gameObject == null) return;
        Destroy(this.gameObject);
    }
}
