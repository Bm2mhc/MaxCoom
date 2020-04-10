/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class CharacterBattle : MonoBehaviour {

    private Character_Base characterBase;
    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private bool isPlayerTeam;
    private GameObject selectionCircleGameObject;
    public  HealthSystem healthSystem;
    private World_Bar healthBar;
    public int damageAmount;
    public int level = 1;
    public Text txt;
    public int upgradedamagevalue = 0;

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

    private void Start() {
    }

    public void Setup(bool isPlayerTeam) {
        this.isPlayerTeam = isPlayerTeam;
        if (isPlayerTeam) {
            characterBase.SetAnimsSwordTwoHandedBack();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().playerSpritesheet;
        } else {
            characterBase.SetAnimsSwordShield();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().enemySpritesheet;
        }
        healthSystem = new HealthSystem(10);
        healthBar = new World_Bar(transform, new Vector3(0, 15), new Vector3(12, 1.7f), Color.grey, Color.red, 1f, 100, new World_Bar.Outline { color = Color.black, size = .6f });
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;

        PlayAnimIdle();
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
       healthBar.SetSize(healthSystem.GetHealthPercent());
    }

    private void PlayAnimIdle() {
        if (isPlayerTeam) {
            characterBase.PlayAnimIdle(new Vector3(+1, 0));
        } else {
            characterBase.PlayAnimIdle(new Vector3(-1, 0));
        }
    }

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
        GameObject.FindGameObjectWithTag("Level").GetComponent<Text>() = "Level" + " " + level;
    }

    public void upgradedamage()
    {
        upgradedamagevalue += 1;
        Debug.Log(upgradedamagevalue);
    }


    public Vector3 GetPosition() {
        return transform.position;
    }

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

        CodeMonkey.Utils.UtilsClass.ShakeCamera(1f, .1f);

        if (healthSystem.IsDead()) {
            // Died
            Debug.Log("level" + " " + level + " " + "complete");
        }
    }

   

    public bool IsDead() {
        return healthSystem.IsDead();
    }

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

    public void HideSelectionCircle() {
        selectionCircleGameObject.SetActive(false);
    }
    public void ShowSelectionCircle() { 
        selectionCircleGameObject.SetActive(true);
    }

    public void playerWins()
    {
        level += 1;
        healthSystem.Heal(10 + level*2);
        GameObject.FindGameObjectWithTag("P2Life").GetComponent<Health>().health += 10 + level*2;

    }

    public void enemywins()
    {
        healthSystem.Heal(10);
        GameObject.FindGameObjectWithTag("P1Life").GetComponent<Health>().health += 10;

        if(level == 1)
        {
            level += 0;
        } else
        {
            level -= 1;
        }
    }

}
