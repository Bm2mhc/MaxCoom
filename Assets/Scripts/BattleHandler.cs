using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHandler : MonoBehaviour {

    private static BattleHandler instance;

    public static BattleHandler GetInstance() {
        return instance;
    }

    // Setup with variables, int, text, scripts and gameobjects
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

    //A function runing once per frame
    public void Update()
    {
        //changing level and coin text every frame with current level and coins
        leveltext.text = "level " + level;
        coinstext.text = "Coins " + coins;
        checkifcheat();

        //Chosing to attack or heal
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
    //Spawing Characters and setting states and activeplayers
    private void Start() {
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);

        SetActiveCharacterBattle(playerCharacterBattle);
        state = State.WaitingForPlayer;
    }

    //Create a cheatcode when you press the buttons K, I and M down at the exact same time
    private void checkifcheat()
    {
        if (Input.GetKeyDown(KeyCode.K) && Input.GetKeyDown(KeyCode.I) && Input.GetKeyDown(KeyCode.M))
        {
            coins += 100;
        }
    }
    
    //checking state and attacking other player, this one is called in update
    public void attacksimple() {
        if (state == State.WaitingForPlayer) {
            
                state = State.Busy;
                playerCharacterBattle.Attack(enemyCharacterBattle, () => {
                    ChooseNextActiveCharacter();
                });
        }
    }

    //checkingstate and attacking other player, this one is called from button attack
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

    //checking state and healing player, this one is called from update
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

    //checking state and healing player, this one is called from button healh
    public void Healother()
    {
            playerCharacterBattle.heal(playerCharacterBattle, () =>
        {
            ChooseNextActiveCharacter();
        });
        
    }

    //checking coins on player and upgrading damage and removing a coin
    public void upgradedamage()
    {
        if (coins > 0)
        {
            playerCharacterBattle.upgradedamage();
            coins -= 1;
        }
    }

    //checking coins on player and upgrading health and removing two coins
    public void upgradehealth()
    {
        if (coins > 1)
        {
            playerCharacterBattle.upgradehealt();
            coins -= 2;
        }
    }

    //spawing the characters at the right position
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

    //Choosing the active players
    private void SetActiveCharacterBattle(CharacterBattle characterBattle) {
        if (activeCharacterBattle != null) {
            activeCharacterBattle.HideSelectionCircle();
        }

        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    //changing between the characters
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

    //checking if game is over
    private bool TestBattleOver() {
       if (playerCharacterBattle.IsDead()) {
            // Player dead, enemy wins
            playerCharacterBattle.enemyWins();
        }
        if (enemyCharacterBattle.IsDead()) {
            // Enemy dead, player wins
            enemyCharacterBattle.playerWins();

            //gives player a coin
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
           
            //Changes level
            level += 1;
        }

        return false;
    }
}
