using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public string username;

    public string side = "";
    public List<GameObject> allCards = new List<GameObject>();
    public List<GameObject> currentDeck = new List<GameObject>();

    //This will be set to true when both players are ready to start.
    private bool playerReady = false;
    //Enemy
    private GameManager enemy;
    // Start is called before the first frame update
    void Start()
    {
        UIinit();
    }
    private void Update()
    {
        //Will try to find another GameManager that is not itself
        if(enemy == null)
        {
            GameObject[] gms = GameObject.FindGameObjectsWithTag("GameManager");
            foreach(GameObject g in gms)
            {
                if(g.GetComponent<GameManager>() != null && g.GetComponent<GameManager>() != this)
                {
                    enemy = g.GetComponent<GameManager>();
                }
            }
        }

    }
    public bool IsPlayerReady()
    {
        return playerReady;
    }
    public void PlayerIsReady()
    {
        GetComponent<PhotonView>().RPC("PhotonPlayerIsReady", RpcTarget.AllBuffered, true);
    }
    public void ShuffleCards(int[] cardIndex)
    {
        currentDeck.Clear();
        for (int i = 0; i < 5; ++i)
        {
           
            currentDeck.Add(allCards[cardIndex[i]]);
        }
    }
    [PunRPC]
    public void SetCardIndex(int[] cardIndex)
    {
        //receive a shuffle of cards that is int vector in which each number is a different card type
        //and then send it to all clients
        Debug.Log("Cards shuffle:" + cardIndex+"From Player: "+username);
        ShuffleCards(cardIndex);
    }
    [PunRPC]
    public void PhotonPlayerIsReady(bool b)
    {
        playerReady = b;

        //It will first update username for all clients
        if (GetComponent<PhotonView>().IsMine)
        {
            //Shuffle deck
            int count = 0;
            List<int> tempCards = new List<int>();
            while (count < 5)
            {
                int rand = Random.Range(0, allCards.Count);

                if (!tempCards.Contains(rand))
                {
                    tempCards.Add(rand);
                    count++;
                }

            }
            int[] tempCardsConvert = tempCards.ToArray();
            GetComponent<PhotonView>().RPC("SetCardIndex", RpcTarget.AllBuffered, (object)tempCardsConvert);
        }
    }
    void UIinit()
    {
        //Search for the Background that has the tag 'UI, because all other UI elements will be its child
        GameObject parent = GameObject.FindGameObjectWithTag("UI");

        

    }
    public void SetEnemy(GameManager gm)
    {
        enemy = gm;
    }
    public void UpdateUsername()
    {
        //It will first update username for all clients
        if (GetComponent<PhotonView>().IsMine)
        {
            string userNameFromMemory = PlayerPrefs.GetString("username", "None");
            GetComponent<PhotonView>().RPC("PUNUpdateUsername", RpcTarget.AllBuffered, userNameFromMemory);
        }
    }
    public void UpdateSide(string s)
    {
        //It will first update username for all clients
        if (GetComponent<PhotonView>().IsMine && side == "")
        {
            GetComponent<PhotonView>().RPC("PUNUpdateSide", RpcTarget.AllBuffered, s);
        }
       
    }

    //Update the username for all clients
    [PunRPC]
    void PUNUpdateUsername(string u)
    {
        username = u;
    }
    //Update the side for all clients
    [PunRPC]
    void PUNUpdateSide(string s)
    {
        side = s;
    }

}
