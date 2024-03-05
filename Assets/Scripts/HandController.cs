using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    public List<Card> heldCards = new List<Card>();
    public List<Vector3> cardPositions = new List<Vector3>();
    public Transform minPos, maxPos;
    public bool isFromDeck;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetCardPositionsInHand();
    }
    public void SetCardPositionsInHand()
    {
        //TODO Adjust the helding card positions
        cardPositions.Clear();
        Vector3 distanceBetweenCards = Vector3.zero;

        if (heldCards.Count > 0)
        {
            distanceBetweenCards = ((maxPos.position - minPos.position) / (heldCards.Count - 1));
        }

        for (int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(minPos.position + distanceBetweenCards * i);

            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);
            heldCards[i].handPosition = i;
            heldCards[i].inHand = true;
        }
    }
    public void CardRemoveFromHand(Card cardToRemove)
    {
        if (heldCards[cardToRemove.handPosition] == cardToRemove)
        {
            heldCards.RemoveAt(cardToRemove.handPosition);
        }
        else
        {
            Debug.LogError("AGA NABIYON AGA");
        }

        SetCardPositionsInHand();
    }

    public void AddCard(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    public void EmptyHand()
    {
        foreach (Card card in heldCards)
        {
            card.inHand=false;
            card.MoveToPoint(BattleController.instance.discardPoint.transform.position, card.transform.rotation);
        }
        heldCards.Clear();
    }
    
}
