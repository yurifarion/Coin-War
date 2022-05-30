using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnType {GettingReady,PlaceCards,PlaceAttack,Attack,EndGame};

    [SerializeField]
    private Text countDownText;
    [SerializeField]
    private Text turnNameText;
    [SerializeField]
    private Text turnNameTextTransition;
    [SerializeField]
    private GameObject turnTransition;

    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    [SerializeField] double timeToRespond = 90;
    ExitGames.Client.Photon.Hashtable CustomeValue;

    [SerializeField]
    private TurnType currentTurn;

    private int speedOfturn;
    // Start is called before the first frame update
    void Start()
    {

       

        currentTurn = TurnType.GettingReady;


        //Sync the timer based in the time of Photon server
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Clear();
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }
    public void PassTurn()
    {
        GetComponent<BoardManager>().GetMaster().SetPlayerIsReady(true);
    }
    private void Update()
    {
        if (currentTurn == TurnType.PlaceCards)
        {
            turnNameText.text ="Place your Cards of speed: "+GetSpeedTurn();
            turnNameTextTransition.text = "Place your Cards!";
        }
        else if (currentTurn == TurnType.PlaceAttack)
        {
            turnNameText.text = "Place your attacks: ";
            turnNameTextTransition.text = "Place your attacks!";
        }
        else if (currentTurn == TurnType.Attack)
        {
            turnNameText.text = "Attacks!";
            turnNameTextTransition.text = "Attacks!";
        }

        if (!startTimer) return;

        timerIncrementValue = timeToRespond -(PhotonNetwork.Time - startTime);
        countDownText.text = (int)timerIncrementValue + "s";

        if (timerIncrementValue <= 0)
        {
            //Timer Completed
            PassTurn();
        }


        //make sure to upadate the speed of turn if not yet
        if(currentTurn == TurnType.PlaceCards && speedOfturn == 0)
        {
            ChangeSpeedTurn(3);
        }
       
        if (GetComponent<BoardManager>().GetLeftPlayer() != null && GetComponent<BoardManager>().GetRightPlayer() != null)
        {
            if (GetComponent<BoardManager>().GetLeftPlayer().IsPlayerReady() || GetComponent<BoardManager>().GetRightPlayer().IsPlayerReady())
            {
               
            }
            //If both players are ready is time to pass turns
            bool bothPlayersReady = GetComponent<BoardManager>().GetLeftPlayer().IsPlayerReady() && GetComponent<BoardManager>().GetRightPlayer().IsPlayerReady();
            if (bothPlayersReady)
            {
               
                GetComponent<BoardManager>().GetLeftPlayer().SetPlayerIsReady(false);
                GetComponent<BoardManager>().GetRightPlayer().SetPlayerIsReady(false);

                if (GetTurnType() == TurnType.GettingReady)
                {
                    SetTurnType(TurnType.PlaceCards);
                   
                }
                else if (GetTurnType() == TurnType.PlaceCards) SetTurnType(TurnType.PlaceAttack);
                else if (GetTurnType() == TurnType.PlaceAttack)
                {
                    SetTurnType(TurnType.Attack);
                    

                }

            }
        }
    }
    public void SetTurnType(TurnType tp)
    {

        //Since Photon can't work with Enum we turn it into a string
        GetComponent<PhotonView>().RPC("PhotonSetTurnType", RpcTarget.AllBuffered, TurnTypeToString(tp));
    }
    [PunRPC]
    public void PhotonSetTurnType(string s)
    {
        
        //currentTurn = StringToTurnType(s);
        if(currentTurn != StringToTurnType(s) && StringToTurnType(s) == TurnType.Attack)
        {
            currentTurn = StringToTurnType(s);
            turnTransition.SetActive(true);

            //play animation of attack
            GetComponent<AttackManager>().attackWindowAnim.SetBool("Enter",true);

            //Speed of turn dictates which card can be handled, it goes from 3 to 1 and then to 3 again.
            Debug.Log("On attack change from" + speedOfturn + " To " + (speedOfturn - 1));
            if ((speedOfturn - 1) == 0)
            {
                Debug.Log("SpeedOfTurn -1 is equal to zero");
                ChangeSpeedTurn(3);
            }
            else
            {
                Debug.Log("SpeedOfTurn -1 is not equal to zero");
                ChangeSpeedTurn((speedOfturn - 1));
            }

            //Now we can make available to check win equals to true
            GetComponent<BoardManager>().GetLeftPlayer().SetAvailableToCheckWin(true);
            GetComponent<BoardManager>().GetRightPlayer().SetAvailableToCheckWin(true);
        }
        else
        {
            currentTurn = StringToTurnType(s);
            turnTransition.SetActive(true);
        }

        if(currentTurn == TurnType.PlaceCards && speedOfturn == 0) ChangeSpeedTurn(3);

        //only shows timer if turn is place cards or place attack
        bool TurnToClock = (currentTurn == TurnType.PlaceCards) || (currentTurn == TurnType.PlaceAttack);
        countDownText.gameObject.SetActive(TurnToClock);

        if (currentTurn == TurnType.Attack)
        {
            GetComponent<AttackManager>().SpawnCardsAttackTurn();
        }

        resetTimer();

        
    }
    void resetTimer()
    {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            CustomeValue.Clear();
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
      
    }
    public TurnType GetTurnType()
    {
       return currentTurn;
    }
    string TurnTypeToString(TurnType tp)
    {
        
        return tp + "";
    }
    TurnType StringToTurnType(string s)
    {

        TurnType parsed_enum = (TurnType)System.Enum.Parse(typeof(TurnType), s);

        return parsed_enum;
    }
    public int GetSpeedTurn()
    {
        return speedOfturn;
    }
    //Check if there is at least one card of a specific speed
    void ChangeSpeedTurn(int s)
    {
        Debug.Log("Checking speed" + s+ "All Deck"+ GetComponent<BoardManager>().GetAllCards().Count);
        foreach (GameObject o in GetComponent<BoardManager>().GetAllCards())
        {
            if (o.GetComponent<CardStats>().speedLevel == s)
            {
                speedOfturn = s;
                Debug.Log("Found cards with Speed: "+s);
                return;
            }
        }
        if ((s - 1) != 0)
        {
            foreach (GameObject o in GetComponent<BoardManager>().GetAllCards())
            {
                if (o.GetComponent<CardStats>().speedLevel == (s - 1))
                {
                    speedOfturn = s - 1;
                    Debug.Log("Found cards with Speed: " + (s - 1));
                    return;
                }
            }

            if ((s - 2) != 0)
            {
                foreach (GameObject o in GetComponent<BoardManager>().GetAllCards())
                {
                    if (o.GetComponent<CardStats>().speedLevel == (s - 2))
                    {
                        speedOfturn = s - 1;
                        Debug.Log("Found cards with Speed: " + (s - 2));
                        return;
                    }
                }
            }
        }

        Debug.Log("Dont exist anycard with any level speed");


    }
    

}
