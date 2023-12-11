using Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PersonalBestPopup : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject noScoreMessage;
    [SerializeField] private TextMeshProUGUI statisticsText;

    public void UpdatePersonalBestUI()
    {
        PlayerData playerData = saveManager.PlayerData;

        statisticsText.gameObject.SetActive(playerData.username != null);
        noScoreMessage.SetActive(playerData.username == null);

        if(playerData.username != null)
        {
            statisticsText.text = "Username: " + playerData.username;
            statisticsText.text += "\nBest : " + playerData.bestScore.ToString();
            statisticsText.text += "\nDate: " + playerData.bestScoreDate;
            statisticsText.text += "\nTotal Players: " + playerData.totalPlayersInGame.ToString();
            statisticsText.text += "\nRoom Name: " + playerData.roomName;
        }
    }

    private void OnEnable()
    {
        UpdatePersonalBestUI();
    }
}
