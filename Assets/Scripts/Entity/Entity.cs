using UnityEngine;
using UnityEngine.Rendering;

public class Entity : MonoBehaviour
{
    protected GameManager gameManager;

    private void Start()
    {
        gameManager = gameManager.getInstance();
    }


    protected void DestroyObject()
    {
        if (gameObject == null) return;
        Destroy(this.gameObject);
    }
}
