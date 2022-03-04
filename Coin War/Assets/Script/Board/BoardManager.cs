using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private GameManager _player_left;
    private GameManager _player_right;

    public Text _player_left_username_UI;
    public Text _player_right_username_UI;

    public Transform[] deckPlaceLeft = new Transform[5];
    public Transform[] deckPlaceRight = new Transform[5];

    private List<GameObject> deckListLeft = new List<GameObject>();
    private List<GameObject> deckListRight = new List<GameObject>();

    //Handle Mouse events
    private float lastClick = 0;
    private int clickedCount = 0;
    private bool mouseDown = false;
    private bool onMouseDrag = false;
    private Vector3 initialMousePosition;
    //This gameObjects are to show the card in preview
    [SerializeField]
    private GameObject fullPreviewSpawn;
    public bool gameReady = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
           
            if (!mouseDown)
            {
                initialMousePosition = Input.mousePosition;
                mouseDown = true;
            }
            clickedCount++;
            if(clickedCount == 1) lastClick = Time.time;

            //look for a double click
            if (clickedCount > 1 && Time.time - lastClick < 0.2f)
            {
                // Double click
                Debug.Log(Time.time - lastClick);
                Debug.Log("You double clicked the target.");
                clickedCount = 0;
                lastClick = 0;
            }
            
        }
        else if (clickedCount == 1 && Time.time - lastClick > 0.2f)
        {
            clickedCount = 0;
            Debug.Log("Singular Click");
        }
        if (Input.GetMouseButton(0))
        {
            if(initialMousePosition != Input.mousePosition)
            {
                //On Drag
                if(!mouseDown)onMouseDrag = true;
                Debug.Log("Dragging");
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            if (onMouseDrag)
            {
                Debug.Log("Drop dragging");
                onMouseDrag = false;
            }
           
        }
        /* RaycastHit hit;
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray, out hit, 100.0f))
         {
             //if we click once in a card, let's make it fullscreen
             if (hit.transform.gameObject.GetComponent<DeckSlot>().GetCard() != null)
             {
                 Debug.Log("Something");
                 Vector3 fullsize = new Vector3(0.2f, 0.2f, 0.2f);
                 GameObject o = Instantiate(hit.transform.gameObject.GetComponent<DeckSlot>().GetCard(), Vector3.zero, Quaternion.identity);
                 o.transform.localScale = fullsize;

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
             Debug.Log("Nothing");
             if (fullPreviewSpawn.transform.childCount > 0)
             {
                 Destroy(fullPreviewSpawn.transform.GetChild(0).gameObject);
             }
         }*/

    }
    void DebugStuff()
    {
        Debug.Log("Player Left is: " + _player_left.username+"and he thinks he is in"+_player_left.side);
        Debug.Log("An have this cards");
        foreach(GameObject o in _player_left.currentDeck)
        {
            Debug.Log("Card" + o.GetComponent<CardStats>().cardname);
        }
        Debug.Log("Player right is: " + _player_right.username + "and he thinks he is in" + _player_right.side);
        Debug.Log("An have this cards");
        foreach (GameObject o in _player_right.currentDeck)
        {
            Debug.Log("Card" + o.GetComponent<CardStats>().cardname);
        }
    }
    void PutCards(GameManager left, GameManager right)
    {
        for (int i = 0; i < 5; ++i)
        {

            GameObject l = Instantiate(left.currentDeck[i], deckPlaceLeft[i].position, deckPlaceLeft[i].rotation);
            deckListLeft.Add(l);
            deckPlaceLeft[i].GetComponent<DeckSlot>().SetCard(l);
            l.transform.parent = deckPlaceLeft[i].transform;
            GameObject r = Instantiate(right.currentDeck[i], deckPlaceRight[i].position, deckPlaceRight[i].rotation);
            deckListRight.Add(r);
            deckPlaceRight[i].GetComponent<DeckSlot>().SetCard(r);
            r.transform.parent = deckPlaceRight[i].transform;

        }
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
            else if (_player_left != null && _player_right != null && deckListLeft.Count < 5 && deckListRight.Count < 5)
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
