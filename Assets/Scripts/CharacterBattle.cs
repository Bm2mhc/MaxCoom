using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class CharacterBattle : MonoBehaviour {

    //Setting int, scripts and varibables
    private Character_Base characterBase;
    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private bool isPlayerTeam;
    private GameObject selectionCircleGameObject;
    public  HealthSystem healthSystem;
    private World_Bar healthBar;
    public int damageAmount;
    public int healthof = 10;
    public int level = 1;
    public int upgradedamagevalue = 0;
    public int upgradehealthvalue = 0;

    public static int test = 8;

    private enum State {
        Idle,
        Sliding,
        Busy,
    }
    private void Awake() {
        characterBase = GetComponent<Character_Base>();
        selectionCircleGameObject = transform.Find("SelectionCircle").gameObject;
        HideSelectionCircle();
        state = State.Idle;
    }

  
    //Setting up the players
    public void Setup(bool isPlayerTeam) {
        this.isPlayerTeam = isPlayerTeam;
        if (isPlayerTeam) {
            characterBase.SetAnimsSwordTwoHandedBack();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().playerSpritesheet;
        } else {
            characterBase.SetAnimsSwordShield();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().enemySpritesheet;
        }
        //creating the health of the characters
        healthSystem = new HealthSystem(healthof);
        //setting up the healthbar over the characters head
        healthBar = new World_Bar(transform, new Vector3(0, 15), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
       
        PlayAnimIdle();
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
       healthBar.SetSize(healthSystem.GetHealthPercent());
    }

    //sets the characters idle position
    private void PlayAnimIdle() {
        if (isPlayerTeam) {
            characterBase.PlayAnimIdle(new Vector3(+1, 0));
        } else {
            characterBase.PlayAnimIdle(new Vector3(-1, 0));
        }
    }
    //Checks what state the player is in and slides the character to the enemey
    private void Update() {
        switch (state) {
        case State.Idle:
            break;
        case State.Busy:
            break;
        case State.Sliding:
            float slideSpeed = 10f;
            transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

            float reachedDistance = 1f;
            if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance) {
                // Arrived at Slide Target Position
                //transform.position = slideTargetPosition;
                onSlideComplete();
            }
            break;
        }
    }
    //adds one attackdamage on upgradedamage function called
    public void upgradedamage()
    {
        upgradedamagevalue += 1;
    }
    //Adds one health on upgradehealt function called and heals the player
    public void upgradehealt()
    {
        upgradehealthvalue += 1;
        healthSystem.Heal(upgradehealthvalue * 2);

    }
    // gets Vector3 for sliding postion
    public Vector3 GetPosition() {
        return transform.position;
    }
    // Checks with player is doing damage and minus the damage done from the other players health
    public void Damage(CharacterBattle attacker, int damageAmount) {
        healthSystem.Damage(damageAmount);
       // CodeMonkey.CMDebug.TextPopup("health " + healthSystem.GetHealthAmount(), GetPosition());
        Vector3 dirFromAttacker = (GetPosition() - attacker.GetPosition()).normalized;
        if (isPlayerTeam)
        {
            GameObject.FindGameObjectWithTag("P1Life").GetComponent<Health>().health -= damageAmount;
        } else
        {
            GameObject.FindGameObjectWithTag("P2Life").GetComponent<Health>().health -= damageAmount;
        }
        DamagePopup.Create(GetPosition(), damageAmount, false);
        characterBase.SetColorTint(new Color(1, 0, 0, 1f));
        Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);


        if (healthSystem.IsDead()) {
            // Died
            Debug.Log("level" + " " + level + " " + "complete");
        }
    }

   
    //does what heathsystem.is dead does
    public bool IsDead() {
        return healthSystem.IsDead();
    }

    // defines the damage done and does attack
    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete) {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            characterBase.PlayAnimAttack(attackDir, () => {
                // Target hit
                int crit = UnityEngine.Random.Range(1, 100);
                if (crit < 95) {

                    int damageAmount = 1 + upgradedamagevalue;
                    targetCharacterBattle.Damage(this, damageAmount);
                } else
                {
                    int damageAmount = UnityEngine.Random.Range(7, 8)  + upgradedamagevalue;
                    targetCharacterBattle.Damage(this, damageAmount);
                }
              //  targetCharacterBattle.Damage(this, damageAmount);
                }, () => {
                // Attack completed, slide back
                SlideToPosition(startingPosition, () => {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    characterBase.PlayAnimIdle(attackDir);
                    onAttackComplete();
                });
            });
        });
    }
    //enemys attack same as above just with diffrent damage values
    public void AttackEnemy(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        // Slide to Target
        SlideToPosition(slideTargetPosition, () => {
            // Arrived at Target, attack him
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            characterBase.PlayAnimAttack(attackDir, () => {
                // Target hit
                int crit = UnityEngine.Random.Range(1, 100);
                if (crit < 95)
                {

                    int damageAmount = 1 + 2*(level - 1);
                   // Debug.Log(level);
                    targetCharacterBattle.Damage(this, damageAmount);
                }
                else
                {
                    int damageAmount = UnityEngine.Random.Range(7, 8) + 2 * (level - 1);
                    targetCharacterBattle.Damage(this, damageAmount);
                }
                //  targetCharacterBattle.Damage(this, damageAmount);
            }, () => {
                // Attack completed, slide back
                SlideToPosition(startingPosition, () => {
                    // Slide back completed, back to idle
                    state = State.Idle;
                    characterBase.PlayAnimIdle(attackDir);
                    onAttackComplete();
                });
            });
        });
    }

    //Heal characters from either player team or enemy
    public void heal(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        int heal = UnityEngine.Random.Range(1, 4);
        healthSystem.Heal(heal);
       
        if (isPlayerTeam)
            {
                GameObject.FindGameObjectWithTag("P1Life").GetComponent<Health>().health += heal;
            }
            else
            {
                GameObject.FindGameObjectWithTag("P2Life").GetComponent<Health>().health += heal;
            }
        onAttackComplete();
    }
    
    //sliding
    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete) {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;
        if (slideTargetPosition.x > 0) {
            characterBase.PlayAnimSlideRight();
        } else {
            characterBase.PlayAnimSlideLeft();
        }
    }

    //hides circle
    public void HideSelectionCircle() {
        selectionCircleGameObject.SetActive(false);
    }
    //shows circle
    public void ShowSelectionCircle() { 
        selectionCircleGameObject.SetActive(true);
    }
    //heals enemy when player wins and adds level
    public void playerWins()
    {
        healthSystem.Heal(10 + level * 2);
        level += 1;
        GameObject.FindGameObjectWithTag("P2Life").GetComponent<Health>().health += 10 + level*2;

    }
    //heals player when enemy wins and adds removes a level
    public void enemyWins()
    {
        healthSystem.Heal(10 + upgradehealthvalue*2);
        //healthSystem = new HealthSystem(10 + upgradehealthvalue*2);
       // healthSystem.healtupgrade(upgradehealthvalue * 2);
        GameObject.FindGameObjectWithTag("P1Life").GetComponent<Health>().health += 10 + upgradehealthvalue * 2;

        if(level == 1)
        {
            level += 0;
        } else
        {
            level -= 1;
        }
    }

}
