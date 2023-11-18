using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static MultiplayerChatDisplay MultiplayerChatDisplay;

    public static Slider PlayerHealthSlider;
    public static Slider[] AbilitySliderArray;
    public static TextMeshProUGUI[] AbilityTextArray;
    public static bool GameInProgress;

    [Header("Game Objective")]

    [SerializeField] private int maxScore;
    [SerializeField] private float matchTimeInSeconds;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject defeatCanvas;
    [SerializeField] private GameObject drawCanvas;
    [SerializeField] private TextMeshProUGUI playerWonText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider[] abilitySliderArray;
    [SerializeField] private TextMeshProUGUI[] abilityTextArray;
    [SerializeField] private Canvas playerCanvas;

    [SerializeField] private GameObject[] restartGameButtons;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Spawning")]

    [SerializeField] private Transform[] spawnPositionArray;
    [SerializeField] private CameraTracking cameraTracking;

    [Header("Respawning")]
    [SerializeField] private Animator respawnAnimator;
    [SerializeField] private TextMeshProUGUI respawnText;
    [SerializeField] private Slider respawnSlider;
    [SerializeField] private int respawnTime;
    [SerializeField] private TextMeshProUGUI eliminationText;
    [SerializeField] private Canvas respawnCanvas;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseCanvas;

    private List<int> _closedSpawnPositionsList = new List<int>();

    private MultiplayerPlayerController _playerController;

    private PhotonView _photonView;

    private int _respawnTimer;
    private float _matchTimer;

    private void Start()
    {
        MultiplayerChatDisplay = this.GetComponent<MultiplayerChatDisplay>();

        GameInProgress = true;

        PlayerHealthSlider = healthSlider;
        AbilitySliderArray = abilitySliderArray;
        AbilityTextArray = abilityTextArray;
        _matchTimer = 0;
        _respawnTimer = 0;

        GameObject obj = PhotonNetwork.Instantiate("Player_Multiplayer", spawnPositionArray[PhotonNetwork.LocalPlayer.ActorNumber].position, Quaternion.identity);
        _playerController = obj.transform.GetChild(0).GetComponent<MultiplayerPlayerController>();
        cameraTracking.PlayerTransform = _playerController.transform;

        _photonView = this.GetComponent<PhotonView>();

        Debug.Log("Player Count: " + PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.LocalPlayer.SetScore(0);
        objectiveText.text = "Eliminations: " + PhotonNetwork.LocalPlayer.GetScore().ToString();

        MultiplayerChatDisplay.CallNewMessage(PhotonNetwork.LocalPlayer.NickName + " joined the game.", Color.red, false);

        if (PhotonNetwork.IsMasterClient)
        {
            InitialiseTimer();
        }
    }

    private void Update()
    {
        if (!playerCanvas.enabled)
        {
            respawnSlider.maxValue = respawnTime;
            respawnSlider.value = Mathf.Lerp(respawnSlider.value, _respawnTimer, 1);
        }
    }

    public void LeaveGame() //Via Inspector (Button)
    {
        if(PhotonNetwork.PlayerList.Length == 2)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                if(player != PhotonNetwork.LocalPlayer)
                {
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
        PhotonNetwork.LoadLevel("LoadingScene");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.IsLocal)
        {
            objectiveText.text = "Eliminations: " + targetPlayer.GetScore().ToString();

            if(targetPlayer.GetScore() == maxScore)
            {
                _photonView.RPC("FindWinner", RpcTarget.AllViaServer);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameInProgress);
        }
        else if (stream.IsReading)
        {
            GameInProgress = (bool)stream.ReceiveNext();
        }
    }

    public void PlayerDeath(Player player, Player attacker, GameObject attackerObj)
    {
        if(PhotonNetwork.LocalPlayer == player && GameInProgress)
        {
            _respawnTimer = respawnTime;
            respawnAnimator.SetBool("Respawning", true);
            eliminationText.text = "You were eliminated by " + attacker.NickName;
            cameraTracking.PlayerTransform = attackerObj.transform;
            _playerController.PlayerMovement.StopMovement = true;
            playerCanvas.enabled = false;

            respawnSlider.maxValue = respawnTime;
            respawnSlider.value = _respawnTimer;
            respawnText.text = _respawnTimer.ToString();

            StartCoroutine(RespawnTimer());
        }
    }

    public void DisplayPauseMenu(bool active)
    {
        pauseCanvas.SetActive(active);
        playerCanvas.enabled = !active;
        _playerController.DisablePlayer(active);
    }

    private void DisplayEndScreen(int resultIndex, Player winner)
    {
        if (resultIndex == 0)
        {
            victoryCanvas.SetActive(true);
            restartGameButtons[0].SetActive(PhotonNetwork.IsMasterClient);
            GameInProgress = false;
        }
        else if(resultIndex == 1)
        {
            defeatCanvas.SetActive(true);
            playerWonText.text = winner.NickName + " is Victorious!";
            restartGameButtons[1].SetActive(PhotonNetwork.IsMasterClient);
        }
        else
        {
            drawCanvas.SetActive(true);
            restartGameButtons[2].SetActive(PhotonNetwork.IsMasterClient);
        }    

        playerCanvas.enabled = false;
    }

    private void InitialiseTimer()
    {
        _photonView.RPC("UpdateGameTimerDisplay", RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void UpdateGameTimerDisplay()
    {
        _matchTimer = matchTimeInSeconds;
        StartCoroutine(TimerDelay());
    }

    private IEnumerator TimerDelay()
    {
        yield return new WaitForSeconds(1);
        _matchTimer--;

        string minutes = (Mathf.FloorToInt(_matchTimer / 60)).ToString("00");
        string seconds = (_matchTimer % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;

        if(_matchTimer <= 0)
        {
            FindWinner();
        }
        else
        {
            StartCoroutine(TimerDelay());
        }
    }

    [PunRPC]
    private void FindWinner()
    {
        if(GameInProgress)
        {
            StopAllCoroutines();
            GameInProgress = false;
            respawnCanvas.enabled = false;

            _matchTimer = 0;

            int index = 0;
            int maxScore = int.MinValue;

            bool draw = false;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                int score = PhotonNetwork.PlayerList[i].GetScore();

                if (score > maxScore)
                {
                    index = i;
                    maxScore = score;
                }
                else if (score == maxScore)
                {
                    Debug.Log("DRAW!");
                    draw = true;
                }
            }

            if (draw)
            {
                DisplayEndScreen(2, null);
            }
            else
            {
                Player winner = PhotonNetwork.PlayerList[index];

                if (PhotonNetwork.LocalPlayer == winner)
                {
                    DisplayEndScreen(0, winner);
                }
                else
                {
                    DisplayEndScreen(1, winner);
                }
            }
        }
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(1);

        if(!GameInProgress)
        {
            respawnAnimator.SetBool("Respawning", false);
            _respawnTimer = 0;
            yield return null;
        }

        _respawnTimer--;

        respawnText.text = _respawnTimer.ToString();

        if(_respawnTimer > 0)
        {
            StartCoroutine(RespawnTimer());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            int spawnIndex = UnityEngine.Random.Range(0, spawnPositionArray.Length - 1);


            int attempts = 0;
            while(_closedSpawnPositionsList.Contains(spawnIndex) && attempts < 100)
            {
                spawnIndex = UnityEngine.Random.Range(0, spawnPositionArray.Length);
                attempts++;
            }

            _closedSpawnPositionsList.Add(spawnIndex);
            StartCoroutine(RemoveSpawnIndex(spawnIndex));

            Transform spawnPosition = spawnPositionArray[spawnIndex];

            _playerController.transform.position = spawnPositionArray[UnityEngine.Random.Range(0, spawnPositionArray.Length - 1)].position;

            cameraTracking.PlayerTransform = _playerController.transform;
            _playerController.PlayerMovement.StopMovement = false;
            _playerController.HealthController.ResetPlayer();
            playerCanvas.enabled = true;
            respawnAnimator.SetBool("Respawning", false);
        }
    }

    private IEnumerator RemoveSpawnIndex(int index)
    {
        yield return new WaitForSeconds(8);
        _closedSpawnPositionsList.Remove(index);
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
        if(PhotonNetwork.PlayerList.Length == 1) //If there is only one player remaining, declare this player the winner.
        {
            if(GameInProgress)
            {
                DisplayEndScreen(0, otherPlayer);
            }

            restartGameButtons[0].SetActive(false);
            restartGameButtons[1].SetActive(false);
            restartGameButtons[2].SetActive(false);
        }

        MultiplayerChatDisplay.CallNewMessage(otherPlayer.NickName + " left the game.", Color.red, true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.PlayerList.Length > 1 && PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) //If there is still an opponent, make sure to continue decreasing the timer every second ONLY on the master client.
        {
            if(!GameInProgress)
            {
                restartGameButtons[0].SetActive(true);
                restartGameButtons[1].SetActive(true);
                restartGameButtons[2].SetActive(true);
            }
        }
    }
}
