using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

namespace Multiplayer
{
    public class RespawnManager : MonoBehaviour
    {
        [Header("Respawning Display")]

        [SerializeField] private Canvas respawnCanvas;
        [SerializeField] private TextMeshProUGUI respawnTimerText;
        [SerializeField] private Slider respawnSlider;
        [SerializeField] private TextMeshProUGUI eliminationText;

        [Header("Respawning Settings")]

        [SerializeField] private int respawnTime;
        [SerializeField] private Animator respawnAnimator;
        [SerializeField] private Transform[] spawnPositionArray;

        private int _respawnTimer;

        private List<int> _closedSpawnPositionsList = new List<int>(); //The spawn positions that are no longer available for a short time. (Avoids players respawning close to one another, making it difficult to react).

        #region Properties

        public Transform[] SpawnPositionArray
        {
            get { return spawnPositionArray; }
        }

        public Canvas RespawnCanvas
        {
            get { return respawnCanvas; }
        }

        #endregion

        #region Core

        private void Start()
        {
            _respawnTimer = 0;
        }

        private void Update()
        {
            if (_respawnTimer > 0) //If respawning is currently active, update the slider display.
            {
                respawnSlider.maxValue = respawnTime;

                if (respawnSlider.value > _respawnTimer) //Display only. Smoothly decreases the respawn slider to match with the current time remaining.
                {
                    respawnSlider.value -= Time.deltaTime * 1;
                }
            }
        }

        #endregion

        public void PlayerDeath(Player player, Player attacker, GameObject attackerObj) //When the player has died, the respawn screen needs to display with useful information.
        {
            if (PhotonNetwork.LocalPlayer == player && GameManager.GameInProgress) //Only display when game is in progress and is a local player
            {
                _respawnTimer = respawnTime;
                respawnAnimator.SetBool("Respawning", true);
                eliminationText.text = "You were eliminated by " + attacker.NickName;

                GameManager.CameraTracking.TargetTransform = attackerObj.transform; //Centres the camera on the attacker for ease of understanding.

                GameManager.PlayerController.PlayerMovement.StopMovement = true;
                GameManager.PlayerDisplay.PlayerCanvas.enabled = false;

                StartCoroutine(RespawnTimer());
            }
        }

        private IEnumerator RespawnTimer() //Decreases the respawn timer overtime each second. This recursive function will stop when the timer reaches zero.
        {
            yield return new WaitForSeconds(1);

            if (!GameManager.GameInProgress) //If the game is no longer in progress, we do not need to display the respawn screen.
            {
                respawnAnimator.SetBool("Respawning", false);
                respawnCanvas.enabled = false;
                _respawnTimer = 0;
                yield return null;
            }

            _respawnTimer--; //Decrease the timer every second.

            //Updates the display. (Slider is updated overtime each tick)
            respawnTimerText.text = _respawnTimer.ToString();

            if (_respawnTimer > 0) //Continue calling this function until timer reaches zero.
            {
                StartCoroutine(RespawnTimer());
            }
            else
            {
                //Find a suitable random location for the player to respawn.
                int spawnIndex = UnityEngine.Random.Range(0, spawnPositionArray.Length - 1);

                int attempts = 0; //To avoid stackoverflow in rare chance.
                while (_closedSpawnPositionsList.Contains(spawnIndex) && attempts < 100)
                {
                    spawnIndex = UnityEngine.Random.Range(0, spawnPositionArray.Length);
                    attempts++;
                }

                _closedSpawnPositionsList.Add(spawnIndex); //Disable this position for other players.
                StartCoroutine(RemoveSpawnIndex(spawnIndex)); //Enable the position after several seconds.

                Transform spawnPosition = spawnPositionArray[spawnIndex];
                GameManager.PlayerController.transform.position = spawnPosition.position;

                //Reset player properties to default.
                GameManager.CameraTracking.TargetTransform = GameManager.PlayerController.transform;
                GameManager.PlayerController.PlayerMovement.StopMovement = false;
                GameManager.PlayerController.HealthController.ResetPlayer();
                GameManager.PlayerDisplay.PlayerCanvas.enabled = true;

                respawnAnimator.SetBool("Respawning", false);
            }
        }

        private IEnumerator RemoveSpawnIndex(int index) //Enable the spawn position (at the index) to be available again after a few seconds. This will allow players that die in the future to respawn at this location.
        {
            yield return new WaitForSeconds(8);
            _closedSpawnPositionsList.Remove(index);
        }
    }
}
