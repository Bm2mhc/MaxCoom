using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class leveltext : CharacterBattle
{
    // Start is called before the first frame update
    public Text textObject;
    // CharacterBattle info;
    void Start()
    {
        Debug.Log("hej");
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log(level);
        changeText();


    }

    public void changeText()
    {

        textObject.text = "level " + level;
       
    }
}