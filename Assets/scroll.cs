﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * speed, 0);

        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
