using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
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
            var playerScoreObjectText = playerScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerScoreObjectText.text = player.NickName + ": " + player.GetScore().ToString();

            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        playerScoreObjectText.text = targetPlayer.NickName + ": " + targetPlayer.GetScore().ToString();
    }
}
