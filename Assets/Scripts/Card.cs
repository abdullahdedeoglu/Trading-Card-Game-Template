using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Pool;
public class Card : MonoBehaviour
{
    // Scriptable Object
    public CardScriptableObject card;

    // Card Powers
    public int attackPower;
    public int manaCost;
    public int health;

    // Card Canvas
    public TMP_Text attackPowerText;
    public TMP_Text manaCostText;
    public TMP_Text healthText;
    public TMP_Text cardNameText;
    public TMP_Text cardDescriptionText;

    public Image cardImage;

    // Moving Elements
    private Vector3 targetPoint;
    public float moveSpeed = 5f, rotSpeed=5f;
    private Quaternion targetRot;

    // Hand Controls
    public int handPosition;
    public bool inHand;
    private HandController handController;

    // Card Playing Elements
    private bool isSelected;
    private Collider cardCollider;
    public LayerMask desktop;

    private bool justPressed;
    public LayerMask cardArea;
    public CardArea assignedPlaced;

    // Player Controls
    public bool isPlayer;

    // Enemy Controls
    public bool isHigh;
    public Vector3 startingPosition;

    // Animation
    public Animator animator;
    public float jumpAnimationWaitTime = 1f;

    void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        SetupCards(); 
        handController = FindAnyObjectByType<HandController>();
        cardCollider = GetComponent<Collider>();
    }

    public void SetEnemyStartPosition(Vector3 cardPosition)
    {
          startingPosition = cardPosition;
    }
    public void SetupCards()
    {
        manaCost = card.manaCost;
        attackPower = card.attackPower;
        health = card.health;
        DisplayCardPowers();
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.cardDescription;
        cardImage.sprite = card.cardSprite;
    }

    public void DisplayCardPowers()
    {
        attackPowerText.text = attackPower.ToString();
        manaCostText.text = manaCost.ToString();
        healthText.text = health.ToString();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);

        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, desktop))
            {
                MoveToPoint(hit.point + new Vector3(0,4f,0), Quaternion.identity);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ReturnToHand();
            }

            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.currentPhase == BattleController.TurnOrder.playerTurn && BattleController.instance.battleEnded == false)
            {
                if (Physics.Raycast(ray, out hit, 100f, cardArea))
                {
                    CardArea cardPlace = hit.collider.GetComponent<CardArea>();

                    if(cardPlace.activeCard == null && cardPlace.isPlayerArea)
                    {
                        if (BattleController.instance.playerMana >= manaCost)
                        {
                            cardPlace.activeCard = this;
                            assignedPlaced = cardPlace;

                            MoveToPoint(cardPlace.transform.position, Quaternion.identity);
                            inHand = false;
                            isSelected = false;
                            BattleController.instance.SpendToPlayerMana(manaCost);
                            handController.CardRemoveFromHand(this);
                        }
                        else
                        {
                            UIController.Instance.ShowLowManaWarning();
                            ReturnToHand();
                        }
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
            justPressed = false;
        }
    }
    
    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch; 
    }

    public void ReturnToHand()
    {
        isSelected = false;
        cardCollider.enabled = true;

        MoveToPoint(handController.cardPositions[handPosition], handController.minPos.transform.rotation);
    }

    public void GettingDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(DeathAnimCo());
        }
        DisplayCardPowers();
        StartCoroutine(DamageAnimCo());
    }

    IEnumerator DamageAnimCo()
    {
        yield return new WaitForSeconds(jumpAnimationWaitTime);
        animator.SetTrigger("Jump");
        yield return null;
    }

    IEnumerator DeathAnimCo()
    {
        yield return new WaitForSeconds(jumpAnimationWaitTime);
        MoveToPoint(BattleController.instance.discardPoint.transform.position, Quaternion.identity);
        Destroy(gameObject, 0.8f);
        yield return null;

    }

    private void OnMouseOver()
    {
        if (inHand && isPlayer && !BattleController.instance.battleEnded)
        {
            MoveToPoint(handController.cardPositions[handPosition] + new Vector3(0, 1f, .5f), Quaternion.identity);
        }

        if(!isPlayer && !isHigh && !BattleController.instance.battleEnded)
        {
            MoveToPoint(startingPosition + new Vector3(0, 4f, -3f), Quaternion.identity);
            isHigh = true;
        }
    }

    private void OnMouseExit()
    {
        if (inHand && isPlayer && !BattleController.instance.battleEnded)
        {
            MoveToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);
        }

        if (!isPlayer && isHigh && !BattleController.instance.battleEnded)
        {
            MoveToPoint(startingPosition, Quaternion.identity);
            isHigh = false;
        }
    }

    private void OnMouseDown()
    {
        if (inHand && isPlayer && BattleController.instance.currentPhase==BattleController.TurnOrder.playerTurn && !BattleController.instance.battleEnded)
        {
            isSelected = true;
            cardCollider.enabled = false;
            justPressed = true;
        }

    }
}
