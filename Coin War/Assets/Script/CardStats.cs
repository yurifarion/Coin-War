using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CardStats : MonoBehaviour
{
    public enum CardElements
    {
        Blademaster,
        Druid,
        Barbarian,
        Assassin,
        Alchemist,
        Necromancer,
        Mage,
        Priest,
        None
    }

    public string cardname;

    //Current Classes
    public CardElements[] cardClass =  new CardElements[2];

    //Strong against
    public CardElements[] strongAgainst = new CardElements[2];

    //Strong against
    public CardElements[] weakAgainst = new CardElements[2];

    public float speedLevel = 0;

    //Card Slots
    public SpriteRenderer[] ClassSlot = new SpriteRenderer[2];
    public SpriteRenderer[] strongAgainstSlot = new SpriteRenderer[2];
    public SpriteRenderer[] weakAgainstSlot = new SpriteRenderer[2];


    public TextMesh cardNameText;
    public TextMesh cardSpeedText;

    //Card Graphics
    [SerializeField]
    private Sprite blademasterSprite;
    [SerializeField]
    private Sprite druidSprite;
    [SerializeField]
    private Sprite barbarianSprite;
    [SerializeField]
    private Sprite assassinSprite;
    [SerializeField]
    private Sprite alchemistSprite;
    [SerializeField]
    private Sprite necromancerSprite;
    [SerializeField]
    private Sprite mageSprite;
    [SerializeField]
    private Sprite priestSprite;

    private void Start()
    {
        //Initiliaze Card
        cardNameText.text = cardname;
        cardSpeedText.text = "" + speedLevel;
        InitializeClassesSprites();
        InitializeStrongClassesSprites();
        InitializeWeakClassesSprites();
    }
    void InitializeClassesSprites()
    {
        int count = 0;
        foreach (CardElements c in cardClass) {
            
            switch (c)
            {
                case CardElements.Blademaster:
                    ClassSlot[count].sprite = blademasterSprite;
                    break;
                case CardElements.Assassin:
                    ClassSlot[count].sprite = assassinSprite;
                    break;
                case CardElements.Necromancer:
                    ClassSlot[count].sprite = necromancerSprite;
                    break;
                case CardElements.Mage:
                    ClassSlot[count].sprite = mageSprite;
                    break;
                case CardElements.Priest:
                     ClassSlot[count].sprite = priestSprite;
                    break;
                case CardElements.Barbarian:
                    ClassSlot[count].sprite = barbarianSprite;
                    break;
                case CardElements.Druid:
                    ClassSlot[count].sprite = druidSprite;
                    break;
                case CardElements.Alchemist:
                    ClassSlot[count].sprite = alchemistSprite;
                    break;

                default:
                    ClassSlot[count].sprite = null;
                    break;
            }
            count++;
        }
    }
    void InitializeStrongClassesSprites()
    {
        int count = 0;
        foreach (CardElements c in strongAgainst)
        {

            switch (c)
            {
                case CardElements.Blademaster:
                    strongAgainstSlot[count].sprite = blademasterSprite;
                    break;
                case CardElements.Assassin:
                    strongAgainstSlot[count].sprite = assassinSprite;
                    break;
                case CardElements.Necromancer:
                    strongAgainstSlot[count].sprite = necromancerSprite;
                    break;
                case CardElements.Mage:
                    strongAgainstSlot[count].sprite = mageSprite;
                    break;
                case CardElements.Priest:
                    strongAgainstSlot[count].sprite = priestSprite;
                    break;
                case CardElements.Barbarian:
                    strongAgainstSlot[count].sprite = barbarianSprite;
                    break;
                case CardElements.Druid:
                    strongAgainstSlot[count].sprite = druidSprite;
                    break;
                case CardElements.Alchemist:
                    strongAgainstSlot[count].sprite = alchemistSprite;
                    break;

                default:
                    strongAgainstSlot[count].sprite = null;
                    break;
            }
            count++;
        }
    }

    void InitializeWeakClassesSprites()
    {
        int count = 0;
        foreach (CardElements c in weakAgainst)
        {

            switch (c)
            {
                case CardElements.Blademaster:
                    weakAgainstSlot[count].sprite = blademasterSprite;
                    break;
                case CardElements.Assassin:
                    weakAgainstSlot[count].sprite = assassinSprite;
                    break;
                case CardElements.Necromancer:
                    weakAgainstSlot[count].sprite = necromancerSprite;
                    break;
                case CardElements.Mage:
                    weakAgainstSlot[count].sprite = mageSprite;
                    break;
                case CardElements.Priest:
                    weakAgainstSlot[count].sprite = priestSprite;
                    break;
                case CardElements.Barbarian:
                    weakAgainstSlot[count].sprite = barbarianSprite;
                    break;
                case CardElements.Druid:
                    weakAgainstSlot[count].sprite = druidSprite;
                    break;
                case CardElements.Alchemist:
                    weakAgainstSlot[count].sprite = alchemistSprite;
                    break;

                default:
                    weakAgainstSlot[count].sprite = null;
                    break;
            }
            count++;
        }
    }

}
