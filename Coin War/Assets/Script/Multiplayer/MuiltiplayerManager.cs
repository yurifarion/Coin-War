using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class MuiltiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject gameManagerPrefab;
  
    [SerializeField]
    private GameObject WaitingForPlayer;

    private BoardManager _board;

    public void Start()
    {

      

        Debug.Log("Start Number of Players in room" + PhotonNetwork.PlayerList.Length);
        //Check if we are the first to get in the room
        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            Debug.Log("Right");
            //if another player enter the room we will need to add its pieces to our board after a delay
            StartCoroutine(SpawnGameManager("Right"));
        }
    }
    //Check if another player entered our Room
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.Log("Entered Number of Players in room" + PhotonNetwork.PlayerList.Length);
        //Check if we are the first to get in the room
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            Debug.Log("Left");
            //if another player enter the room we will need to add its pieces to our board after a delay
            StartCoroutine(SpawnGameManager("Left"));
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        PhotonNetwork.LoadLevel("GameWin");
    }
    IEnumerator SpawnGameManager(string side)
    {
        _board = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();

        yield return new WaitForSeconds(2f);//This delay is just to make sure the other client connects
        GameObject o = PhotonNetwork.Instantiate(gameManagerPrefab.name, transform.position, Quaternion.identity);
        o.GetComponent<GameManager>().UpdateUsername();
        o.GetComponent<GameManager>().UpdateSide(side);
        o.GetComponent<GameManager>().InitPlayer();
        o.GetComponent<GameManager>().SetPlayerIsReady(true);

        _board.SetMaster(o.GetComponent<GameManager>());

        Destroy(WaitingForPlayer);//Clear scene to game
    }
}
