using Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopup : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    [SerializeField] private GameObject scoreHolder;
    [SerializeField] private GameObject noScoreMessage;

    [Header("Text")]

    [SerializeField] private Text usernameText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private Text dateText;
    [SerializeField] private Text totalPlayersText;
    [SerializeField] private Text roomNameText;

    public void UpdatePersonalBestUI()
    {
        PlayerData playerData = saveManager.PlayerData;

        scoreHolder.SetActive(playerData.username != null);
        noScoreMessage.SetActive(playerData.username == null);

        if(playerData.username != null)
        {
            usernameText.text = playerData.username;
            bestScoreText.text = playerData.bestScore.ToString();
            dateText.text = playerData.bestScoreDate;
            totalPlayersText.text = playerData.totalPlayersInGame.ToString();
            roomNameText.text = playerData.roomName;
        }
    }

    private void OnEnable()
    {
        UpdatePersonalBestUI();
    }
}
