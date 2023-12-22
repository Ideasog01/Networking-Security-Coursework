using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class HealthController : MonoBehaviour, IPunObservable
    {
        [Header("Statistics")]

        [SerializeField] private int maxHealth;
        [SerializeField] private Slider healthSlider; //This slider will be displayed on the heads-up-display or in-world canvas based on whether the health controller belongs to the local player.

        private int _currentHealth;

        private bool _isInvulnerable;

        private PhotonView _photonView;
        private Animator _characterAnimator;

        #region Properties

        public bool IsInvulnerable
        {
            get { return _isInvulnerable; }
            set { _isInvulnerable = value; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public int CurrentHealth
        {
            get { return _currentHealth; }
        }

        #endregion

        private void Start()
        {
            //Assign components
            _photonView = this.GetComponent<PhotonView>();
            _characterAnimator = this.transform.GetChild(0).GetComponent<Animator>();

            if (_photonView.IsMine)
            {
                healthSlider = GameManager.PlayerDisplay.HealthSlider; //Assign to HUD Slider
            }
            else
            {
                healthSlider = this.transform.GetChild(3).GetChild(0).GetComponent<Slider>(); //Assign to in-world canvas slider under this object
            }

            _currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = _currentHealth;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //Update the health of this player to be the same on all clients.
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_currentHealth);
            }
            else if (stream.IsReading)
            {
                _currentHealth = (int)stream.ReceiveNext();
                healthSlider.value = _currentHealth;
            }
        }

        #region Apply Health

        public void TakeDamage(CollisionController collision)
        {
            if (!_isInvulnerable && _currentHealth > 0)
            {
                _currentHealth -= collision.CollisionDamage;

                Debug.Log("Entity took damage: " + collision.Owner.NickName + "\nHealth: " + _currentHealth.ToString());

                if (_photonView.IsMine) //Only need to update the health display on the local client
                {
                    healthSlider.value = _currentHealth;
                }

                if (_currentHealth <= 0) //When the health reaches zero, the player dies, score is updated for attacker and respawn system is initiated for the local player that was eliminated.
                {
                    if (_photonView.IsMine)
                    {
                        collision.Owner.AddScore(1);
                    }

                    ControllerDeath(collision);
                }
            }
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;

            if (_currentHealth > maxHealth) //The health should not be able to be larger than the maximum health
            {
                _currentHealth = maxHealth;
            }
        }

        #endregion

        #region Player Death & Respawn

        public void ControllerDeath(CollisionController collision)
        {
            if (_photonView.IsMine)
            {
                _characterAnimator.SetBool("isDead", true); //Plays death animation
                GameManager.RespawnManager.PlayerDeath(PhotonNetwork.LocalPlayer, collision.Owner, collision.OwnerCollider.gameObject); //Initiates the respawn display for the local player
            }
        }

        public void ResetPlayer() //When it is time to respawn the player, set character animator state to default and reset health.
        {
            _characterAnimator.SetBool("isDead", false);
            _currentHealth = maxHealth;
            healthSlider.value = _currentHealth;
        }

        #endregion
    }
}
