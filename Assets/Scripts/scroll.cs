using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour
{

    public float speed;
    void Start()
    {
       
    }

    void Update()
    {
        // makes the background move
        Vector2 offset = new Vector2(Time.time * speed, 0);

        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
