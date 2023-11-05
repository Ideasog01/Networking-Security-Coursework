using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private Transform panel;

    private Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();

    private void Start()
    {
        foreach(var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);

            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
            playerScoreObjectText.text = string.Format("{0} Score: {1}", player.NickName, player.GetScore());

            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
        playerScoreObjectText.text = string.Format("{0} Score: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }
}
