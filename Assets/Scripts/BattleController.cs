using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    // Mana Variables
    public int startingMana = 4, maxMana = 12;
    
    //Player Mana
    private int currentPlayerMaxMana;
    public int playerMana;

    // Enemy Mana
    private int currentEnemyMaxMana;
    public int enemyMana;

    // Player Turn Variables
    public enum TurnOrder { playerTurn, playerAttack, enemyTurn, enemyAttack }
    public TurnOrder currentPhase;

    // Game Mechanic Variables
    public int startingCardsAmount = 5;
    public GameObject discardPoint;
    private bool isFirstTurn;
    public bool battleEnded;

    // Healths
    public int playerHealth;
    public int enemyHealth;

    public float resultScreenDelayTime = 1f;

    //TODO
    //With a basic random system, game choose which player start first,
    //for now it's unnecessary so i don't make but needed code is just
    //select a value with random.value and if random value greater than
    //some value we decide player first, if is not enemy first. It's very
    //easy but not needed for now


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        UIController.Instance.SetPlayerHealthText(playerHealth);
        UIController.Instance.SetEnemyHealthText(enemyHealth);
        currentPlayerMaxMana = startingMana;
        currentEnemyMaxMana = startingMana;
        FillPlayerMana();
        FillEnemyMana();
        DeckController.instance.DrawMultipleCards(startingCardsAmount);
        isFirstTurn = true;
    }

    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.Instance.SetManaText(playerMana);
    }

    public void FillEnemyMana()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.Instance.SetEnemyManaText(enemyMana);

    }
    public void SpendToPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;
        if (playerMana < 0)
        {
            playerMana = 0;
        }
        UIController.Instance.SetManaText(playerMana);
    }

    public void SpendToEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;
        if (enemyMana < 0)
        {
            enemyMana = 0;
        }
        UIController.Instance.SetEnemyManaText(enemyMana);
    }

    public void AdvanceTurn()
    {
        if (!battleEnded)
        {
            currentPhase++;

            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
            {
                currentPhase = 0;
            }
            switch (currentPhase)
            {
                case TurnOrder.playerTurn:
                    if (currentPlayerMaxMana < maxMana)
                    {
                        currentPlayerMaxMana++;
                    }
                    FillPlayerMana();

                    DeckController.instance.DrawCardToHand();
                    UIController.Instance.endPlayerTurnButton.SetActive(true);
                    break;
                case TurnOrder.playerAttack:
                    CardPointsController.instance.PlayerAttack();
                    break;
                case TurnOrder.enemyTurn:

                    if (currentEnemyMaxMana < maxMana && !isFirstTurn)
                    {
                        currentEnemyMaxMana++;
                    }
                    FillEnemyMana();

                    EnemyController.instance.StartAction();

                    isFirstTurn = false;
                    break;
                case TurnOrder.enemyAttack:
                    CardPointsController.instance.EnemyAttack();
                    break;

            }
        }  
    }
    public void PlayerGettingDamage(int damageAmount)
    {
        if (playerHealth > 0 && !battleEnded)
        {
            playerHealth -= damageAmount;
            if (playerHealth <= 0)
            {
                UIController.Instance.SetWinnerText(false);
                EndBattle();
            }
        }

        UIController.Instance.SetPlayerHealthText(playerHealth);
    }
    public void EnemyGettingDamage(int damageAmount)
    {
        if (enemyHealth > 0 && !battleEnded)
        {
            enemyHealth -= damageAmount;
            if (enemyHealth <= 0)
            {
                UIController.Instance.SetWinnerText(true);
                EndBattle();
            }
        }

        UIController.Instance.SetEnemyHealthText(enemyHealth);
    }

    public void EndBattle()
    {
        battleEnded = true;
        HandController.instance.EmptyHand();
        StartCoroutine(EndScreenCO());
    }

    IEnumerator EndScreenCO()
    {
        yield return new WaitForSeconds(resultScreenDelayTime);
        UIController.Instance.endScreen.SetActive(true);
    }
    public void EndPlayerTurn()
    {
        AdvanceTurn();

        //UIController.Instance.endPlayerTurnButton.SetActive(false);
    }

}
