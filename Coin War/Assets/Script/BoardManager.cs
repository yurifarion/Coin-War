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

    public bool gameReady = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FillWithPlayers();
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
                gameReady = _player_left.IsPlayerReady() && _player_right.IsPlayerReady();

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
