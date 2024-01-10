using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;
using PlayFab;

public class GlobalLeaderboard : MonoBehaviour
{
    [SerializeField] private int maxResults = 5; //The max results for the leaderboard (number of player scores)
    [SerializeField] private LeaderboardPopup leaderboardPopup; //Manages the leaderboard interface

    public void SubmitScore(int playerScore) //Submits the new score to the leaderboard
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate()
                {
                    StatisticName = "Most Kills", //The name of the leaderboard
                    Value = playerScore,
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatsError);
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest() //Retrieves the leaderboard values for later displaying the menu
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

    private void PlayFabGetLeaderboardResult(GetLeaderboardResult getLeaderboardResult) //Updates the leaderboard menu using the retreived results
    {
        Debug.Log("PlayFab - GetLeadboard completed.");
        leaderboardPopup.UpdateUI(getLeaderboardResult.Leaderboard);
    }

    private void PlayFabGetLeaderboardError(PlayFabError getLeaderboardError)
    {
        Debug.Log("PlayFab - Error occurred while getting Leaderboard: " + getLeaderboardError.ErrorMessage);
    }
}
