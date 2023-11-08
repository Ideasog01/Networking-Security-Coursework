using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    public static Slider PlayerHealthSlider;
    public static Slider[] AbilitySliderArray;
    public static TextMeshProUGUI[] AbilityTextArray;


    [SerializeField] private int maxKills = 3;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private Text winnerText;

    [Header("HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider[] abilitySliderArray;
    [SerializeField] private TextMeshProUGUI[] abilityTextArray;

    private void Start()
    {
        PlayerHealthSlider = healthSlider;
        AbilitySliderArray = abilitySliderArray;
        AbilityTextArray = abilityTextArray;

        GameObject obj = PhotonNetwork.Instantiate("Player_Multiplayer", Vector3.zero, Quaternion.identity);
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer.GetScore() == maxKills)
        {
            //winnerText.text = targetPlayer.NickName;
            //gameOverPopup.SetActive(true);
            Debug.Log(targetPlayer.NickName + " won!");
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
