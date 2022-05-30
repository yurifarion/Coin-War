using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
public class SpawnPlayers : MonoBehaviour
{
    public InputField playerUsername;

    public void SetPlayerUsername()
    {
        PlayerPrefs.SetString("username", playerUsername.text);
        //Pick Panel and disable it
        playerUsername.transform.parent.parent.gameObject.SetActive(false);
        //Enable Wait For Player
        PhotonNetwork.LoadLevel("Loading");

    }
  

}
