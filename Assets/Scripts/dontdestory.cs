using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dontdestory : MonoBehaviour
{
    //Sets up the gameobject music
    public GameObject music;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //The gameobject music does not destory when a new scene is loaded
        DontDestroyOnLoad(music);
    }
}
