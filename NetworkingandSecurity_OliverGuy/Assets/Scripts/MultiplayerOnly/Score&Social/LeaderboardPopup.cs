using Multiplayer;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPopup : MonoBehaviour
{
    public GameObject scoreHolder;
    public GameObject noScoreText;
    public GameObject leaderboardItem;

    private GlobalLeaderboard _globalLeaderboard;

    private void Awake()
    {
        _globalLeaderboard = FindFirstObjectByType<GlobalLeaderboard>();
    }

    public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries) //Update the leaderboard for each entry
    {
        if(playerLeaderboardEntries.Count > 0)
        {
            foreach (Transform child in scoreHolder.transform) //Reset leaderboard display
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerLeaderboardEntries.Count; i++) //For entry, display on the leaderboard menu
            {
                GameObject newLeaderboardItem = Instantiate(leaderboardItem, Vector3.zero, Quaternion.identity, scoreHolder.transform);
                newLeaderboardItem.GetComponent<LeaderboardItem>().SetScores(i + 1, playerLeaderboardEntries[i].DisplayName, playerLeaderboardEntries[i].StatValue);
            }

            scoreHolder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true); //Only display if the number of entries equals to zero
        }


    }

    private void OnEnable()
    {
        _globalLeaderboard.GetLeaderboard();
    }
}
