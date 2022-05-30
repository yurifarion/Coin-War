using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttackManager : MonoBehaviour
{
    /*
     * This list will save all the official attacks in the turn
     * including attacks made from the opponent
     */
    public List<Attack> turnAttacks = new List<Attack>();

    /*This list will storage attacks are not official yet
    the attack to be official needs to have one origin and one destiny
    origin needs to be from the master and destiny from the enemy
    Once the players selects a correct combination it will turn oficial
    and this list will be empty*/

    public List<CardStats> tempAttacks = new List<CardStats>();


    private List<GameObject> attackIcons = new List<GameObject>();
    [SerializeField]
    private Sprite attackIconAlchemist;
    [SerializeField]
    private Sprite attackIconAssassin;
    [SerializeField]
    private Sprite attackIconBarbarian;
    [SerializeField]
    private Sprite attackIconBlademaster;
    [SerializeField]
    private Sprite attackIconMage;
    [SerializeField]
    private Sprite attackIconDruid;
    [SerializeField]
    private Sprite attackIconNecromancer;
    [SerializeField]
    private Sprite attackIconPriest;

    public GameObject attackIconPrefab;

    //GameObject of the AttackTurn
    [SerializeField]
    private GameObject attackturn_GO;
    [SerializeField]
    private Transform card_left;
    [SerializeField]
    private Transform card_right;
    //attack order is being used to make sure all attacks are spawned in the correct order
    private int attackOrder = 0;

    //UI from the attack window
    public TMPro.TextMeshPro card_leftRole;
    public TMPro.TextMeshPro card_rightRole;
    public TMPro.TextMeshPro attribute;
    public TMPro.TextMeshPro rollToWin;
    public TMPro.TextMeshPro rollFromDice;
    public TMPro.TextMeshPro card_leftWinOrLose;
    public TMPro.TextMeshPro card_rightWinOrLose;

    //Variables to make the dice animation
    private bool isDiceAnimationOn = false;
   

    private float diceTimer = 0;
    private float diceTimeInterval = 0.05f;
    private int counterDice = 0;
    private int finalDiceNumber = 0;


    public Animator attackWindowAnim;

    private GameObject left,right;
    private List<GameObject> allLeftCards = new List<GameObject>();//Save all left cards involved in this turn attacks
    private List<GameObject> allRightCards = new List<GameObject>();//Save all left cards involved in this turn attacks
    private List<int> loserList = new List<int>();

    private void Update()
    {
        DiceAnimation();
    }
    public void AddAttack(int originID , int destinyID)
    {
        bool isItblue;
        isItblue = GetComponent<BoardManager>().Master.side == "Left";

        //Random number from 1 to 6 that will be the result of dice
        int diceResult = Random.Range(1, 6);
        GetComponent<PhotonView>().RPC("PhotonAddAttack", RpcTarget.AllBuffered, originID,destinyID, isItblue, diceResult);
    }
    [PunRPC]
    public void PhotonAddAttack(int originpID, int destinypID, bool isBlue , int diceResult)
    {
        CardStats originp, destinyp;
        originp = GetComponent<BoardManager>().GetCardFromID(originpID);
        destinyp = GetComponent<BoardManager>().GetCardFromID(destinypID);

        Vector3 newposition = destinyp.gameObject.transform.position;
        newposition -= new Vector3(1, 0, 0);
        GameObject o = Instantiate(attackIconPrefab, newposition, Quaternion.identity);
        o.GetComponent<Attack>().SetAttack(originp, destinyp);
        o.GetComponent<Attack>().order = turnAttacks.Count;
        o.GetComponent<Attack>().SetBlue(isBlue);
        o.GetComponent<Attack>().SetDice(diceResult);

        //set scale and position depending if there is or not a attack there already
        int count = 1;//it will have at least the current attack, so it starts in 1
        List<Attack> acumulativeAttacks = new List<Attack>();//all attacks that the card is the destiny, to make icons smaller
        acumulativeAttacks.Add(o.GetComponent<Attack>());

        foreach (Attack a in turnAttacks)
        {
            if(a.GetAttackDestiny() == destinyp)
            {
                count++;
                acumulativeAttacks.Add(a);
            }
        }
        
        for(int i = 0; i < acumulativeAttacks.Count; ++i)
        {
            Vector3 position = newposition + (new Vector3(0, 0.6f, 0) * i);//this is used to adjust the position
            Vector3 scale = new Vector3(0.12f, 0.12f, 0.12f);

            acumulativeAttacks[i].SetSpritePosition(position);
            acumulativeAttacks[i].SetSpriteScale(scale);
        }

        attackIcons.Add(o);




        if (originp.cardname.Contains("ALCHEMIST")) o.GetComponent<Attack>().SetAttackSprite(attackIconAlchemist);
        else if (originp.cardname.Contains("ASSASSIN")) o.GetComponent<Attack>().SetAttackSprite(attackIconAssassin);
        else if (originp.cardname.Contains("BLADEMASTER")) o.GetComponent<Attack>().SetAttackSprite(attackIconBlademaster);
        else if (originp.cardname.Contains("MAGE")) o.GetComponent<Attack>().SetAttackSprite(attackIconMage);
        else if (originp.cardname.Contains("DRUID")) o.GetComponent<Attack>().SetAttackSprite(attackIconDruid);
        else if (originp.cardname.Contains("NECROMANCER")) o.GetComponent<Attack>().SetAttackSprite(attackIconNecromancer);
        else if (originp.cardname.Contains("PRIEST")) o.GetComponent<Attack>().SetAttackSprite(attackIconPriest);
        else if (originp.cardname.Contains("BARBARIAN")) o.GetComponent<Attack>().SetAttackSprite(attackIconBarbarian);




        turnAttacks.Add(o.GetComponent<Attack>());
    }
    public void AddTempAttack(CardStats cardp)
    {
        //First card (origin)
        if (tempAttacks.Count <= 0)
        {
            //if we click once in a card , let's turn it in Attack Mode, but it should be inside of a battleground deck
            if (cardp.gameObject.transform.parent.gameObject.name.Contains("Battle") && !CardAlreadyAttacked(cardp))
            {


                //If that card is in the player deck
                if (GetComponent<BoardManager>().GetMaster().currentDeck.Contains(cardp.gameObject))
                {

                    //Needs to check if cardp is from master and it is on battleground
                    tempAttacks.Add(cardp);
                }
                else
                {
                    tempAttacks.Add(cardp);
                    StartCoroutine(DeactivateWrongCard());
                    //cardp.deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
                }
            }
            else
            {
                tempAttacks.Add(cardp);
                StartCoroutine(DeactivateWrongCard());
            }

        }
        //Second card (Destiny)
        else if (tempAttacks.Count > 0)
        {
            //If that card is not in the player deck
            if (!GetComponent<BoardManager>().GetMaster().currentDeck.Contains(cardp.gameObject))
            {
                tempAttacks.Add(cardp);

                AddAttack(tempAttacks[0].ID, tempAttacks[1].ID);

                StartCoroutine(DeactivateCards());
            }
            else
            {
                tempAttacks.Add(cardp);
                //Blink cards and them clear list
                StartCoroutine(DeactivateWrongCards());
            }

            
        }
    }
    /*
     * Using WaitForSeconds to deactivate cards with a little of Delay
     */
    IEnumerator DeactivateCards()
    {
        
        yield return new WaitForSeconds(0.3f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        tempAttacks[1].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        //empty list
        tempAttacks = new List<CardStats>();

    }
    /*
    * Using WaitForSeconds to blink the cards and show you choose a wrong combination
    */
    IEnumerator DeactivateWrongCards()
    {

        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        tempAttacks[1].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(true);
        tempAttacks[1].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(true);
        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        tempAttacks[1].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        //empty list
        tempAttacks = new List<CardStats>();

    }
    /*
    * Using WaitForSeconds to blink the card and show you choose a wrong card
    */
    IEnumerator DeactivateWrongCard()
    {

        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(true);
        yield return new WaitForSeconds(0.2f);
        //Deactive attackMode
        tempAttacks[0].deckSlot.GetComponent<DeckSlot>().ToggleAttackMode(false);
        //empty list
        tempAttacks = new List<CardStats>();

    }
    //function that will create the cards involved in the attack list
    public void SpawnCardsAttackTurn()
    {
        if (turnAttacks.Count > 0)//Check if there is an attack
        {
            Attack a = turnAttacks[attackOrder];

            attackturn_GO.SetActive(true);

            CardStats leftCard, rightCard;
            BoardManager bm = GetComponent<BoardManager>();

            rollToWin.text = a.GetEnoughToWin() + "+";

            //save winner and loser ID
            int LoserID = 1000;//1000 will always be a non existing id

            //Set the right side to the card to appear
            if (bm.GetLeftPlayer().currentDeck.Contains(a.GetAttackOrigin().gameObject))
            {
                leftCard = a.GetAttackOrigin();
                rightCard = a.GetAttackDestiny();

                //Update the roles into the Attack Window
                card_leftRole.text = "Attacker";
                card_rightRole.text = "Defender";

                //Set Winner and Loser
                if (a.GetDice() >= a.GetEnoughToWin())
                {
                    card_leftWinOrLose.color = Color.green;
                    card_leftWinOrLose.text = "Winner";
                    card_rightWinOrLose.color = Color.red;
                    card_rightWinOrLose.text = "Loser";


                    LoserID = rightCard.GetComponent<CardStats>().ID;
                }
                else
                {
                    card_leftWinOrLose.color = Color.red;
                    card_leftWinOrLose.text = "Loser";
                    card_rightWinOrLose.color = Color.green;
                    card_rightWinOrLose.text = "Winner";



                }

            }
            else
            {
                leftCard = a.GetAttackDestiny();
                rightCard = a.GetAttackOrigin();

                //Update the roles into the Attack Window
                card_leftRole.text = "Defender";
                card_rightRole.text = "Attacker";


                //Set Winner and Loser
                if (a.GetDice() >= a.GetEnoughToWin())
                {
                    card_leftWinOrLose.color = Color.red;
                    card_leftWinOrLose.text = "Loser";
                    card_rightWinOrLose.color = Color.green;
                    card_rightWinOrLose.text = "Winner";


                    LoserID = leftCard.GetComponent<CardStats>().ID;
                }
                else
                {
                    card_leftWinOrLose.color = Color.green;
                    card_leftWinOrLose.text = "Winner";
                    card_rightWinOrLose.color = Color.red;
                    card_rightWinOrLose.text = "Loser";



                }

            }

            //Update the attribute relantionship in this attack
            attribute.text = a.GetAttribute() + "";
            //roll dice animation
            finalDiceNumber = a.GetDice();
            isDiceAnimationOn = true;

            left = Instantiate(leftCard.gameObject, card_left.position, card_left.rotation);
            right = Instantiate(rightCard.gameObject, card_right.position, card_right.rotation);

            //remove colliders so i cant interact with triggers
            Destroy(left.GetComponent<BoxCollider>());
            Destroy(right.GetComponent<BoxCollider>());

            //Rescale


            left.transform.parent = card_left;
            right.transform.parent = card_right;

            left.transform.localScale = new Vector3(1, 1, 1);
            right.transform.localScale = new Vector3(1, 1, 1);

            //change the sorting layer so they appear in front of everything
            //Check all cards, the with the right ID will be put in the slot
            foreach (Transform child in left.transform)
            {
                if (child.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    child.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FullCard";
                }
                else if (child.gameObject.GetComponent<TMPro.TextMeshPro>() != null)
                {
                    child.gameObject.GetComponent<TMPro.TextMeshPro>().renderer.sortingLayerName = "FullCard";

                }
            }

            foreach (Transform child in right.transform)
            {
                if (child.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    child.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FullCard";
                }
                else if (child.gameObject.GetComponent<TMPro.TextMeshPro>() != null)
                {
                    child.gameObject.GetComponent<TMPro.TextMeshPro>().renderer.sortingLayerName = "FullCard";

                }
            }

            if (LoserID != 1000) loserList.Add(LoserID);

            //allLeftCards.Add(leftCard.gameObject);
            //allRightCards.Add(rightCard.gameObject);


            //attackWindowAnim.SetTrigger("Enter");
        }
        else
        {
            GetComponent<TurnManager>().SetTurnType(TurnManager.TurnType.PlaceCards);
            attackOrder = 0;

            //return cards to deck
            //Fill Allleftcards and AllRightCards 
            foreach (GameObject o in GetComponent<BoardManager>().GetAllCards())
            {
                //See if parent of parent is either Right or Left
                if (o.transform.parent.parent.gameObject.name == "Right")
                {
                    allRightCards.Add(o);
                }
                if (o.transform.parent.parent.gameObject.name == "Left")
                {
                    allLeftCards.Add(o);
                }
            }

            foreach (GameObject o in allLeftCards)
            {


                //Place Cards back to deck left
                foreach (Transform t in GetComponent<BoardManager>().deckPlaceLeft)
                {
                    //check if the transform is empty
                    if (t.childCount <= 1)//all decks already have "AttackMode" in it.
                    {
                        //pick the father of the card in the attack that must be an BattleDeck
                        GameObject battledeckParent = o.gameObject.transform.parent.gameObject;

                        //make card null if it have less than 1 child (attack mode gameobject)

                        battledeckParent.GetComponent<DeckSlot>().card = null;

                        if (battledeckParent.name.Contains("Battle"))
                        {
                            int indexBattle = int.Parse(battledeckParent.name.Replace("BattleCard_", ""));
                            GetComponent<BoardManager>().SetBattleDeck(indexBattle - 1, 0);
                        }




                        o.transform.position = t.position;
                        o.transform.parent = t.transform;

                        t.GetComponent<DeckSlot>().card = o;
                        int index = int.Parse(t.name.Replace("Card_", ""));
                        GetComponent<BoardManager>().SetDeck(index - 1, o.GetComponent<CardStats>().ID);

                    }
                }
            }
            foreach (GameObject o in allRightCards)
            {


                //Place Cards back to deck right
                foreach (Transform t in GetComponent<BoardManager>().deckPlaceRight)
                {
                    //check if the transform is empty
                    if (t.childCount <= 1)//all decks already have "AttackMode" in it.
                    {
                        //pick the father of the card in the attack that must be an BattleDeck
                        GameObject battledeckParent = o.gameObject.transform.parent.gameObject;

                        //make card null if it have less than 1 child (attack mode gameobject)

                        battledeckParent.GetComponent<DeckSlot>().card = null;

                        if (battledeckParent.name.Contains("Battle"))
                        {
                            int indexBattle = int.Parse(battledeckParent.name.Replace("BattleCard_", ""));
                            GetComponent<BoardManager>().SetBattleDeck(indexBattle - 1, 0);
                        }


                        o.transform.position = t.position;
                        o.transform.parent = t.transform;
                        t.GetComponent<DeckSlot>().card = o;
                        int index = int.Parse(t.name.Replace("Card_", ""));
                        GetComponent<BoardManager>().SetDeck(index - 1, o.GetComponent<CardStats>().ID);

                    }
                }
            }
            //clear list
            allLeftCards.Clear();
            allRightCards.Clear();
        }
    }
    void DiceAnimation()
    {
        //It will check if the animation is not finish yet and if it is on, this will be set on when a new attack is being shown
        if(isDiceAnimationOn)
        {

            diceTimer += Time.deltaTime;
            

            if (diceTimer >= diceTimeInterval && counterDice < 60)
            {
                //Make color of Dice number equals to white
                rollFromDice.color = new Color(1, 1, 1);
                counterDice++;

                //Roll a different number, if its the same roll one above or under
                int rand = Random.Range(1, 6);
                if(int.Parse(rollFromDice.text) == rand)
                {
                    if (rand == 6) rand--;
                    else rand++;
                }
                rollFromDice.text = "" + rand;
                diceTimer = 0;
                if (counterDice == 60) diceTimeInterval = 10 * diceTimeInterval;//it will change the interval in the last loop to times 10

            }
            else if(diceTimer >= diceTimeInterval && counterDice >= 60 && counterDice < 65)
            {
                //Make color of Dice number equals to white
                rollFromDice.color = new Color(1, 1, 1);
                counterDice++;
                //Roll a different number, if its the same roll one above or under
                int rand = Random.Range(1, 6);
                if (int.Parse(rollFromDice.text) == rand)
                {
                    if (rand == 6) rand--;
                    else rand++;
                }
                rollFromDice.text = "" + rand;
                diceTimer = 0;
            }
            else if(diceTimer >= diceTimeInterval && counterDice == 65)
            {
               
                counterDice++;
                //Make the final number yellow
                rollFromDice.color = Color.yellow;
                rollFromDice.text = "" + finalDiceNumber;
                diceTimeInterval = 2 * diceTimeInterval;
                diceTimer = 0;

                //Enable resul of win or lose in cards
                card_rightWinOrLose.gameObject.SetActive(true);
                card_leftWinOrLose.gameObject.SetActive(true);

               
            }
            //Wait a little bit before exiting the AttackScene
            else if (diceTimer >= diceTimeInterval && counterDice > 65)
            {
                resetToDefault();
            }
        }
    }
    //Reset all setting to default, this is being used before showing another attack
    void resetToDefault()
    {
        //Destroy cards in the manager
        //Left
        for(int i = 0; i < card_left.transform.childCount; i++)
        {
            if(card_left.transform.GetChild(i).GetComponent<CardStats>() != null)
            {
                Destroy(card_left.transform.GetChild(i).gameObject);
            }
        }
        //right
        for (int i = 0; i < card_right.transform.childCount; i++)
        {
            if (card_right.transform.GetChild(i).GetComponent<CardStats>() != null)
            {
                Destroy(card_right.transform.GetChild(i).gameObject);
            }
        }
        
        //Reset to default
        isDiceAnimationOn = false;
        diceTimer = 0;
        diceTimeInterval = diceTimeInterval / 20;
        counterDice = 0;
        attackWindowAnim.SetBool("Enter",false);


        card_leftWinOrLose.gameObject.SetActive(false);
        card_rightWinOrLose.gameObject.SetActive(false);

       
        //Check if there is more attacks
        if (turnAttacks.Count > (attackOrder+1))
        {
            attackOrder++;
            //Spawn next attack
            SpawnCardsAttackTurn();
            attackWindowAnim.SetBool("Enter", true);
        }
        else
        {
            //Fill Allleftcards and AllRightCards 
            foreach (GameObject o in GetComponent<BoardManager>().GetAllCards())
            {
                //See if parent of parent is either Right or Left
                if(o.transform.parent.parent.gameObject.name == "Right")
                {
                    allRightCards.Add(o);
                }
                if (o.transform.parent.parent.gameObject.name == "Left")
                {
                    allLeftCards.Add(o);
                }
            }

            foreach (GameObject o in allLeftCards)
            { 

               
                //Place Cards back to deck left
                foreach (Transform t in GetComponent<BoardManager>().deckPlaceLeft)
                {
                    //check if the transform is empty
                    if (t.childCount <= 1)//all decks already have "AttackMode" in it.
                    {
                        //pick the father of the card in the attack that must be an BattleDeck
                        GameObject battledeckParent = o.gameObject.transform.parent.gameObject;
                        
                        //make card null if it have less than 1 child (attack mode gameobject)

                        battledeckParent.GetComponent<DeckSlot>().card = null;

                        if (battledeckParent.name.Contains("Battle"))
                        {
                            int indexBattle = int.Parse(battledeckParent.name.Replace("BattleCard_", ""));
                            GetComponent<BoardManager>().SetBattleDeck(indexBattle - 1, 0);
                        }
                           



                        o.transform.position = t.position;
                        o.transform.parent = t.transform;

                        t.GetComponent<DeckSlot>().card = o;
                        int index = int.Parse(t.name.Replace("Card_", ""));
                        GetComponent<BoardManager>().SetDeck(index - 1, o.GetComponent<CardStats>().ID);
                      
                    }
                }
            }
            foreach (GameObject o in allRightCards)
            {
           

                //Place Cards back to deck right
                foreach (Transform t in GetComponent<BoardManager>().deckPlaceRight)
                {
                    //check if the transform is empty
                    if (t.childCount <= 1)//all decks already have "AttackMode" in it.
                    {
                        //pick the father of the card in the attack that must be an BattleDeck
                        GameObject battledeckParent = o.gameObject.transform.parent.gameObject;

                        //make card null if it have less than 1 child (attack mode gameobject)

                        battledeckParent.GetComponent<DeckSlot>().card = null;

                        if (battledeckParent.name.Contains("Battle"))
                        {
                            int indexBattle = int.Parse(battledeckParent.name.Replace("BattleCard_", ""));
                            GetComponent<BoardManager>().SetBattleDeck(indexBattle - 1, 0);
                        }


                        o.transform.position = t.position;
                        o.transform.parent = t.transform;
                        t.GetComponent<DeckSlot>().card = o;
                        int index = int.Parse(t.name.Replace("Card_", ""));
                        GetComponent<BoardManager>().SetDeck(index - 1, o.GetComponent<CardStats>().ID) ;
                        
                    }
                }
            }
            //clear list
            allLeftCards.Clear();
            allRightCards.Clear();



            //Remove and destroy all losers
            foreach (int i in loserList)
            {
                GetComponent<BoardManager>().DestroyCard(i);

              
            }
            //clear list
            loserList.Clear();

            //remove attack icons
            for(int i = 0; i < attackIcons.Count; ++i)
            {
                Destroy(attackIcons[i]);
            }
            //Clear turn manager
            turnAttacks.Clear();
            //Change turn to place cards again
            GetComponent<TurnManager>().SetTurnType(TurnManager.TurnType.PlaceCards);
            attackOrder = 0;
        }
       


        
    }
    //Check if card already is in the list of attackers, it can't attack twice
    bool CardAlreadyAttacked(CardStats c)
    {
        foreach (Attack a in turnAttacks)
        {
            if(a.GetAttackOrigin() == c)
            {
                return true;
            }
        }
        return false;
    }
    
}
