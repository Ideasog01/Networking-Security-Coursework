using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using PlayFab;

public class GlobalLeaderboard : MonoBehaviour
{
    public int maxResults = 5;
    public LeaderboardPopup leaderboardPopup;

    public void SubmitScore(int playerScore)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate()
                {
                    StatisticName = "Most Kills",
                    Value = playerScore,
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatsError);
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            MaxResultsCount = maxResults,
            StatisticName = "Most Kills",
        };

        PlayFabClientAPI.GetLeaderboard(request, PlayFabGetLeaderboardResult, PlayFabGetLeaderboardError);
    }

    private void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
    {
        Debug.Log("PlayFab - Score submitted.");
    }

    private void PlayFabUpdateStatsError(PlayFabError updatePlayerStatisticsError)
    {
        Debug.Log("PlayFab - error occurred while submitting score: " + updatePlayerStatisticsError.ErrorMessage);
    }

    private void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult)
    {
        Debug.Log("PlayFab - GetLeadboard completed.");
        leaderboardPopup.UpdateUI(getLeaderboardResult.Leaderboard);
    }

    private void PlayFabGetLeaderboardError(PlayFabError getLeaderboardError)
    {
        Debug.Log("PlayFab - Error occurred while getting Leaderboard: " + getLeaderboardError.ErrorMessage);
    }
}
