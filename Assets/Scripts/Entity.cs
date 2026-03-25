using UnityEngine;

public class Entity : MonoBehaviour
{
    protected void DestroyObject()
    {
        if (gameObject == null) return;
        Destroy(this.gameObject);
    }
}
