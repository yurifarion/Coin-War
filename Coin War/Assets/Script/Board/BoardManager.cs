using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameManager Master;
    private GameManager _player_left;
    private GameManager _player_right;

    public Text _player_left_username_UI;
    public Text _player_right_username_UI;

    //Isready Icons
    public GameObject _player_left_ready;
    public GameObject _player_right_ready;

    public Transform[] deckPlaceLeft = new Transform[5];
    public Transform[] deckPlaceRight = new Transform[5];

    //This arrays works in a way to communicate the clients
    // 0 means the is no card in the slot
    // and then it will save the number of the id of the card
    private int[] deckIndex = new int[10];
    private int[] battleIndex = new int[6];

    //List saving GameObject for the card slots and all cards
    [SerializeField]
    private List<GameObject> DeckCardSlots = new List<GameObject>();
    [SerializeField]
    private List<GameObject> BattleCardSlots = new List<GameObject>();
    private List<GameObject> allCards = new List<GameObject>();

    //Handle Mouse events
    private float lastClick = 0;
    private int clickedCount = 0;
    private bool mouseDown = false;
    private Vector3 initialMousePosition;

    //This gameObjects are to show the card in preview
    [SerializeField]
    private GameObject fullPreviewSpawn;
    public bool gameReady = false;

    //Card to manage
    public GameObject selectedCard;


   
    public void UpdateIndexes()
    {
        GetComponent<PhotonView>().RPC("PhotonUpdateIndexes", RpcTarget.AllBuffered, deckIndex,battleIndex);
    }
    //Updates all indexes to make sure all cards are in the same spot ans sync
    [PunRPC]
    void PhotonUpdateIndexes(int[] deckIndexp, int[] BattleIndexp)
    {
        if (deckIndexp != deckIndex || BattleIndexp != battleIndex)
        {
            deckIndex = deckIndexp;
            battleIndex = BattleIndexp;

            
            //Update the position of all cards in the board
            UpdatePositionOfCards();
        }
    }

    // Update is called once per frame
    void Update()
    {

        
        UpdateReadyIcon();
        GetMouseCommands();
        FillWithPlayers();
        if (Input.GetKeyDown(KeyCode.K))
        {
            DebugStuff();
        }
    }
    /*
     * This Function will handle all the mouse commands
     * like click to fullscree the card, drag and drop the card, etc
     */
    void GetMouseCommands()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            //If fullscreen card is on screen let's minimize it
            if (fullPreviewSpawn.transform.childCount > 0)
            {
                Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
            }
            if (!mouseDown)
            {
                initialMousePosition = Input.mousePosition;
                mouseDown = true;
            }
            clickedCount++;
            if(clickedCount == 1) lastClick = Time.time;

            //look for a double click
            if (clickedCount > 1 && Time.time - lastClick < 0.2f )
            {
                
                CheckClickCard();
             
                clickedCount = 0;
                lastClick = 0;
            }
            
        }
        //Click once and its place attack turn
        else if (clickedCount == 1 && Time.time - lastClick > 0.2f )
        {

            //Single click
            clickedCount = 0;
            if(GetComponent<TurnManager>().GetTurnType() == TurnManager.TurnType.PlaceAttack) CheckAttackMode();


        }
        if (Input.GetMouseButton(0))
        {
            //Drag if position is diferent and turn is place cards
            if(initialMousePosition != Input.mousePosition && GetComponent<TurnManager>().GetTurnType() == TurnManager.TurnType.PlaceCards)
            {
                
                //If it's not already selected, select card
                if (selectedCard == null) CheckCardDrag();

                if (selectedCard != null)
                {
                    //Make card follows mouse
                    Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z - transform.position.z)));
                    newPos.z = selectedCard.transform.position.z;

                    selectedCard.transform.position = newPos;

                    //Uncheck the attackmode if its true
                    DeckSlot currentSlot = selectedCard.GetComponent<CardStats>().deckSlot.GetComponent<DeckSlot>();

                    if (currentSlot.GetAttackMode())
                    {
                        currentSlot.ToggleAttackMode(false);
                    }
                }


            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            if (selectedCard != null)
            {
                //changes the layer of all childs that have "SpriteRenderer" component

                foreach (Transform child in selectedCard.transform)
                {
                    if (child.gameObject.GetComponent<SpriteRenderer>() != null)
                    {
                        //child.gameObject.GetComponent<SpriteRenderer>().sortingOrder += 4;
                        child.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                    }
                    else if (child.gameObject.GetComponent<TMPro.TextMeshPro>() != null)
                    {
                        child.gameObject.GetComponent<TMPro.TextMeshPro>().renderer.sortingLayerName = "Default";
                        //child.gameObject.GetComponent<TMPro.TextMeshPro>().sortingLayerName = "FullCard";
                    }
                }
                //Make card go to its deck position and then drop it from Selected Card
                selectedCard.transform.position = selectedCard.GetComponent<CardStats>().deckSlot.transform.position;
                selectedCard = null;
                
                
            }
           
        }
       

    }
    void CheckAttackMode()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.transform.gameObject.GetComponent<DeckSlot>().GetCard() != null)
            { 
                 hit.transform.gameObject.GetComponent<DeckSlot>().ToggleAttackMode();
                 GetComponent<AttackManager>().AddTempAttack(hit.transform.gameObject.GetComponent<DeckSlot>().GetCard().GetComponent<CardStats>());
            }

        }
        //if not, but we have one card fullscreen, let's minimize it
        else
        {
           
            if (fullPreviewSpawn.transform.childCount > 0)
            {
                Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
            }
        }
    }
    void CheckCardDrag()
    {
        

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            //if we click once in a card , let's turn it in Attack Mode
            if (hit.transform.gameObject.GetComponent<DeckSlot>().GetCard() != null)
            {

                if (hit.transform.gameObject.GetComponent<DeckSlot>().GetCard().GetComponent<CardStats>().speedLevel == GetComponent<TurnManager>().GetSpeedTurn())
                {
                    if (GetComponent<BoardManager>().GetMaster().currentDeck.Contains(hit.transform.gameObject.GetComponent<DeckSlot>().GetCard())) {
                        selectedCard = hit.transform.gameObject.GetComponent<DeckSlot>().GetCard();
                        //changes the layer of all childs that have "SpriteRenderer" component

                        foreach (Transform child in selectedCard.transform)
                        {
                            if (child.gameObject.GetComponent<SpriteRenderer>() != null)
                            {
                                //child.gameObject.GetComponent<SpriteRenderer>().sortingOrder += 4;
                                child.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FullCard";
                            }
                            else if (child.gameObject.GetComponent<TMPro.TextMeshPro>() != null)
                            {
                                child.gameObject.GetComponent<TMPro.TextMeshPro>().renderer.sortingLayerName = "FullCard";
                                //child.gameObject.GetComponent<TMPro.TextMeshPro>().sortingLayerName = "FullCard";
                            }
                        }
                    }
                }
                
            }

        }
        //if not, but we have one card fullscreen, let's minimize it
        else
        {
            
            if (fullPreviewSpawn.transform.childCount > 0)
            {
                Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
            }
        }
    }
    void CheckClickCard()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            //if we click twice in a card, let's make it fullscreen
            if (hit.transform.gameObject.GetComponent<DeckSlot>().GetCard() != null)
            {
               
                Vector3 fullsize = new Vector3(0.15f, 0.15f, 0.15f);
                GameObject o = Instantiate(hit.transform.gameObject.GetComponent<DeckSlot>().GetCard(), Vector3.zero, Quaternion.identity);
                //remove colliders so i cant interact with triggers
                Destroy(o.GetComponent<BoxCollider>());
                o.transform.localScale = fullsize;
                
                //changes the layer of all childs that have "SpriteRenderer" component
                
                foreach (Transform child in o.transform)
                {
                    if(child.gameObject.GetComponent<SpriteRenderer>() != null)
                    {
                        //child.gameObject.GetComponent<SpriteRenderer>().sortingOrder += 4;
                        child.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "FullCard";
                    }
                    else if(child.gameObject.GetComponent<TMPro.TextMeshPro>() != null)
                    {
                        child.gameObject.GetComponent<TMPro.TextMeshPro>().renderer.sortingLayerName = "FullCard";
                        //child.gameObject.GetComponent<TMPro.TextMeshPro>().sortingLayerName = "FullCard";
                    }
                }

                if (fullPreviewSpawn.transform.childCount > 0)
                {
                    Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
                }
                o.transform.parent = fullPreviewSpawn.transform;
            }

        }
        //if not, but we have one card fullscreen, let's minimize it
        else
        {
            
            if (fullPreviewSpawn.transform.childCount > 0)
            {
                Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
            }
        }
    }
    void DebugStuff()
    {
        Debug.Log("Player Left is: " + _player_left.username+"and he thinks he is in"+_player_left.side);
        Debug.Log("An have this cards");
        foreach(GameObject o in _player_left.currentDeck)
        {
            Debug.Log("Card" + o.GetComponent<CardStats>().cardname + "ID: " + o.GetComponent<CardStats>().ID);
        }
        Debug.Log("Player right is: " + _player_right.username + "and he thinks he is in" + _player_right.side);
        Debug.Log("An have this cards");
        foreach (GameObject o in _player_right.currentDeck)
        {
            Debug.Log("Card" + o.GetComponent<CardStats>().cardname+"ID: "+ o.GetComponent<CardStats>().ID);
        }
        for(int i = 0; i < 10; ++i)
        {
            Debug.Log("Deck in" + i + "position is " + deckIndex[i]);
            
        }
        for (int i = 0; i < 6; ++i)
        {
            Debug.Log("Battle in" + i + "position is " + battleIndex[i]);
        }
    }
    void PutCards(GameManager left, GameManager right)
    {
        for (int i = 0; i < 5; ++i)
        {
            GameObject l = Instantiate(left.currentDeck[i], deckPlaceLeft[i].position, deckPlaceLeft[i].rotation);
            //deckListLeft.Add(l);
            deckPlaceLeft[i].GetComponent<DeckSlot>().SetCard(l);
            l.GetComponent<CardStats>().deckSlot = deckPlaceLeft[i].gameObject;
            l.transform.parent = deckPlaceLeft[i].transform;
            string IDleft = l.GetComponent<CardStats>().deckSlot.name.Replace("Card_", "");
            l.GetComponent<CardStats>().ID = int.Parse(IDleft);
            left.currentDeck[i] = l;
            SetDeck(i,l.GetComponent<CardStats>().ID);
            allCards.Add(l);

            GameObject r = Instantiate(right.currentDeck[i], deckPlaceRight[i].position, deckPlaceRight[i].rotation);
           // deckListRight.Add(r);
            deckPlaceRight[i].GetComponent<DeckSlot>().SetCard(r);
            r.GetComponent<CardStats>().deckSlot = deckPlaceRight[i].gameObject;
            r.transform.parent = deckPlaceRight[i].transform;
            string IDright = r.GetComponent<CardStats>().deckSlot.name.Replace("Card_", "");
            r.GetComponent<CardStats>().ID = int.Parse(IDright);
            right.currentDeck[i] = r;
            SetDeck(i+5,r.GetComponent<CardStats>().ID);
            allCards.Add(r);
        }
        UpdateIndexes();
        gameReady = true;

       
    }
    //This function will be activated everyframe until this class have 2 GameManagers
    void FillWithPlayers()
    {
        if (!gameReady)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("GameManager");

            if(players.Length >= 2 && _player_left == null && _player_right == null)
            {
                if(players[0].GetComponent<GameManager>().side == "Left")
                {
                    _player_left = players[0].GetComponent<GameManager>();
                    _player_right = players[1].GetComponent<GameManager>();
                }
                else
                {
                    _player_left = players[1].GetComponent<GameManager>();
                    _player_right = players[0].GetComponent<GameManager>();
                }
               
            }
            else if (_player_left != null && _player_right != null)
            {
                //Make sure that both players have 5 cards in the deck
                if (_player_left.currentDeck.Count == 5 && _player_right.currentDeck.Count == 5)
                {
                    // gameReady = _player_left.IsPlayerReady() && _player_right.IsPlayerReady();
                    PutCards(_player_left, _player_right);
                    //Update UI with the usernames
                    if (gameReady)
                    {
                        _player_left_username_UI.text = _player_left.username;
                        _player_right_username_UI.text = _player_right.username;
                    }
                }

                
            }

            
        }
    }
    public void UpdatePositionOfCards()
    {
      
        //Update Deck part of Board
        for(int i = 0; i < 10; ++i)
        {
            //First clear card space in Deck
            DeckCardSlots[i].GetComponent<DeckSlot>().SetCard(null);
            if (deckIndex[i] != 0)
            {
               
                //Check all cards, the with the right ID will be put in the slot
                foreach (GameObject o in allCards)
                {
                    if(o.GetComponent<CardStats>().ID == deckIndex[i])
                    {
                        DeckCardSlots[i].GetComponent<DeckSlot>().SetCard(o);
                        o.GetComponent<CardStats>().deckSlot = DeckCardSlots[i];
                        o.transform.position = DeckCardSlots[i].gameObject.transform.position;
                        o.transform.parent = DeckCardSlots[i].gameObject.transform;
                        //o.transform.localPosition = Vector3.zero;
                    }
                }

               
            }
           
        }
        for (int i = 0; i < 6; ++i)
        {
            //First clear card space in Deck
            BattleCardSlots[i].GetComponent<DeckSlot>().SetCard(null);
            if (battleIndex[i] != 0)
            {
               
                //Check all cards, the with the right ID will be put in the slot
                foreach (GameObject o in allCards)
                {
                    if (o.GetComponent<CardStats>().ID == battleIndex[i])
                    {
                        BattleCardSlots[i].GetComponent<DeckSlot>().SetCard(o);
                        o.GetComponent<CardStats>().deckSlot = BattleCardSlots[i];
                        o.transform.position = BattleCardSlots[i].gameObject.transform.position;
                        o.transform.parent = BattleCardSlots[i].gameObject.transform;

                    }
                }

               
            }
        }

    }
    void UpdateReadyIcon()
    {
        if (_player_left != null && _player_right_ready != null)
        {
            _player_left_ready.SetActive(_player_left.IsPlayerReady());
            _player_right_ready.SetActive(_player_right.IsPlayerReady());
        }

    }
    //GET AND SET to indexes
    public int[] GetDeckIndexes()
    {
        return deckIndex;
    }
    public void SetDeck(int index, int cardID)
    {
        deckIndex[index] = cardID;
    }
    
    public int[] GetBattleIndexes()
    {
        return battleIndex;
    }
    public void SetBattleDeck(int index, int cardID)
    {
        battleIndex[index] = cardID;
    }
    
    public void SetMaster(GameManager gm)
    {
        Master = gm;
        Debug.Log("Master is " + Master.name);
    }
    public GameManager GetMaster()
    {
        return Master;
    }
    public GameManager GetLeftPlayer()
    {
        return _player_left;
    }
    public GameManager GetRightPlayer()
    {
        return _player_right;
    }
    public CardStats GetCardFromID(int id)
    {
        foreach(GameObject c in allCards)
        {
            if(c.GetComponent<CardStats>().ID == id)
            {
                return c.GetComponent<CardStats>();
            }
        }
        return null;
    }
    //Destroy Card in Allcards and also in "CurrentDeck" in GameManager
    public void DestroyCard(int id)
    {


        //Check if the card exist in the array first
        if (System.Array.IndexOf(GetDeckIndexes(), id) >= 0)
        {
            SetDeck(System.Array.IndexOf(GetDeckIndexes(), id), 0);

            GameObject o = new GameObject();


            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].GetComponent<CardStats>().ID == id)
                {
                    o = allCards[i];
                    //remove from list
                    allCards.Remove(o);
                }
            }


            for (int i = 0; i < _player_left.currentDeck.Count; i++)
            {
                if (_player_left.currentDeck[i].GetComponent<CardStats>().ID == id)
                {
                    o = _player_left.currentDeck[i];
                    //remove from list
                    _player_left.currentDeck.Remove(o);

                }
            }

            for (int i = 0; i < _player_right.currentDeck.Count; i++)
            {
                if (_player_right.currentDeck[i].GetComponent<CardStats>().ID == id)
                {
                    o = _player_right.currentDeck[i];
                    //remove from list
                    _player_right.currentDeck.Remove(o);

                }
            }

            Destroy(o);
        }
    }
    public List<GameObject> GetAllCards()
    {
        return allCards;
    }
   
}
