using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private CharacterBattle characterbattle;
    public Sprite Diceone;

    void Start()
    {
        
    }

    void Update()
    {
        int damage = GameObject.Find("characterBattle").GetComponent<CharacterBattle>().damageAmount;

        if (damage < 10) 
        {
            Debug.Log("Fuckoff");
            this.gameObject.GetComponent<SpriteRenderer>().sprite = Diceone;

        }
    }
}
