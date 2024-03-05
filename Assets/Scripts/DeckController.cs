using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;

    public float drawMultipleCardWaitTime=0.25f;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetupDeck();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            DrawCardToHand();
        }
    }
    public void SetupDeck()
    {
        activeCards.Clear();
        
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iteration=0;
        while (tempDeck.Count > 0 && iteration<500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iteration++;
        }

    }
    public void DrawCardToHand()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        Card newCard = Instantiate(cardToSpawn, this.transform.position, transform.rotation);
        //Debug.Log("New Card Starting Position: " + newCard.transform.position);

        newCard.card = activeCards[0];
        newCard.SetupCards();
        HandController.instance.AddCard(newCard);
        activeCards.RemoveAt(0);
    }
    public void DrawMultipleCards(int drawCardAmount)
    {
        StartCoroutine(DrawMultipleCo(drawCardAmount));
    }
    IEnumerator DrawMultipleCo(int drawCardAmount)
    {
        for (int i = 0; i < drawCardAmount; i++)
        {
            DrawCardToHand();
            yield return new WaitForSeconds(drawMultipleCardWaitTime);
        }
    }
}
