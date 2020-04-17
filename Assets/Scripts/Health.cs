using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is a old script we had when the grafics was diffrent, not used in the final project

public class Health : MonoBehaviour
{
    // Creates variables
    public int health;
    public int numOfHearts;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Update(){
        //Debug.Log(player1.healthSystem);
        //makes the health the number of hearts if it exedes the number of hearts
        if(health > numOfHearts)
        {
            health = numOfHearts;
        }
      
        //removes a heart if the array lenght changes

        for (int i = 0; i < hearts.Length; i++){
            if(i < health){
                hearts[i].sprite = fullHeart;
            } else {
                hearts[i].sprite = emptyHeart;
            }

            if(i < numOfHearts)
            {
                hearts[i].enabled = true;
            } else
            {
                hearts[i].enabled = false;
            }
            


        }
    }

}
