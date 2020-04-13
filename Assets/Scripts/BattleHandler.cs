/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHandler : MonoBehaviour {

    private static BattleHandler instance;

    public static BattleHandler GetInstance() {
        return instance;
    }


    [SerializeField] private Transform pfCharacterBattle;
    public Texture2D playerSpritesheet;
    public Texture2D enemySpritesheet;

    private CharacterBattle playerCharacterBattle;
    private CharacterBattle enemyCharacterBattle;
    private CharacterBattle activeCharacterBattle;
    public CharacterBattle other;
    private State state;
    public Text coinstext;
    public Text leveltext;
    public int level = 1;
    public int coins = 0;

    private enum State {
        WaitingForPlayer,
        Busy,
    }

    private void Awake() {
        instance = this;
    }

    public void Update()
    {

        leveltext.text = "level " + level;
        coinstext.text = "Coins " + coins;

        if (UnityEngine.Random.Range(1, 10) < 8)
        {
            attacksimple();
           // textObject.text = "level " + playerCharacterBattle.level;
          //  level = other.level;
            //Debug.Log(level);
        } else
        {
            Heal();
            //textObject.text = "level " + playerCharacterBattle.level;
           // level = other.level;
           // Debug.Log(level);
        }
    }
    private void Start() {
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);

        SetActiveCharacterBattle(playerCharacterBattle);
        state = State.WaitingForPlayer;
    }

    public void attacksimple() {
        if (state == State.WaitingForPlayer) {
            
                state = State.Busy;
                playerCharacterBattle.Attack(enemyCharacterBattle, () => {
                    ChooseNextActiveCharacter();
                });
        }
    }

    public void attackother()
    {
        if (state == State.WaitingForPlayer)
        {

            state = State.Busy;
            playerCharacterBattle.Attack(enemyCharacterBattle, () => {
                ChooseNextActiveCharacter();
            });
        }
    }

    public void Heal()
    {
        if (state == State.WaitingForPlayer)
        {

            state = State.Busy;
            playerCharacterBattle.heal(playerCharacterBattle, () =>
        {
            ChooseNextActiveCharacter();
        });
        }
    }

    public void Healother()
    {
            playerCharacterBattle.heal(playerCharacterBattle, () =>
        {
            ChooseNextActiveCharacter();
        });
        
    }

    public void upgradedamage()
    {
        if (coins > 0)
        {
            playerCharacterBattle.upgradedamage();
            coins -= 1;
        }
    }

    public void upgradehealth()
    {
        if (coins > 1)
        {
            playerCharacterBattle.upgradehealt();
            coins -= 2;
        }
    }


    private CharacterBattle SpawnCharacter(bool isPlayerTeam) {
        Vector3 position;
        if (isPlayerTeam) {
            position = new Vector3(-50, 0);
        } else {
            position = new Vector3(+50, 0);
        }
        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam);

        return characterBattle;
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle) {
        if (activeCharacterBattle != null) {
            activeCharacterBattle.HideSelectionCircle();
        }

        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter() {
        if (TestBattleOver()) {
            return;
        }

        if (activeCharacterBattle == playerCharacterBattle) {
            SetActiveCharacterBattle(enemyCharacterBattle);
            state = State.Busy;

            if (UnityEngine.Random.Range(1, 10) < 9)
            {
                enemyCharacterBattle.AttackEnemy(playerCharacterBattle, () =>
                {
                    ChooseNextActiveCharacter();
                });
            }else
            {
                enemyCharacterBattle.heal(enemyCharacterBattle, () =>
                {
                    ChooseNextActiveCharacter();
                });
            }

        } else {
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }
    }

    private bool TestBattleOver() {
       if (playerCharacterBattle.IsDead()) {
            // Player dead, enemy wins
            //CodeMonkey.CMDebug.TextPopupMouse("Enemy Wins!");
            //BattleOverWindow.Show_Static("Enemy Wins!");
            playerCharacterBattle.enemyWins();
            if (level > 1)
            {
                level -= 1;
            }
           // textObject.text = "level " + playerCharacterBattle.level;
            // return true;
        }
        if (enemyCharacterBattle.IsDead()) {
            // Enemy dead, player wins
            //CodeMonkey.CMDebug.TextPopupMouse("Player Wins!");
            //BattleOverWindow.Show_Static("Player Wins!");
            enemyCharacterBattle.playerWins();
            int randomcoinamount = UnityEngine.Random.Range(0, 1000);
            if (randomcoinamount < 500)
            {
                coins += 1;
            } else if(randomcoinamount > 500 && randomcoinamount < 700)
            {
                coins += 2;
            } else if (randomcoinamount > 700 && randomcoinamount < 800)
            {
                coins += 3;
            } else if (randomcoinamount > 800 && randomcoinamount < 900)
            {
                coins += 4;
            } else if (randomcoinamount > 900 && randomcoinamount < 990)
            {
                coins += 5;
            } else if (randomcoinamount > 990 && randomcoinamount < 1000)
            {
                coins += 10;
            }
           
            level += 1;
          //  textObject.text = "level " + playerCharacterBattle.level;

            //return true;
        }

        return false;
    }
}
