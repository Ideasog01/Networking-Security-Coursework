using TMPro;
using UnityEngine;

namespace Singleplayer
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int enemyCount; //The current number of enemies still alive

        [SerializeField] private TextMeshProUGUI enemyCountText;

        [SerializeField] private GameObject victoryCanvas;

        [SerializeField] private PlayerController playerController;

        private void Start()
        {
            enemyCountText.text = "Enemies Remaining: " + enemyCount.ToString();
            HealthController.OnEnemyKilled += OnEnemyKilledAction;
        }

        public void OnEnemyKilledAction() //When an enemy is eliminated, update the number of enemies.
        {
            enemyCount--;
            enemyCountText.text = "Enemies Remaining: " + enemyCount.ToString(); //Objective Display

            if (enemyCount <= 0) //When all enemies have been defeated display end screen and disable player.
            {
                victoryCanvas.SetActive(true);
                playerController.DisablePlayer(true);
            }
        }
    }
}