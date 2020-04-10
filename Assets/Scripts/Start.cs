using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start : MonoBehaviour
{
    public void dostart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
