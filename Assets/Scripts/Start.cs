using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{
    public void dostart()
    {
      //  Reloads the scene
        Application.LoadLevel(Application.loadedLevel);
    }
}
