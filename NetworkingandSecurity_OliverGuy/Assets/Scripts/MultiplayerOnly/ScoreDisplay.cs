using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class ScoreDisplay : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject playerScorePrefab; //Includes a background and text for displaying score on heads-up-display
        [SerializeField] private Transform panel; //The parent for all score display objects

        private Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();

        private void Start()
        {
            foreach (var player in PhotonNetwork.PlayerList) //Instantiate a score display object for each player in the room. This will then be updated when a player's score changes.
            {
                player.SetScore(0);

                var playerScoreObject = Instantiate(playerScorePrefab, panel);
                var playerScoreObjectText = playerScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                playerScoreObjectText.text = player.NickName + ": " + player.GetScore().ToString();

                playerScore.Add(player.ActorNumber, playerScoreObject); //For access later
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) //Updates each score value across all clients whenever one is updated
        {
            var playerScoreObject = playerScore[targetPlayer.ActorNumber];
            var playerScoreObjectText = playerScoreObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerScoreObjectText.text = targetPlayer.NickName + ": " + targetPlayer.GetScore().ToString();
        }
    }
}
