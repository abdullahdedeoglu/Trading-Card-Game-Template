using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    // Enemy Deck Variables
    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform spawnPoint;
    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking}
    public AIType enemyAI;

    private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetupDeck();

        if(enemyAI != AIType.placeFromDeck)
        {
            SetUpHand();
        }
    }
    void Update()
    {
    }
    public void SetupDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iteration = 0;
        while (tempDeck.Count > 0 && iteration < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iteration++;
        }
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        if(activeCards.Count == 0)
        {
            SetupDeck();
        }

        yield return new WaitForSeconds(.5f);

        if(enemyAI != AIType.placeFromDeck )
        {
            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);

            if(activeCards.Count == 0)
            {
                SetupDeck();
            }
        }

        List<CardArea> cardPoints = new List<CardArea>();
        cardPoints.AddRange(CardPointsController.instance.enemyPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardArea selectedPoint = cardPoints[randomPoint];

        if(enemyAI == AIType.placeFromDeck || enemyAI==AIType.handRandomPlace)
        {
            cardPoints.Remove(selectedPoint);

            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }
        }

        CardScriptableObject selectedCard = null;
        int iteration = 0;

        List<CardArea> preferredPoint = new List<CardArea>();
        List<CardArea> secondaryPoint = new List<CardArea>();

        switch (enemyAI)
        {
            case AIType.placeFromDeck:

                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, spawnPoint, transform);
                    newCard.card = activeCards[0];
                    activeCards.RemoveAt(0);
                    newCard.SetupCards();

                    Quaternion additionalRotation = Quaternion.Euler(-90f, 0f, 0f);
                    Quaternion newRotation = selectedPoint.transform.rotation * additionalRotation;

                    newCard.MoveToPoint(selectedPoint.transform.position, newRotation);
                    Debug.Log("WTF");
                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlaced = selectedPoint;

                    newCard.SetEnemyStartPosition(selectedPoint.transform.position);
                }
                break;

            case AIType.handRandomPlace:
                selectedCard = SelectCardToPlay();
                iteration = 50;
                while(selectedCard != null && iteration>0 && selectedPoint.activeCard == null)
                {
                    PlayCard(selectedCard, selectedPoint);

                    //Check if we should play another card
                    selectedCard=SelectCardToPlay();

                    iteration--;

                    yield return new WaitForSeconds(CardPointsController.instance.waitTime);

                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }
                break;

            case AIType.handDefensive:
                selectedCard= SelectCardToPlay();

                preferredPoint.Clear();
                secondaryPoint.Clear();

                for(int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerPoints[i].activeCard != null)
                        {
                            preferredPoint.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoint.Add(cardPoints[i]);
                        }
                    }
                }

                iteration = 50;
                while(selectedCard != null && preferredPoint.Count + secondaryPoint.Count > 0 && iteration>0)
                {
                    if (preferredPoint.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoint.Count);
                        selectedPoint = preferredPoint[selectPoint];

                        preferredPoint.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoint.Count);
                        selectedPoint = secondaryPoint[selectPoint];

                        secondaryPoint.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //check if we should play another card
                    selectedCard = SelectCardToPlay();

                    iteration--;
                    yield return new WaitForSeconds(1);
                }

                break;

            case AIType.handAttacking:
                selectedCard = SelectCardToPlay();

                preferredPoint.Clear();
                secondaryPoint.Clear();

                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointsController.instance.playerPoints[i].activeCard == null)
                        {
                            preferredPoint.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoint.Add(cardPoints[i]);
                        }
                    }
                }

                iteration = 50;
                while (selectedCard != null && preferredPoint.Count + secondaryPoint.Count > 0 && iteration > 0)
                {
                    if (preferredPoint.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoint.Count);
                        selectedPoint = preferredPoint[selectPoint];

                        preferredPoint.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoint.Count);
                        selectedPoint = secondaryPoint[selectPoint];

                        secondaryPoint.RemoveAt(selectPoint);
                    }

                    PlayCard(selectedCard, selectedPoint);

                    //check if we should play another card
                    selectedCard = SelectCardToPlay();

                    iteration--;
                    yield return new WaitForSeconds(1);
                }

                break;
        }


        yield return new WaitForSeconds(0.5f);
        BattleController.instance.AdvanceTurn();

    }

    void SetUpHand()
    {

        if (activeCards.Count == 0)
        {
            SetupDeck();
        }
        for (int i = 0; i < startHandSize; i++)
        {
            cardsInHand.Add(activeCards[0]);

            activeCards.RemoveAt(0);
        }
    }

    public void PlayCard(CardScriptableObject card, CardArea selectedPoint)
    {
        Card newCard = Instantiate(cardToSpawn, spawnPoint, transform);
        newCard.card = card;
        activeCards.RemoveAt(0);
        newCard.SetupCards();

        Quaternion additionalRotation = Quaternion.Euler(-90f, 0f, 0f);
        Quaternion newRotation = selectedPoint.transform.rotation * additionalRotation;

        newCard.MoveToPoint(selectedPoint.transform.position, newRotation);

        selectedPoint.activeCard = newCard;
        newCard.assignedPlaced = selectedPoint;

        newCard.SetEnemyStartPosition(selectedPoint.transform.position);
        cardsInHand.Remove(card);
        BattleController.instance.SpendToEnemyMana(card.manaCost);
    }

    CardScriptableObject SelectCardToPlay()
    {
        CardScriptableObject cardToPlay = null;
        List<CardScriptableObject> playAbleCards = new List<CardScriptableObject>();

        foreach (CardScriptableObject card in cardsInHand)
        {
            if(card.manaCost <= BattleController.instance.enemyMana)
            {
                playAbleCards.Add(card);
            } 
        }

        if (playAbleCards.Count > 0)
        {
            int selected = Random.Range(0, playAbleCards.Count);

            cardToPlay = cardsInHand[selected];
        }

        return cardToPlay;
    }

}
