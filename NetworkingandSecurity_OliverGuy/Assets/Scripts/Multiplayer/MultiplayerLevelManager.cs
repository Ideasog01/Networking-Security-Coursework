using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Slider PlayerHealthSlider;
    public static Slider[] AbilitySliderArray;
    public static TextMeshProUGUI[] AbilityTextArray;
    public static bool GameInProgress;

    [Header("Game Objective")]

    [SerializeField] private int maxKills = 3;
    [SerializeField] private float matchTimeInSeconds;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject defeatCanvas;
    [SerializeField] private TextMeshProUGUI playerWonText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider[] abilitySliderArray;
    [SerializeField] private TextMeshProUGUI[] abilityTextArray;
    [SerializeField] private Canvas playerCanvas;

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

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseCanvas;

    private List<int> _closedSpawnPositionsList = new List<int>();

    private GameObject _playerObj;

    private PhotonView _photonView;

    private int _respawnTimer;
    private float _matchTimer;

    private void Start()
    {
        GameInProgress = true;

        PlayerHealthSlider = healthSlider;
        AbilitySliderArray = abilitySliderArray;
        AbilityTextArray = abilityTextArray;

        _playerObj = PhotonNetwork.Instantiate("Player_Multiplayer", spawnPositionArray[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
        cameraTracking.PlayerTransform = _playerObj.transform.GetChild(0);

        _photonView = this.GetComponent<PhotonView>();

        Debug.Log("Player Count: " + PhotonNetwork.LocalPlayer.ActorNumber);

        objectiveText.text = "Eliminations Remaining: " + maxKills.ToString();

        if(PhotonNetwork.IsMasterClient)
        {
            InitialiseTimer();
        }
    }

    private void Update()
    {
        if (!playerCanvas.enabled)
        {
            respawnSlider.maxValue = respawnTime;
            respawnSlider.value = Mathf.LerpUnclamped(respawnSlider.value, _respawnTimer, 1);
        }
    }

    public void LeaveGame() //Via Inspector (Button)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public void RestartGame() //Via Inspector (Button)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        
        PhotonNetwork.LoadLevel("LoadingScene");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer.GetScore() == maxKills)
        {
            if(targetPlayer == PhotonNetwork.LocalPlayer)
            {
                victoryCanvas.SetActive(true);
            }
            else
            {
                defeatCanvas.SetActive(true);
                playerWonText.text = targetPlayer.NickName + " is Victorious!";
            }

            respawnAnimator.SetBool("Respawning", false);
            playerCanvas.enabled = false;
            GameInProgress = false;
        }

        if(targetPlayer.IsLocal)
        {
            objectiveText.text = "Eliminations Remaining: " + (maxKills - targetPlayer.GetScore()).ToString();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameInProgress);
            //stream.SendNext(_closedSpawnPositionsList);
            stream.SendNext(_matchTimer);
        }
        else if (stream.IsReading)
        {
            GameInProgress = (bool)stream.ReceiveNext();
            //_closedSpawnPositionsList = (List<int>)stream.ReceiveNext();
            _matchTimer = (float)stream.ReceiveNext();
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
            _playerObj.transform.GetChild(0).GetComponent<PlayerMovement>().StopMovement = true;
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
        _playerObj.transform.GetChild(0).GetComponent<Multiplayer>().DisablePlayer(active);
    }

    private void InitialiseTimer()
    {
        _matchTimer = matchTimeInSeconds;
        _photonView.RPC("UpdateGameTimerDisplay", RpcTarget.AllViaServer);
        StartCoroutine(MatchTimer());
    }

    [PunRPC]
    private void UpdateGameTimerDisplay()
    {
        string minutes = (Mathf.FloorToInt(_matchTimer / 60)).ToString("00");
        string seconds = (_matchTimer % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;
    }

    private IEnumerator MatchTimer()
    {
        yield return new WaitForSeconds(1);

        _matchTimer--;
        _photonView.RPC("UpdateGameTimerDisplay", RpcTarget.AllViaServer);

        if(_matchTimer > 0)
        {
            StartCoroutine(MatchTimer());
        }
        else
        {
            _photonView.RPC("FindWinner", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void FindWinner()
    {
        int index = 0;
        int minScore = int.MaxValue;

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int score = PhotonNetwork.PlayerList[i].GetScore();

            if(score < minScore)
            {
                index = i;
            }
            else if(score == minScore)
            {
                Debug.Log("DRAW!");
            }
        }

        Player winner = PhotonNetwork.PlayerList[index];

        if(PhotonNetwork.LocalPlayer == winner)
        {
            victoryCanvas.SetActive(true);
        }
        else
        {
            defeatCanvas.SetActive(true);
            playerWonText.text = winner.NickName + " is Victorious!";
        }

        respawnAnimator.SetBool("Respawning", false);
        playerCanvas.enabled = false;
        GameInProgress = false;
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(1);

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

            _playerObj.transform.GetChild(0).position = spawnPositionArray[UnityEngine.Random.Range(0, spawnPositionArray.Length - 1)].position;

            cameraTracking.PlayerTransform = _playerObj.transform.GetChild(0);
            _playerObj.transform.GetChild(0).GetComponent<PlayerMovement>().StopMovement = false;
            _playerObj.transform.GetChild(0).GetComponent<MultiplayerHealthController>().ResetPlayer();
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
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
