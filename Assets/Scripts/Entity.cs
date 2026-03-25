using UnityEngine;

public abstract class Entity : MonoBehaviour
{

    public void DestroyObject()
    {
        if (gameObject == null) return;
        Destroy(this.gameObject);
    }
}
