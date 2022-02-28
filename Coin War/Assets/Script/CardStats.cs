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

    public SpriteRenderer mainArt;

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
    //Card Graphics for main image
    [SerializeField]
    private Sprite blademasterSpriteMain;
    [SerializeField]
    private Sprite druidSpriteMain;
    [SerializeField]
    private Sprite barbarianSpriteMain;
    [SerializeField]
    private Sprite assassinSpriteMain;
    [SerializeField]
    private Sprite alchemistSpriteMain;
    [SerializeField]
    private Sprite necromancerSpriteMain;
    [SerializeField]
    private Sprite mageSpriteMain;
    [SerializeField]
    private Sprite priestSpriteMain;

    //Card Graphics for classes
    [SerializeField]
    private Sprite blademasterSpriteClass;
    [SerializeField]
    private Sprite druidSpriteClass;
    [SerializeField]
    private Sprite barbarianSpriteClass;
    [SerializeField]
    private Sprite assassinSpriteClass;
    [SerializeField]
    private Sprite alchemistSpriteClass;
    [SerializeField]
    private Sprite necromancerSpriteClass;
    [SerializeField]
    private Sprite mageSpriteClass;
    [SerializeField]
    private Sprite priestSpriteClass;

    //Card Graphics for strong and weak
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
        InitializeMainSprites();
        InitializeClassesSprites();
        InitializeStrongClassesSprites();
        InitializeWeakClassesSprites();
    }
    void InitializeMainSprites()
    {
        if (cardname.Contains("ALCHEMIST")) mainArt.sprite = alchemistSpriteMain;
        else if(cardname.Contains("ASSASSIN")) mainArt.sprite = assassinSpriteMain;
        else if (cardname.Contains("BLADEMASTER")) mainArt.sprite = blademasterSpriteMain;
        else if (cardname.Contains("MAGE")) mainArt.sprite = mageSpriteMain;
        else if (cardname.Contains("DRUID")) mainArt.sprite = druidSpriteMain;
        else if (cardname.Contains("NECROMANCER")) mainArt.sprite = necromancerSpriteMain;
        else if (cardname.Contains("PRIEST")) mainArt.sprite = priestSpriteMain;
        else if (cardname.Contains("BARBARIAN")) mainArt.sprite = barbarianSpriteMain;

    }
        void InitializeClassesSprites()
    {
        int count = 0;
        foreach (CardElements c in cardClass) {
            
            switch (c)
            {
                case CardElements.Blademaster:
                    ClassSlot[count].sprite = blademasterSpriteClass;
                    break;
                case CardElements.Assassin:
                    ClassSlot[count].sprite = assassinSpriteClass;
                    break;
                case CardElements.Necromancer:
                    ClassSlot[count].sprite = necromancerSpriteClass;
                    break;
                case CardElements.Mage:
                    ClassSlot[count].sprite = mageSpriteClass;
                    break;
                case CardElements.Priest:
                     ClassSlot[count].sprite = priestSpriteClass;
                    break;
                case CardElements.Barbarian:
                    ClassSlot[count].sprite = barbarianSpriteClass;
                    break;
                case CardElements.Druid:
                    ClassSlot[count].sprite = druidSpriteClass;
                    break;
                case CardElements.Alchemist:
                    ClassSlot[count].sprite = alchemistSpriteClass;
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
