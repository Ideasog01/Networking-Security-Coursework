using UnityEngine;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI scoreText;

    public void SetScores(int order, string username, int score)
    {
        orderText.text = order.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}
