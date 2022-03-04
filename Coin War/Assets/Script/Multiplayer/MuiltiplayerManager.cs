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

    public void Start()
    {
        Debug.Log("Start Number of Players in room" + PhotonNetwork.PlayerList.Length);
        //Check if we are the first to get in the room
        if (PhotonNetwork.PlayerList.Length >= 2)
        {
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
            //if another player enter the room we will need to add its pieces to our board after a delay
            StartCoroutine(SpawnGameManager("Left"));
        }
    }
    IEnumerator SpawnGameManager(string side)
    {
        Debug.Log("SpawnGameManager");
        yield return new WaitForSeconds(2f);//This delay is just to make sure the other client connects
        GameObject o = PhotonNetwork.Instantiate(gameManagerPrefab.name, transform.position, Quaternion.identity);
        o.GetComponent<GameManager>().UpdateUsername();
        o.GetComponent<GameManager>().UpdateSide(side);
        o.GetComponent<GameManager>().PlayerIsReady();

        Destroy(WaitingForPlayer);//Clear scene to game
    }
}
