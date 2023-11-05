using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxKills = 3;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private Text winnerText;

    private void Start()
    {
        PhotonNetwork.Instantiate("Player_Multiplayer", Vector3.zero, Quaternion.identity);
    }
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer.GetScore() == maxKills)
        {
            winnerText.text = targetPlayer.NickName;
            gameOverPopup.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("GameplayScene_Multiplayer");
    }
}
