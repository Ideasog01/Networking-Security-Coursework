using UnityEngine.UI;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    public Text orderText;
    public Text usernameText;
    public Text scoreText;

    public void SetScores(int order, string username, int score)
    {
        orderText.text = order.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}
