using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem {

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;

    private int healthMax;
    private int health;

    public HealthSystem(int healthMax) {
        this.healthMax = healthMax;
        health = healthMax;
    }

    // sets the amount of health
    public void SetHealthAmount(int health) {
        this.health = health;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
    
    //takes the health and divides the healthMax this if for the healthbar
    public float GetHealthPercent() {
        return (float)health / healthMax;
    }

    //Gets the amount of health
    public int GetHealthAmount() {
        return health;
    }

    //Removes the damage from the health of either the player or enemy
    public void Damage(int amount) {
        health -= amount;
        if (health < 0) {
            health = 0;
        }
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    //sets when the player is dead
    public bool IsDead() {
        return health <= 0;
    }

   // Heals the player the amount healht the script gets
    public void Heal(int amount) {
        health += amount;
        /*if (health > healthMax) {
            health = healthMax;
        }*/
        healthMax += amount;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
}

