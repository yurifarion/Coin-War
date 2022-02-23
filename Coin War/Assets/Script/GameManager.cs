using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public string username;

    public string side;

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
    [PunRPC]
    public void PhotonPlayerIsReady(bool b)
    {
        playerReady = b;
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
        if (GetComponent<PhotonView>().IsMine)
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
