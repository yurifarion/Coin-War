using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    public void PlayAgain()
    {
        StartCoroutine(DoSwitchLevel("Loading"));


    }

    IEnumerator DoSwitchLevel(string level)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(level);
    }
}
