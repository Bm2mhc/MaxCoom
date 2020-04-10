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
    private State state;

    private enum State {
        WaitingForPlayer,
        Busy,
    }

    private void Awake() {
        instance = this;
    }

    public void Update()
    {
        if(UnityEngine.Random.Range(1, 10) < 8)
        {
            attacksimple();
        } else
        {
            Heal();
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
            playerCharacterBattle.Attack(enemyCharacterBattle, () => {
                ChooseNextActiveCharacter();
            });
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
        playerCharacterBattle.upgradedamage();
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
            BattleOverWindow.Show_Static("Enemy Wins!");
            return true;
        }
        if (enemyCharacterBattle.IsDead()) {
            // Enemy dead, player wins
            //CodeMonkey.CMDebug.TextPopupMouse("Player Wins!");
            //BattleOverWindow.Show_Static("Player Wins!");
            enemyCharacterBattle.playerWins();
         
            //return true;
        }

        return false;
    }
}
