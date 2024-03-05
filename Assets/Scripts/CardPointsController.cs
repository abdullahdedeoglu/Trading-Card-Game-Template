using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController instance;

    public CardArea[] playerPoints, enemyPoints;

    public float waitTime = 0.25f;

    private void Awake()
    {
        instance = this;
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCO());
    }

    IEnumerator PlayerAttackCO()
    {
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < playerPoints.Length; i++)
        {
            if(playerPoints[i].activeCard != null)
            {
                if (enemyPoints[i].activeCard != null)
                {
                    playerPoints[i].activeCard.animator.SetTrigger("Attack");
                    enemyPoints[i].activeCard.GettingDamage(playerPoints[i].activeCard.attackPower);
                }

                else
                {
                    //Attack To Enemy's Health Points
                    playerPoints[i].activeCard.animator.SetTrigger("Attack");
                    BattleController.instance.EnemyGettingDamage(playerPoints[i].activeCard.attackPower);
                }
            }

            if (BattleController.instance.battleEnded)
            {
                i = playerPoints.Length;
            }
        }

        BattleController.instance.AdvanceTurn();
    }

    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCO());
    }

    IEnumerator EnemyAttackCO()
    {
        yield return new WaitForSeconds(waitTime);

        for (int i = 0; i < enemyPoints.Length; i++)
        {
            if (enemyPoints[i].activeCard != null)
            {
                if (playerPoints[i].activeCard != null)
                {
                    enemyPoints[i].activeCard.animator.SetTrigger("Attack");
                    playerPoints[i].activeCard.GettingDamage(enemyPoints[i].activeCard.attackPower);
                }

                else
                {
                    enemyPoints[i].activeCard.animator.SetTrigger("Attack");
                    BattleController.instance.PlayerGettingDamage(enemyPoints[i].activeCard.attackPower);
                    //Attack To Player's Health Points
                }
            }
            if (BattleController.instance.battleEnded)
            {
                i = enemyPoints.Length;
            }
        }

        BattleController.instance.AdvanceTurn();
    }
}
