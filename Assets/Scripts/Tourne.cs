using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Tourne : MonoBehaviour
{
    private Vector3 dir;
    private float vitesse;
    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        vitesse = Random.Range(0.001f, 0.01f);
        float skal = Random.Range(0.5f, 2f);
        transform.localScale = new Vector3(skal, skal, skal);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAroundLocal(dir, vitesse);
    }
}
