using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int order = 0;

    private CardStats attackOrigin;
    private CardStats attackDestiny;

    public bool isItBlue = false;

    [SerializeField]
    private SpriteRenderer attackColor;
    [SerializeField]
    private Sprite attackIconRed;
    [SerializeField]
    private Sprite attackIconBlue;

    private int diceNumber = 0;//random number between 1 and 6, this is the result of the dice in each attack

    public enum attackAttribute { DoubleWeak, Weak, Neutral, Strong, DoubleStrong };

    public attackAttribute currentAttribute = attackAttribute.Neutral;

    //Every attack will have an origin (Who attacks) and a destiny (The target)
    public void SetAttack(CardStats originp, CardStats destinyp)
    {
        attackOrigin = originp;
        attackDestiny = destinyp;

        SetAttribute();
    }

    //Set the color of the attack, if blue is false then it will be red
    public void SetBlue(bool b)
    {
        isItBlue = b;

        if (isItBlue) attackColor.sprite = attackIconBlue;
        else attackColor.sprite = attackIconRed;

    }
    //Set the art of the attack icon
    public void SetAttackSprite(Sprite s)
    {
        GetComponent<SpriteRenderer>().sprite = s;
    }
    public CardStats GetAttackOrigin()
    {
        return attackOrigin;
    }
    public CardStats GetAttackDestiny()
    {
        return attackDestiny;
    }
    //This attribute is always from the perspective of the attacker
    private void SetAttribute()
    {
        int strongness = 0; // -2 is double weak, 0 is neutral and 2 is double strongger

        //Check strong against
        foreach (CardStats.CardElements elementOrigin in attackOrigin.strongAgainst)
        {
            foreach (CardStats.CardElements elementDestiny in attackDestiny.cardClass)
            {

                if (elementOrigin == elementDestiny)
                {

                    strongness++;
                }
            }
        }

        //Check weak against
        foreach (CardStats.CardElements elementOrigin in attackOrigin.weakAgainst)
        {
            foreach (CardStats.CardElements elementDestiny in attackDestiny.cardClass)
            {

                if (elementOrigin == elementDestiny)
                {

                    strongness--;
                }
            }
        }

        switch (strongness)
        {
            case -2:
                currentAttribute = attackAttribute.DoubleWeak;
                break;
            case -1:
                currentAttribute = attackAttribute.Weak;
                break;
            case 0:
                currentAttribute = attackAttribute.Neutral;
                break;
            case 1:
                currentAttribute = attackAttribute.Strong;
                break;
            case 2:
                currentAttribute = attackAttribute.DoubleStrong;
                break;
        }

    }
    public void SetDice(int i)
    {
        diceNumber = i;
    }
    public int GetDice()
    {
        return diceNumber;
    }
    public int GetEnoughToWin()
    {
        switch (currentAttribute)
        {
            case attackAttribute.DoubleWeak:
                return 6;
                break;
            case attackAttribute.Weak:
                return 5;
                break;
            case attackAttribute.Neutral:
                return 4;
                break;
            case attackAttribute.Strong:
                return 3;
                break;
            case attackAttribute.DoubleStrong:
                return 2;
                break;
            default:
                return 0;
                break;
        }
    }
    public void SetSpriteScale(Vector3 s)
    {
        transform.localScale = s;
    }
    public void SetSpritePosition(Vector3 p)
    {
        transform.localPosition = p;
    }

    public attackAttribute GetAttribute()
    {
        return currentAttribute;
    }
}
