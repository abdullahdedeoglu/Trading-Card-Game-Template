using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public int attackPower;
    public int manaCost;
    public int health;

    public string cardName;
    [TextArea]
    public string cardDescription;

    public Sprite cardSprite;
}
