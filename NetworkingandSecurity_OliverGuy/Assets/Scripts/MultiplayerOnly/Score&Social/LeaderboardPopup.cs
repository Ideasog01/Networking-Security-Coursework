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

    public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        if(playerLeaderboardEntries.Count > 0)
        {
            foreach (Transform child in scoreHolder.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerLeaderboardEntries.Count; i++)
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
            noScoreText.SetActive(true);
        }


    }

    private void OnEnable()
    {
        _globalLeaderboard.GetLeaderboard();
    }
}
