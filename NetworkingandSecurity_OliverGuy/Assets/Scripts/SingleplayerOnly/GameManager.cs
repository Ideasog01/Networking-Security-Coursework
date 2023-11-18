using TMPro;
using UnityEngine;

namespace Singleplayer
{
    public class GameManager : MonoBehaviour
    {
        public static PlayerController PlayerController;

        [SerializeField] private int enemyCount; //The current number of enemies still alive

        [SerializeField] private TextMeshProUGUI enemyCountText;

        [SerializeField] private GameObject victoryCanvas;

        private void Start()
        {
            PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();

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
                PlayerController.DisablePlayer(true);
            }
        }
    }
}