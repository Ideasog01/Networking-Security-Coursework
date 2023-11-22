using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        //Allows for global access across multiple scripts
        public static MultiplayerChatDisplay MultiplayerChatDisplay;
        public static RespawnManager RespawnManager;
        public static PlayerDisplay PlayerDisplay;
        public static PlayerController PlayerController;
        public static CameraTracking CameraTracking;
        public static SaveManager SaveManager;

        public static bool GameInProgress;

        [Header("Game Objective")]

        [SerializeField] private int maxScore;
        [SerializeField] private float matchTimeInSeconds;

        [Header("End Screen")]

        [SerializeField] private GameObject victoryCanvas;
        [SerializeField] private GameObject defeatCanvas;
        [SerializeField] private GameObject drawCanvas;
        [SerializeField] private TextMeshProUGUI playerWonText; //Displayed on the defeat screen for other players to be informed who won.
        [SerializeField] private GameObject[] restartGameButtons;

        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseCanvas;

        private PhotonView _photonView;

        private float _matchTimer;

        private void Start()
        {
            //Assign global scripts
            MultiplayerChatDisplay = this.GetComponent<MultiplayerChatDisplay>();
            RespawnManager = this.GetComponent<RespawnManager>();
            PlayerDisplay = this.GetComponent<PlayerDisplay>();
            CameraTracking = Camera.main.GetComponent<CameraTracking>();
            SaveManager = this.GetComponent<SaveManager>();

            GameInProgress = true;
            _matchTimer = 0;

            //Instantiate the player and assign all neccessary values
            GameObject obj = PhotonNetwork.Instantiate("Player_Multiplayer", RespawnManager.SpawnPositionArray[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
            PlayerController = obj.transform.GetChild(0).GetComponent<PlayerController>();
            CameraTracking.TargetTransform = PlayerController.transform;
            _photonView = this.GetComponent<PhotonView>();
            PhotonNetwork.LocalPlayer.SetScore(0); //In case the score value still persists from a previous match

            Debug.Log("Player Count: " + PhotonNetwork.LocalPlayer.ActorNumber);
            MultiplayerChatDisplay.CallNewMessage(PhotonNetwork.LocalPlayer.NickName + " joined the game.", false);

            if(PhotonNetwork.IsMasterClient)
            {
                InitialiseTimer(); //The game timer that will decrease every second
            }
        }

        #region End Screen Buttons

        public void LeaveGame() //Via Inspector (Button)
        {
            if (PhotonNetwork.PlayerList.Length == 2) //If there will be only one player left after we leave, declare the winner to be the only remaining player
            {
                foreach (Player player in PhotonNetwork.PlayerList) //Find the remaining player
                {
                    if (player != PhotonNetwork.LocalPlayer)
                    {
                        //Sets the player to be the winner
                        player.SetScore(int.MaxValue);
                        _photonView.RPC("FindWinner", RpcTarget.Others);
                    }
                }
            }

            PhotonNetwork.Disconnect();
        }

        public void RestartGame() //Via Inspector (Button) (Only Master Client)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.LoadLevel("LoadingScene"); //The loading scene will be loaded to properly reload the gameplay scene across all clients.
        }

        #endregion

        #region Network Utilities

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (targetPlayer.IsLocal)
            {
                PlayerDisplay.UpdateEliminationDisplay(targetPlayer.GetScore(), maxScore); //Update the score display

                if (targetPlayer.GetScore() == maxScore) //If the score reached maximum, declare the winner on all clients
                {
                    _photonView.RPC("FindWinner", RpcTarget.AllViaServer);
                    StorePersonalBest();
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //Update the GameInProgress value to be the same on all clients
        {
            if (stream.IsWriting)
            {
                stream.SendNext(GameInProgress);
                stream.SendNext(_matchTimer);
            }
            else if (stream.IsReading)
            {
                GameInProgress = (bool)stream.ReceiveNext();
                _matchTimer = (float)stream.ReceiveNext();
            }
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("MainMenu");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.PlayerList.Length == 1) //If there is only one player remaining, declare this player the winner.
            {
                if (GameInProgress) //In the case that the game has already finished
                {
                    DisplayEndScreen(PhotonNetwork.LocalPlayer);
                }

                //We do not want to give the ability to restart the game, as there are no players left!
                restartGameButtons[0].SetActive(false);
                restartGameButtons[1].SetActive(false);
                restartGameButtons[2].SetActive(false);
            }

            MultiplayerChatDisplay.CallNewMessage(otherPlayer.NickName + " left the game.", true);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //If the host left and the game is not in progress (has finished), then we want to give the new host the ability to restart the game.
            if (!GameInProgress)
            {
                //Check if there are still remaining players. (We don't want to restart the game if there are no opponents left). Only display the button for the master client, as the host has ultimate control of when to start a rematch.
                if (PhotonNetwork.PlayerList.Length > 1 && PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer)
                {
                    restartGameButtons[0].SetActive(true);
                    restartGameButtons[1].SetActive(true);
                    restartGameButtons[2].SetActive(true);
                }
            }
            else if(PhotonNetwork.LocalPlayer == newMasterClient)
            {
                StartCoroutine(TimerDelay()); //Ensures the timer continues if the game is still in progress. (Match timer is synced across all clients).
            }
        }

        #endregion

        #region Menu Displays

        public void DisplayPauseMenu(bool active) //Note that the pause menu does NOT pause the entire match. It only disables the player movement and abilities.
        {
            pauseCanvas.SetActive(active);
            PlayerDisplay.PlayerCanvas.enabled = !active;
            PlayerController.DisablePlayerActions(active);
        }

        private void DisplayEndScreen(Player winner)
        {
            //Note that only the master client (leader/creator of the room) can restart the match

            if(winner == null) //If there is no winner, and the result of the match is a draw
            {
                drawCanvas.SetActive(true);
                restartGameButtons[2].SetActive(PhotonNetwork.IsMasterClient);
            }
            else if (winner == PhotonNetwork.LocalPlayer) //If the local player is the winner
            {
                victoryCanvas.SetActive(true);
                restartGameButtons[0].SetActive(PhotonNetwork.IsMasterClient);
                GameInProgress = false;
            }
            else //If the local player is NOT the winner
            {
                defeatCanvas.SetActive(true);
                playerWonText.text = winner.NickName + " is Victorious!";
                restartGameButtons[1].SetActive(PhotonNetwork.IsMasterClient);
            }

            PlayerDisplay.PlayerCanvas.enabled = false; //Display the heads-up-display
        }

        #endregion

        #region Game Timer

        private void InitialiseTimer()
        {
            _matchTimer = matchTimeInSeconds;
            StartCoroutine(TimerDelay());
        }

        private IEnumerator TimerDelay() //A recursive coroutine that only stops when the timer reaches zero.
        {
            yield return new WaitForSeconds(1);
            _matchTimer--;

            _photonView.RPC("UpdateTimerDisplay", RpcTarget.AllViaServer, _matchTimer); //Called on all clients from the 'PlayerDisplay' component on this object

            //Repeat the process of decreasing the timer and displaying the remaining time until the timer runs out.
            if (_matchTimer <= 0)
            {
                _photonView.RPC("FindWinner", RpcTarget.AllViaServer); //Called on all clients
            }
            else
            {
                StartCoroutine(TimerDelay());
            }
        }

        #endregion

        [PunRPC]
        private void FindWinner() //Find the result of the match. Victory, defeat or draw.
        {
            if(GameInProgress)
            {
                StopAllCoroutines();
                GameInProgress = false;
                RespawnManager.RespawnCanvas.enabled = false; //Disable the respawn canvas (in the case that the local player is that last player that died).

                _matchTimer = 0;

                int winnerIndex = 0;
                int maxScore = int.MinValue;

                bool draw = false;

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    int score = PhotonNetwork.PlayerList[i].GetScore();

                    if (score > maxScore) //Set the current player we are checking to be the new winner.
                    {
                        winnerIndex = i;
                        maxScore = score;
                    }
                    else if (score == maxScore) //A draw has occured, as more than one player has the same score.
                    {
                        Debug.Log("DRAW!");
                        draw = true;
                    }
                }

                //Display the end screen based on whether the local result is victory, defeat or draw.
                if (draw)
                {
                    DisplayEndScreen(null);
                }
                else
                {
                    DisplayEndScreen(PhotonNetwork.PlayerList[winnerIndex]); //Pass in the winner to check what screen to display (defeat or victory).
                }
            }
        }

        private void StorePersonalBest()
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;

            int currentScore = localPlayer.GetScore();
            PlayerData playerData = SaveManager.PlayerData;

            if(currentScore > playerData.bestScore)
            {
                Debug.Log("Personal best stored!");

                playerData.username = localPlayer.NickName;
                playerData.bestScore = currentScore;
                playerData.bestScoreDate = DateTime.UtcNow.ToString();
                playerData.totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
                playerData.roomName = PhotonNetwork.CurrentRoom.Name;

                SaveManager.SavePlayerData();
            }
        }    
    }
}
