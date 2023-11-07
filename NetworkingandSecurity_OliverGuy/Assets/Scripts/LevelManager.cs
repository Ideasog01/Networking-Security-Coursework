using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int enemyCount;

    [SerializeField] private TextMeshProUGUI enemyCountText;

    [SerializeField] private GameObject victoryCanvas;

    [SerializeField] private PlayerActions playerActions;

    private void Start()
    {
        enemyCountText.text = "Enemies Remaining: " + enemyCount.ToString();
        HealthController.OnEnemyKilled += OnEnemyKilledAction;
    }

    public void OnEnemyKilledAction()
    {
        enemyCount--;
        enemyCountText.text = "Enemies Remaining: " + enemyCount.ToString();

        if(enemyCount <= 0)
        {
            victoryCanvas.SetActive(true);
            playerActions.DisablePlayer(true);
        }
    }
}
